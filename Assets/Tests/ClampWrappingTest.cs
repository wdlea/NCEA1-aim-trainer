using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ClampWrappingTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ClampWrappingTestSimplePasses()
    {
        Assert.AreEqual(0, Helpers.ClampWrapping(0, 0, 1, 360));
        Assert.AreEqual(1, Helpers.ClampWrapping(0, 1, 2, 360));
        Assert.Throws<System.InvalidOperationException>(() => Helpers.ClampWrapping(0, 0, 1, 0));//boundary -wrap point of zero, this could cause unexpected behaviour so it is best to throw error
        Assert.AreEqual(0, Helpers.ClampWrapping(359, 0, 20, 360));//test
        Assert.AreEqual(45, Helpers.ClampWrapping(120, -45, 45, 360));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ClampWrappingTestWithEnumeratorPasses()
    {

        yield return null;
    }
}
