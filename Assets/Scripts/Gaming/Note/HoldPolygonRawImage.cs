using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HoldPolygonRawImage : Image
{
    private Music.NoteCfg cfg;
    public Music.NoteCfg Cfg 
    { 
        get { return cfg; }
        set
        {
            if (cfg != value)
            {
                if (value.checkPoints.Count >= 2)
                {
                    cfg = value;
                    checkpoint_count = cfg.checkPoints.Count;
                    GenerateMeshPoint();
                    SetVerticesDirty();
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(transform);
#endif
                }
            }
        }
    }

    private float drop_speed = 500;
    private float width = 250;
    private Vector3[] mesh_points;
    private int checkpoint_count;

    protected override void Awake()
    {
        base.Awake();
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (cfg == null) return;
        if (cfg.checkPoints == null) return;
        if (cfg.checkPoints.Count < 2) return;
        Debug.Log("OnPopulateMesh");

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
        Debug.Log(transform.position + " " + transform.localPosition);
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
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local))
            return false;
        
        for (int i = 0; i < checkpoint_count - 1; i++)
        {
            if (Util.PointInsideTriangle(local, mesh_points[i * 2], mesh_points[i * 2 + 1], mesh_points[i * 2 + 2]) ||
                Util.PointInsideTriangle(local, mesh_points[i * 2 + 1], mesh_points[i * 2 + 2], mesh_points[i * 2 + 3]))
            {
                //Debug.Log(local);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 根据cfg计算mesh
    /// RectTransform适应mesh大小
    /// </summary>
    private void GenerateMeshPoint()
    {
        float current_h = 0;
        mesh_points = new Vector3[checkpoint_count * 2];
        for (int i=0; i<checkpoint_count; i++)
        {
            var p1 = cfg.checkPoints[i];
            mesh_points[i * 2] = new(Screen.width * (float)p1.position_x - width * 0.5f - Screen.width * 0.5f, current_h);
            mesh_points[i * 2 + 1] = new(Screen.width * (float)p1.position_x + width * 0.5f - Screen.width * 0.5f, current_h);
            if (i < checkpoint_count - 1)
            {
                var p2 = cfg.checkPoints[i + 1];
                float delta_time = (float)(p2.time - p1.time);
                current_h += delta_time * drop_speed;
            }
        }

        float min_x = float.MaxValue;
        float max_x = float.MinValue;
        for (int i = 0; i < checkpoint_count * 2; i++)
        {
            min_x = Mathf.Min(min_x, mesh_points[i].x);
            max_x = Mathf.Max(max_x, mesh_points[i].x);
        }

        float rect_width = max_x - min_x;
        float rect_height = current_h;
        Vector3 offset = new Vector3(max_x - rect_width / 2, rect_height / 2, 0);
        for (int i = 0; i < checkpoint_count * 2; i++)
        {
            mesh_points[i] -= offset;
        }

        rectTransform.sizeDelta = new Vector2 (rect_width, rect_height);

/*        Debug.Log("mesh_points:");
        for (int i = 0; i < checkpoint_count; i++)
        {
            Vector3 p1 = mesh_points[i * 2] + transform.localPosition;
            Vector3 p2 = mesh_points[i * 2 + 1] + transform.localPosition;
            Debug.Log(p1 + " " + p2);
        }*/
    }
}