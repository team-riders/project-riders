using NUnit.Framework;

public class StringNumberData
{
    public string Text { get; set; }
    public int Number { get; set; }
}

public class AppendTextStep : IDataPipelineStep<StringNumberData>
{
    private readonly string _textToAppend;

    public AppendTextStep(string textToAppend)
    {
        _textToAppend = textToAppend;
    }

    public StringNumberData ProcessData(StringNumberData data)
    {
        data.Text += _textToAppend;
        return data;
    }
}

public class IncrementNumberStep : IDataPipelineStep<StringNumberData>
{
    private readonly int _increment;

    public IncrementNumberStep(int increment)
    {
        _increment = increment;
    }

    public StringNumberData ProcessData(StringNumberData data)
    {
        data.Number += _increment;
        return data;
    }
}

[TestFixture]
public class DataPipelineTests
{
    [Test]
    public void T1_Process_MultipleSteps_ShouldTransformDataCorrectly()
    {
        DataPipleline<IDataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
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
        DataPipleline<IDataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
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
        DataPipleline<IDataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        pipeline.AddStep(new AppendTextStep(" -> Step1"));
        pipeline.AddStep(new IncrementNumberStep(10));

        pipeline.ClearQueue();

        Assert.DoesNotThrow(() => pipeline.Process(new StringNumberData { Text = "Start", Number = 0 }));
    }

    [Test]
    public void RemoveStep_ShouldRemoveSpecificStep()
    {
        DataPipleline<IDataPipelineStep<StringNumberData>, StringNumberData> pipeline = new();
        AppendTextStep stepToRemove = new(" -> Step1");
        pipeline.AddStep(stepToRemove);
        pipeline.AddStep(new IncrementNumberStep(10));

        pipeline.RemoveStep(stepToRemove);

        StringNumberData data = new() { Text = "Start", Number = 0 };
        pipeline.Process(data);

        Assert.AreEqual("Start", data.Text);
        Assert.AreEqual(10, data.Number);
    }
}