using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HoldPolygonRawImage : Image
{
    public class CheckPoint
    {
        public double time;
        public double position_x;
        public CheckPoint(double time, double position_x)
        {
            this.time = time;
            this.position_x = position_x;
        }
    }

    public class NoteCfg
    {
        public int note_type;
        public List<CheckPoint> checkPoints;
        public CheckPoint FirstCheckPoint
        {
            get 
            {
                if (checkPoints == null) return null;
                return checkPoints[0]; 
            }
        }
        public CheckPoint LastCheckPoint
        {
            get
            {
                if (checkPoints == null) return null;
                return checkPoints[checkPoints.Count-1];
            }
        }
        public double Duration
        {
            get
            {
                if (checkPoints == null) return 0.0f;
                else
                    return LastCheckPoint.time - FirstCheckPoint.time;
            }
        }
        public NoteCfg()
        {
            checkPoints = new List<CheckPoint>();
        }
        public void AddCheckPoint(double time, double position_x)
        {
            checkPoints.Add(new CheckPoint(time, position_x));
        }
    }

    private NoteCfg cfg;
    public NoteCfg Cfg 
    { 
        get { return cfg; }
        set
        {
            if (cfg != value)
            {
                if (value.checkPoints.Count >= 2)
                {
                    cfg = value;
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

    protected override void Awake()
    {
        base.Awake();
        Debug.Log(transform.position);
        this.alphaHitTestMinimumThreshold = 0.9f;
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (cfg == null) return;
        if (cfg.checkPoints == null) return;
        if (cfg.checkPoints.Count < 2) return;
        Debug.Log("OnPopulateMesh");

        vh.Clear();
        Vector3 center = transform.position;
        float current_h = center.y;
        int point_count = cfg.checkPoints.Count;
        mesh_points = new Vector3[point_count * 2];

        for (int i=0; i< point_count - 1; i++)
        {
            var p1 = cfg.checkPoints[i];
            var p2 = cfg.checkPoints[i+1];
            float delta_time = (float)(p2.time - p1.time);
            var vertices = new Vector3[]
            {
                new(Screen.width * (float)p1.position_x - width * 0.5f - Screen.width * 0.5f, current_h),
                new(Screen.width * (float)p1.position_x + width * 0.5f - Screen.width * 0.5f, current_h),
                new(Screen.width * (float)p2.position_x + width * 0.5f - Screen.width * 0.5f, current_h + delta_time * drop_speed),
                new(Screen.width * (float)p2.position_x - width * 0.5f - Screen.width * 0.5f, current_h + delta_time * drop_speed)
            };
            var uv = new Vector3[4]
            {
                new (0, 0 ,0),
                new (1, 0 ,0),
                new (1, 1 ,0),
                new (0, 1 ,0),
            };
            vh.AddUIVertexQuad(SetVbo(vertices, uv));
            current_h += delta_time * drop_speed;

            mesh_points[i * 2] = vertices[0];
            mesh_points[i * 2 + 1] = vertices[1];
            if (i == point_count - 2)
            {
                mesh_points[i * 2 + 2] = vertices[3];
                mesh_points[i * 2 + 3] = vertices[2];
            }
        }

        Debug.Log("mesh_points:");
        for (int i=0; i<point_count; i++)
        {
            Debug.Log(mesh_points[i * 2] + " " + mesh_points[i * 2 + 1]);
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
        Debug.Log(screenPoint);
        return base.IsRaycastLocationValid(screenPoint, eventCamera);
    }
}