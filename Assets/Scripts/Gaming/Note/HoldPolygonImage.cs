using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HoldPolygonImage : Image
{
    public Vector3[] mesh_points { get; private set; }
    public int checkpoint_count;
    private float screen_width;

    public void SetCheckPoints(List<Music.CheckPoint> checkPoints, float drop_speed, float screen_width,  float width_extend=0, float head_time_offset=0) 
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
        this.screen_width = screen_width;
        GenerateMeshPoint(checkPoints, drop_speed, width_extend, head_time_offset);
        SetVerticesDirty();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(transform);
#endif
    }

    public Vector3 GetCheckPointCenter(int idx)
    {
        if (2 * idx + 1 < mesh_points.Length)
            return (mesh_points[2 * idx] + mesh_points[2 * idx + 1]) * 0.5f;
        else
        {
            Debug.Log("[HoldPolygonImage.CheckPointCenter] index is out of mesh_points.Length");
            return Vector3.zero;
        }
    }

    public Vector3 GetCheckPointLeft(int idx)
    {
        if (2 * idx + 1 < mesh_points.Length)
            return mesh_points[2 * idx];
        else
        {
            Debug.Log("[HoldPolygonImage.GetCheckPointLeft] index is out of mesh_points.Length");
            return Vector3.zero;
        }
    }

    public Vector3 GetCheckPointRight(int idx)
    {
        if (2 * idx + 1 < mesh_points.Length)
            return mesh_points[2 * idx + 1];
        else
        {
            Debug.Log("[HoldPolygonImage.GetCheckPointRight] index is out of mesh_points.Length");
            return Vector3.zero;
        }
    }



    public float GetCheckPointWidth(int idx)
    {
        if (2 * idx + 1 < mesh_points.Length)
            return mesh_points[2 * idx + 1].x - mesh_points[2 * idx].x;
        else
        {
            Debug.Log("[HoldPolygonImage.CheckPointCenter] index is out of mesh_points.Length");
            return 0;
        }
    }

    /// <summary>
    /// ��һ���ж��������
    /// </summary>
    public Vector3 HeadCenter
    {
        get => GetCheckPointCenter(0);
    }

    /// <summary>
    /// ���һ���ж��������
    /// </summary>
    public Vector3 TailCenter
    {
        get
        {
            int l = mesh_points.Length / 2 - 1;
            return GetCheckPointCenter(l);
         }
    }

    /// <summary>
    /// ��һ���ж���Ŀ��
    /// </summary>
    public float HeadWidth
    {
        get => GetCheckPointWidth(0);
    }

    /// <summary>
    /// ���һ���ж���Ŀ��
    /// </summary>
    public float TailWidth
    {
        get
        {
            int l = mesh_points.Length / 2 - 1;
            return GetCheckPointWidth(l);
        }
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (mesh_points == null) return;
        if (mesh_points.Length <= 0) return;
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
    /// ����cfg����mesh
    /// RectTransform��Ӧmesh��С
    /// </summary>
    private void GenerateMeshPoint(List<Music.CheckPoint> checkPoints, float drop_speed, float width_extend, float head_time_offset)
    {
        void SetMeshPoint(int idx, Music.CheckPoint ckp, float current_h)
        {
            float width = screen_width * (float)(ckp.position_r - ckp.position_l);
            float center_x = screen_width * (float)((ckp.position_l + ckp.position_r) / 2 - 0.5f);
            mesh_points[idx * 2] = new(center_x - width / 2 - width_extend, current_h);
            mesh_points[idx * 2 + 1] = new(center_x + width / 2 + width_extend, current_h);
        }

        #region ��(0, 0)Ϊԭ�����mesh
        float current_h = 0;
        mesh_points = new Vector3[(checkpoint_count + 2) * 2];

        //note��ʼ��Ԥ���ж���
        var first_p = checkPoints[0];
        SetMeshPoint(0, first_p, current_h);
        current_h += head_time_offset * drop_speed;

        //icon����
        for (int i=0; i<checkpoint_count; i++)
        {
            var p1 = checkPoints[i];
            SetMeshPoint(i + 1, p1, current_h);
            if (i < checkpoint_count - 1)
            {
                var p2 = checkPoints[i + 1];
                float delta_time = (float)(p2.time - p1.time);
                current_h += delta_time * drop_speed;
            }
        }

        //noteĩβ��Ԥ���ж���
        current_h += head_time_offset * drop_speed;
        var last_p = checkPoints[checkpoint_count - 1];
        SetMeshPoint(checkpoint_count + 1, last_p, current_h);
        #endregion

        float rect_height = current_h;
        #region ��rectTransform��y�Ƶ�����ͼ��������
        Vector3 offset = new(0, rect_height / 2, 0);
        for (int i = 0; i < (checkpoint_count + 2) * 2; i++)
        {
            mesh_points[i] -= offset;
        }
        #endregion

        #region ����rectTransform�Ĵ�С
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

    /// <summary>
    /// rectTransform.sizeDelta.y
    /// </summary>
    public float Height
    {
        get => rectTransform.sizeDelta.y;
    }
}