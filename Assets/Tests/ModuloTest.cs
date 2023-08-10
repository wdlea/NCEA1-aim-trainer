using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ModuloTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ModuloTestSimplePasses()
    {
        // Use the Assert class to test conditions

        Assert.AreEqual(3, Helpers.Modulo(3, 4));
        Assert.AreEqual(1, Helpers.Modulo(-3, 4));
        Assert.AreEqual(-3, Helpers.Modulo(-3, -4));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ModuloTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
