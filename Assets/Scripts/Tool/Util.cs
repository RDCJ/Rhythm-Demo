using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static Vector3 ChangeV3(Vector3 v, int position, float value)
    {
        v[position] = value;
        return v;
    }

    public static Vector2 ChangeV2(Vector2 v, int position, float value)
    {
        v[position] = value;
        return v;
    }
}
