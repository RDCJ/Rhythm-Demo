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
    public static bool PointInsideTriangle(Vector3 p, Vector3 t1, Vector3 t2, Vector3 t3)
    {
        Vector3 d1 = t1 - p;
        Vector3 d2 = t2 - p;
        Vector3 d3 = t3 - p;

        Vector3 c1 = Vector3.Cross(d1, d2);
        Vector3 c2 = Vector3.Cross(d2, d3);
        Vector3 c3 = Vector3.Cross(d3, d1);


    }
}
