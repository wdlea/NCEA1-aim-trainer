using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Helpers
{
    public static Vector3 RandomVector3Range(Vector3 a, Vector3 b)
    {
        MinMax(a.x, b.x, out float minX, out float maxX);
        MinMax(a.y, b.y, out float minY, out float maxY);
        MinMax(a.z, b.z, out float minZ, out float maxZ);

        return new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Random.Range(minZ, maxZ)
        );
    }
}
