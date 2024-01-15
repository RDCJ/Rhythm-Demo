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

    /// <summary>
    /// 判断点是否在三角形内部，叉乘法
    /// </summary>
    /// <returns></returns>
    public static bool PointInsideTriangle()
    {

    }
}
