using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> _data = new Dictionary<string, object>();

    public void SetValue<T>(string key, T value)
    {
        if (_data.ContainsKey(key))
        {
            _data[key] = value;
        }
        else
        {
            _data.Add(key, value);
        }
    }

    // Read a value from the blackboard
    public T GetValue<T>(string key)
    {
        if (_data.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        throw new KeyNotFoundException($"Key '{key}' not found or type mismatch.");
    }

    // Check if a key exists
    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }

    // Remove a key from the blackboard
    public bool Remove(string key)
    {
        return _data.Remove(key);
    }

    // Clear all data from the blackboard
    public void Clear()
    {
        _data.Clear();
    }
}