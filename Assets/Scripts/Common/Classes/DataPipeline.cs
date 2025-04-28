using System;
using System.Collections.Generic;

/// <summary>
/// A data pipeline processing step that changes the data contents.
/// This is the base class for all data pipeline steps.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class DataPipelineStep<T> where T : class
{
    internal virtual T ProcessData(T data)
    {
        return OnStep(data);
    }

    public abstract T OnStep(T data);
}

/// <summary>
/// A data pipeline processing step that only runs if conditions are met. You can define conditions within the class or passing through a function that provides that condition externally via the constructor.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ConditionalDataPipelineStep<T> : DataPipelineStep<T> where T : class
{
    Func<bool> ExternalCondition { get; set; }
    public virtual bool IsConditionMet(T data)
    {
        return true;
    }

    bool AreBothConditionsMet(T data)
    {
        if (ExternalCondition != null)
        {
            return IsConditionMet(data) && ExternalCondition();
        }
        return IsConditionMet(data);
    }

    internal override T ProcessData(T data)
    {
        if (AreBothConditionsMet(data))
        {
            return OnStep(data);
        }
        return data;
    }

    public ConditionalDataPipelineStep(Func<bool> condition = null)
    {
        ExternalCondition = condition;
    }
}

/// <summary>
/// A data pipeline consists of multiple steps to transform data.
/// </summary>
public class DataPipleline<T, J> where T : DataPipelineStep<J> where J : class
{
    private List<T> _queue = new();

    public DataPipleline()
    {
    }

    public DataPipleline(List<T> queue)
    {
        _queue = queue;
    }

    public void AddStep(T item)
    {
        _queue.Add(item);
    }

    public void RemoveStep(T item)
    {
        _queue.Remove(item);
    }

    public void ClearQueue()
    {
        _queue.Clear();
    }

    public void Process(J data)
    {
        foreach (var item in _queue)
        {
            data = item.ProcessData(data);
        }
    }
}