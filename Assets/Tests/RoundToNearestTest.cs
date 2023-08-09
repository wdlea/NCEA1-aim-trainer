using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoundToNearestTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void RoundToNearestTestSimplePasses()
    {
        Assert.IsTrue(Helpers.RoundToNearest(2, -1, 3) == 3);
        Assert.IsTrue(Helpers.RoundToNearest(2, 1, 3) == 1);
        Assert.IsTrue(Helpers.RoundToNearest(2, 1.01F, 3) == 1.01F);
        Assert.IsTrue(Helpers.RoundToNearest(1, 2, 0) == 2);//boundary, highest value in first position
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator RoundToNearestTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
