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
    public void T1_WriteAndRead_MixedTypes()
    {
        string stringKey = "stringKey";
        int intKey = 42;
        string stringValue = "Hello, World!";
        int intValue = 123;

        _blackboard.Write(stringKey, stringValue);
        _blackboard.Write(intKey.ToString(), intValue);

        Assert.AreEqual(stringValue, _blackboard.Read<string>(stringKey));
        Assert.AreEqual(intValue, _blackboard.Read<int>(intKey.ToString()));
    }

    [Test]
    public void T2_SingleKeyRead()
    {
        string key = "testKey";
        _blackboard.Write(key, 42);

        bool containsKey = _blackboard.ContainsKey(key);

        Assert.IsTrue(containsKey);
        Assert.AreEqual(42, _blackboard.Read<int>(key));
    }

    [Test]
    public void T3_RemoveKey()
    {
        string key = "testKey";
        _blackboard.Write(key, 42);

        bool removed = _blackboard.Remove(key);

        Assert.IsTrue(removed);
        Assert.IsFalse(_blackboard.ContainsKey(key));
    }

    [Test]
    public void T4_ClearKeys()
    {
        _blackboard.Write("key1", 1);
        _blackboard.Write("key2", "value2");

        _blackboard.Clear();

        Assert.IsFalse(_blackboard.ContainsKey("key1"));
        Assert.IsFalse(_blackboard.ContainsKey("key2"));
    }

    [Test]
    public void T5_NonExistentKey()
    {
        string key = "nonExistentKey";

        Assert.Throws<KeyNotFoundException>(() => _blackboard.Read<int>(key));
    }
}