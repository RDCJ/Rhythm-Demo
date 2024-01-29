using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HoldPolygonImage : Image
{
    public void SetCheckPoints(List<Music.CheckPoint> checkPoints, float drop_speed, float width_extend=0, float head_time_offset=0) 
    {
        if (checkPoints == null) 
        {
            Debug.Log("[HoldPolygonRawImage] checkPoints == null");
            return;
        }
        if (checkPoints.Count < 2)
        {
            Debug.Log("[HoldPolygonRawImage] checkPoints.Count < 2");
            return;
        }
        checkpoint_count = checkPoints.Count;
        GenerateMeshPoint(checkPoints, drop_speed, width_extend, head_time_offset);
        SetVerticesDirty();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(transform);
#endif
    }

    private Vector3[] mesh_points;
    private int checkpoint_count;

    public Vector3 HeadCenter
    {
        get => (mesh_points[0] + mesh_points[1]) * 0.5f;
    }

    public Vector3 TailCenter
    {
        get 
        {
            int l = mesh_points.Length;
            return (mesh_points[l - 2] + mesh_points[l - 1]) * 0.5f;
         }
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        for (int i=0; i< checkpoint_count + 1; i++)
        {
            var vertices = new Vector3[]
            {
                mesh_points[i * 2],
                mesh_points[i * 2 + 1],
                mesh_points[i * 2 + 3],
                mesh_points[i * 2 + 2]
            };
            var uv = new Vector3[4]
            {
                new (0, 0 ,0),
                new (1, 0 ,0),
                new (1, 1 ,0),
                new (0, 1 ,0),
            };
            vh.AddUIVertexQuad(SetVbo(vertices, uv));
        }
    }

    protected UIVertex[] SetVbo(Vector3[] vertices, Vector3[] uvs)
    {
        UIVertex[] vbo = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = vertices[i];
            vert.uv0 = uvs[i];
            vbo[i] = vert;
        }
        return vbo;
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector2 local))
            return false;

        for (int i = 0; i < checkpoint_count +1; i++)
        {
            if (Util.PointInsideTriangle(local, mesh_points[i * 2], mesh_points[i * 2 + 1], mesh_points[i * 2 + 2]) ||
                Util.PointInsideTriangle(local, mesh_points[i * 2 + 1], mesh_points[i * 2 + 2], mesh_points[i * 2 + 3]))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 根据cfg计算mesh
    /// RectTransform适应mesh大小
    /// </summary>
    private void GenerateMeshPoint(List<Music.CheckPoint> checkPoints, float drop_speed, float width_extend, float head_time_offset)
    {
        #region 以(0, 0)为原点计算mesh
        float current_h = 0;
        mesh_points = new Vector3[(checkpoint_count + 2) * 2];

        //note起始端预留判定区
        var first_p = checkPoints[0];
        mesh_points[0] = new(Screen.width * ((float)first_p.position_l - 0.5f) - width_extend, current_h);
        mesh_points[1] = new(Screen.width * ((float)first_p.position_r - 0.5f) + width_extend, current_h);
        current_h += head_time_offset * drop_speed;

        //icon区域
        for (int i=0; i<checkpoint_count; i++)
        {
            var p1 = checkPoints[i];
            mesh_points[i * 2 + 2] = new(Screen.width * ((float)p1.position_l - 0.5f) - width_extend, current_h);
            mesh_points[i * 2 + 3] = new(Screen.width * ((float)p1.position_r - 0.5f) + width_extend, current_h);
            if (i < checkpoint_count - 1)
            {
                var p2 = checkPoints[i + 1];
                float delta_time = (float)(p2.time - p1.time);
                current_h += delta_time * drop_speed;
            }
        }

        //note末尾端预留判定区
        current_h += head_time_offset * drop_speed;
        var last_p = checkPoints[checkpoint_count - 1];
        mesh_points[(checkpoint_count + 2) * 2 - 2] = new(Screen.width * ((float)last_p.position_l - 0.5f) - width_extend, current_h);
        mesh_points[(checkpoint_count + 2) * 2 - 1] = new(Screen.width * ((float)last_p.position_r - 0.5f) + width_extend, current_h);
        #endregion

        float rect_height = current_h;
        #region 将原点移到整个图案的中心
        float min_x = float.MaxValue;
        float max_x = float.MinValue;
        /*        foreach (var p in checkPoints)
                {
                    min_x = Mathf.Min(min_x, (float)p.position_l);
                    max_x = Mathf.Max(max_x, (float)p.position_r);
                }*/

        for (int i = 2; i < checkpoint_count * 2 - 2; i++)
        {
            min_x = Mathf.Min(min_x, (float)mesh_points[i].x);
            max_x = Mathf.Max(max_x, (float)mesh_points[i].x);
        }

        Vector3 offset = new(0, rect_height / 2, 0);
        for (int i = 0; i < (checkpoint_count + 2) * 2; i++)
        {
            mesh_points[i] -= offset;
        }
        #endregion

        #region 计算rectTransform的大小
        float rect_width = 0;
        for (int i = 0; i < (checkpoint_count + 2) * 2; i++)
        {
            rect_width = MathF.Max(rect_width, MathF.Abs(mesh_points[i].x));
        }
        rectTransform.sizeDelta = new Vector2(rect_width * 2, rect_height);
        #endregion

        /*        Debug.Log("mesh_points:");
                for (int i = 0; i < checkpoint_count; i++)
                {
                    Vector3 p1 = mesh_points[i * 2] + transform.localPosition;
                    Vector3 p2 = mesh_points[i * 2 + 1] + transform.localPosition;
                    Debug.Log(p1 + " " + p2);
                }*/
    }
}