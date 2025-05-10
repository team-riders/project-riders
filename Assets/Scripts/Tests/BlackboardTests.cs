using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class BlackboardTests
{
    private Blackboard _blackboard;

    [SetUp]
    public void SetUp()
    {
        _blackboard = new Blackboard();
    }

    [Test]
    public void T1_WriteAndReadSingleValue_ShouldReturnCorrectValue()
    {
        string key = "testKey";
        _blackboard.SetValue(key, 42);

        bool containsKey = _blackboard.ContainsKey(key);

        Assert.IsTrue(containsKey);
        Assert.AreEqual(42, _blackboard.GetValue<int>(key));
    }

    [Test]
    public void T2_WriteAndReadMixedTypes_ShouldReturnCorrectValues()
    {
        string stringKey = "stringKey";
        int intKey = 42;
        string stringValue = "Hello, World!";
        int intValue = 123;

        _blackboard.SetValue(stringKey, stringValue);
        _blackboard.SetValue(intKey.ToString(), intValue);

        Assert.AreEqual(stringValue, _blackboard.GetValue<string>(stringKey));
        Assert.AreEqual(intValue, _blackboard.GetValue<int>(intKey.ToString()));
    }

    [Test]
    public void T3_RemoveKey_ShouldNotExist()
    {
        string key = "testKey";
        _blackboard.SetValue(key, 42);

        bool removed = _blackboard.Remove(key);

        Assert.IsTrue(removed);
        Assert.IsFalse(_blackboard.ContainsKey(key));
    }

    [Test]
    public void T4_ClearKeys_ShouldNotExist()
    {
        _blackboard.SetValue("key1", 1);
        _blackboard.SetValue("key2", "value2");

        _blackboard.Clear();

        Assert.IsFalse(_blackboard.ContainsKey("key1"));
        Assert.IsFalse(_blackboard.ContainsKey("key2"));
    }

    [Test]
    public void T5_NonExistentKey_ShouldThrowError()
    {
        string key = "nonExistentKey";

        Assert.Throws<KeyNotFoundException>(() => _blackboard.GetValue<int>(key));
    }
}