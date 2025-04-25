using System.Collections.Generic;

public interface IDataPipelineStep<T> where T : class
{
    abstract T ProcessData(T data);
}

public class DataPipleline<T, J> where T : IDataPipelineStep<J> where J : class
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