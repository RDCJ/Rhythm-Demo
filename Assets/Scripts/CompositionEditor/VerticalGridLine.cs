using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerticalGridLine : MonoBehaviour
{
    #region Singleton
    private VerticalGridLine() { }
    private static VerticalGridLine instance;
    public static VerticalGridLine Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    /// <summary>
    /// 水平线预制体
    /// </summary>
    ObjectPool line_pool;
    HorizontalLayoutGroup layoutGroup;

    private void Awake()
    {
        instance = this;
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        line_pool = new ObjectPool(11, "Prefabs/Editor/vertical_line", this.transform);
        RefreshGridLine(11);
    }

    public void RefreshGridLine(int line_count)
    {
        for (int i = 0; i < transform.childCount; i++)
            line_pool.ReturnObject(transform.GetChild(i).gameObject);
        for (int i=0; i<line_count; i++)
        {
            GameObject new_line = line_pool.GetObject();
        }
        layoutGroup.spacing = (transform as RectTransform).sizeDelta.x / (line_count - 1);
    }

    public float GetPositionX(PointerEventData eventData, bool force_absorb=true)
    {
        float x = eventData.position.x;
        if (force_absorb && CompositionEditor.Instance.NoteAbsorbIsOn)
        {
            x = GetNearestLineX(x);
        }
        return (x - (Screen.width - CompositionEditor.Instance.EditorRealWidth) / 2) / CompositionEditor.Instance.GameWindowRealWidth;
    }

    public int GetNearestLineActiveIndex(float x)
    {
        int index = -1;
        float min_delta = float.MaxValue;
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (!transform.GetChild(i).gameObject.activeSelf) continue;
            float delta = Mathf.Abs(transform.GetChild(i).position.x - x);
            if (delta < min_delta)
            {
                min_delta = delta;
                index = i;
            }
            else break;
        }
        return index;
    }

    public float GetNearestLineX(float x)
    {
        int idx = GetNearestLineActiveIndex(x);
        return transform.GetChild(idx).position.x;
    }
}
