using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HoldPolygonImage : Image
{
    public void SetCheckPoints(List<Music.CheckPoint> checkPoints, float drop_speed, float width, float head_time_offset=0) 
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
        GenerateMeshPoint(checkPoints, drop_speed, width, head_time_offset);
        SetVerticesDirty();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(transform);
#endif
    }

    private Vector3[] mesh_points;
    private int checkpoint_count;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        for (int i=0; i< checkpoint_count - 1; i++)
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

        for (int i = 0; i < checkpoint_count - 1; i++)
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
    private void GenerateMeshPoint(List<Music.CheckPoint> checkPoints, float drop_speed, float width, float head_time_offset)
    {
        #region 以起始点为原点计算mesh
        float current_h = 0;
        mesh_points = new Vector3[checkpoint_count * 2];
        for (int i=0; i<checkpoint_count; i++)
        {
            var p1 = checkPoints[i];
            if (i == 0)
            {
                p1 = new Music.CheckPoint(checkPoints[i]);
                double tan_a = (checkPoints[i + 1].time - checkPoints[i].time) / (checkPoints[i + 1].position_x - checkPoints[i].position_x);
                float offset_x = head_time_offset / (float)tan_a;
                p1.position_x -= offset_x;
                p1.time -= head_time_offset;
            }
            else if (i == checkpoint_count - 1)
            {
                p1 = new Music.CheckPoint(checkPoints[i]);
                double tan_a = (checkPoints[i].time - checkPoints[i - 1].time) / (checkPoints[i].position_x - checkPoints[i - 1].position_x);
                float offset_x = head_time_offset / (float)tan_a;
                p1.position_x += offset_x;
                current_h += head_time_offset * drop_speed;
            }
            mesh_points[i * 2] = new(Screen.width * ((float)p1.position_x - 0.5f) - width * 0.5f, current_h);
            mesh_points[i * 2 + 1] = new(Screen.width * ((float)p1.position_x - 0.5f) + width * 0.5f, current_h);
            if (i < checkpoint_count - 1)
            {
                var p2 = checkPoints[i + 1];
                float delta_time = (float)(p2.time - p1.time);
                current_h += delta_time * drop_speed;
            }
        }
        #endregion

        #region 将原点移到整个图案的中心
        float min_x = float.MaxValue;
        float max_x = float.MinValue;
        foreach (var p in checkPoints)
        {
            min_x = Mathf.Min(min_x, (float)p.position_x);
            max_x = Mathf.Max(max_x, (float)p.position_x);
        }

        float rect_height = current_h;
        Vector3 offset = new(0.5f * (max_x + min_x - 1) * Screen.width, rect_height / 2, 0);
        for (int i = 0; i < checkpoint_count * 2; i++)
        {
            mesh_points[i] -= offset;
        }
        #endregion

        #region 计算rectTransform的大小
        float rect_width = 0;
        for (int i = 0; i < checkpoint_count * 2; i++)
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