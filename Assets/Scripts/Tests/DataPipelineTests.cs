using System;
using NUnit.Framework;

public class StringNumberData
{
    public string Text { get; set; }
    public int Number { get; set; }
}

public class AppendTextStep : DataPipelineStep<StringNumberData>
{
    private readonly string _textToAppend;

    public AppendTextStep(string textToAppend)
    {
        _textToAppend = textToAppend;
    }

    public override StringNumberData OnStep(StringNumberData data)
    {
        data.Text += _textToAppend;
        return data;
    }
}

public class IncrementNumberStep : DataPipelineStep<StringNumberData>
{
    private readonly int _increment;

    public IncrementNumberStep(int increment)
    {
        _increment = increment;
    }

    public override StringNumberData OnStep(StringNumberData data)
    {
        data.Number += _increment;
        return data;
    }
}

public class ConditionalStep : DataPipelineStep<StringNumberData>
{
    // WTF this isn't how I wanted it to work but uhhhhhhhhhhhhhhh it works?
    private readonly Func<StringNumberData, bool> _condition;
    private readonly DataPipelineStep<StringNumberData> _step;

    public ConditionalStep(Func<StringNumberData, bool> condition, DataPipelineStep<StringNumberData> step)
    {
        _condition = condition;
        _step = step;
    }

    public override StringNumberData OnStep(StringNumberData data)
    {
        if (_condition(data))
        {
            return _step.OnStep(data);
        }
        return data;
    }
}

public class ConditionalAppendTextStep : ConditionalDataPipelineStep<StringNumberData>
{
    private readonly string _textToAppend;

    public ConditionalAppendTextStep(string textToAppend, Func<bool> condition = null) : base(condition)
    {
        _textToAppend = textToAppend;
    }

    public override StringNumberData OnStep(StringNumberData data)
    {
        data.Text += _textToAppend;
        return data;
    }
}

[TestFixture]
public class DataPipelineTests
{
    [Test]
    public void T1_Process_MultipleSteps_ShouldTransformDataCorrectly()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new AppendTextStep(" -> Step1"));
        pipeline.AddStep(new IncrementNumberStep(10));
        pipeline.AddStep(new AppendTextStep(" -> Step2"));
        pipeline.AddStep(new IncrementNumberStep(20));

        pipeline.Process(initialData);

        Assert.AreEqual("Start -> Step1 -> Step2", initialData.Text);
        Assert.AreEqual(30, initialData.Number);
    }

    [Test]
    public void T2_ProcessDifferentSteps_ShouldTransformDataCorrectly()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new AppendTextStep(" -> Step2"));
        pipeline.AddStep(new AppendTextStep(" -> Step1"));
        pipeline.AddStep(new IncrementNumberStep(10));
        pipeline.AddStep(new IncrementNumberStep(20));

        pipeline.Process(initialData);

        Assert.AreEqual("Start -> Step2 -> Step1", initialData.Text);
        Assert.AreEqual(30, initialData.Number);
    }

    [Test]
    public void ClearQueue_ShouldRemoveAllSteps()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        pipeline.AddStep(new AppendTextStep(" -> Step1"));
        pipeline.AddStep(new IncrementNumberStep(10));

        pipeline.ClearQueue();

        Assert.DoesNotThrow(() => pipeline.Process(new StringNumberData { Text = "Start", Number = 0 }));
    }

    [Test]
    public void RemoveStep_ShouldRemoveSpecificStep()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        AppendTextStep stepToRemove = new(" -> Step1");
        pipeline.AddStep(stepToRemove);
        pipeline.AddStep(new IncrementNumberStep(10));

        pipeline.RemoveStep(stepToRemove);

        StringNumberData data = new() { Text = "Start", Number = 0 };
        pipeline.Process(data);

        Assert.AreEqual("Start", data.Text);
        Assert.AreEqual(10, data.Number);
    }

    [Test]
    public void ConditionalStep_ShouldExecuteStep_WhenConditionIsTrue()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new ConditionalStep(
            data => data.Number == 0,
            new AppendTextStep(" -> ConditionMet")
        ));

        pipeline.Process(initialData);

        Assert.AreEqual("Start -> ConditionMet", initialData.Text);
        Assert.AreEqual(0, initialData.Number);
    }

    [Test]
    public void ConditionalStep_ShouldNotExecuteStep_WhenConditionIsFalse()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 10 };

        pipeline.AddStep(new ConditionalStep(
            data => data.Number == 0,
            new AppendTextStep(" -> ConditionMet")
        ));

        pipeline.Process(initialData);

        Assert.AreEqual("Start", initialData.Text);
        Assert.AreEqual(10, initialData.Number);
    }

    [Test]
    public void ConditionalStep_ShouldHandleExternalCondition()
    {
        bool externalCondition = false;
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new ConditionalStep(
            data => externalCondition,
            new IncrementNumberStep(5)
        ));

        pipeline.Process(initialData);
        Assert.AreEqual(0, initialData.Number);

        externalCondition = true;
        pipeline.Process(initialData);
        Assert.AreEqual(5, initialData.Number);
    }

    [Test]
    public void ConditionalStep_ShouldWorkWithMultipleConditions()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new ConditionalStep(
            data => data.Number == 0,
            new AppendTextStep(" -> Zero")
        ));
        pipeline.AddStep(new ConditionalStep(
            data => data.Number > 0,
            new AppendTextStep(" -> Positive")
        ));

        pipeline.Process(initialData);
        Assert.AreEqual("Start -> Zero", initialData.Text);

        initialData.Number = 10;
        pipeline.Process(initialData);
        Assert.AreEqual("Start -> Zero -> Positive", initialData.Text);
    }

    [Test]
    public void ConditionalStep_ShouldChainWithOtherSteps()
    {
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new IncrementNumberStep(10));
        pipeline.AddStep(new ConditionalStep(
            data => data.Number == 10,
            new AppendTextStep(" -> Ten")
        ));
        pipeline.AddStep(new IncrementNumberStep(5));
        pipeline.AddStep(new ConditionalStep(
            data => data.Number == 15,
            new AppendTextStep(" -> Fifteen")
        ));

        pipeline.Process(initialData);

        Assert.AreEqual("Start -> Ten -> Fifteen", initialData.Text);
        Assert.AreEqual(15, initialData.Number);
    }

    [Test]
    public void ConditionalAppendTextStep_ShouldAppendText_WhenConditionIsTrue()
    {
        bool condition = true;
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new ConditionalAppendTextStep(" -> ConditionMet", () => condition));

        pipeline.Process(initialData);

        Assert.AreEqual("Start -> ConditionMet", initialData.Text);
    }

    [Test]
    public void ConditionalAppendTextStep_ShouldNotAppendText_WhenConditionIsFalse()
    {
        bool condition = false;
        DataPipleline<DataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        StringNumberData initialData = new() { Text = "Start", Number = 0 };

        pipeline.AddStep(new ConditionalAppendTextStep(" -> ConditionMet", () => condition));

        pipeline.Process(initialData);

        Assert.AreEqual("Start", initialData.Text);
    }
}