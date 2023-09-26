using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoxScaling))]
public class BoxScalingEditor : Editor
{
    public bool auto_scale = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BoxScaling box = (BoxScaling)target;
        auto_scale = box.auto_scale;
        if (auto_scale)
        {
            BoxCollider2D collider = box.GetComponent<BoxCollider2D>();
            RectTransform trans = box.GetComponent<RectTransform>();
            if (collider != null && trans != null)
            {
                collider.size = new Vector2(trans.rect.width, trans.rect.height);
            }
        }
    }
}
