using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IsRunning
{
    // A Test behaves as an ordinary method
    [Test]
    public void VerifyApplicationPlaying()
    {
        Assert.That(Application.isPlaying, Is.True);
    }
}
