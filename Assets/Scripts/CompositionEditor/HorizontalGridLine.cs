using Music;
using Note;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalGridLine : MonoBehaviour, IPointerClickHandler
{
    #region Singleton
    private HorizontalGridLine() { }
    private static HorizontalGridLine instance;
    public static HorizontalGridLine Instance
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
    public GameObject horizontal_line;
    /// <summary>
    /// 小节号文本预制体
    /// </summary>
    public GameObject bar_order_txt;
    /// <summary>
    /// 小节号序列
    /// </summary>
    public RectTransform bar_order_trans;

    private RectTransform rect;


    private int BPM;
    /// <summary>
    /// 每小节节拍数
    /// </summary>
    private int beatPerBar;
    private int grid_density;
    /// <summary>
    /// 小节数量
    /// </summary>
    private int bar_count;

    private void Awake()
    {
        instance = this;
        rect = GetComponent<RectTransform>();
    }

    public void RefreshGridLine()
    {
        int beatPerBar = CompositionEditor.Instance.GetBeatPerBar;
        int BPM = CompositionEditor.Instance.GetBPM;
        int grid_density = CompositionEditor.Instance.GetGridDensity + 1;
        if (beatPerBar > 0 && BPM > 0)
        {
            this.BPM = BPM;
            this.grid_density = grid_density;
            this.beatPerBar = beatPerBar;
            // 清空
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
            for (int i = 0; i < bar_order_trans.childCount; i++)
                Destroy(bar_order_trans.GetChild(i).gameObject);
            //
            float audio_time = AudioWaveForm.Instance.GetAudioLength;
            float bar_time = (float)(beatPerBar * 60.0 / BPM);
            bar_count = (int)Mathf.Ceil(audio_time / bar_time);

            for (int i = 0; i < bar_count; i++)
            {
                GameObject new_order_txt = Instantiate(bar_order_txt, bar_order_trans);
                new_order_txt.GetComponent<Text>().text = "# " + (i + 1).ToString();

                for (int j = 0; j < beatPerBar * grid_density; j++)
                {
                    GameObject new_line = Instantiate(horizontal_line, transform);
                    if (j == 0)
                        new_line.GetComponent<Image>().color = Color.green;
                    else if (j % grid_density == 0)
                        new_line.GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
                    else
                        new_line.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
                }
            }
            RefreshHeight();
        }
        else
            Debug.Log("arguement error");
    }

    /// <summary>
    /// 设置高度
    /// </summary>
    public void RefreshHeight()
    {
        Vector2 v = rect.sizeDelta;
        v.y = (float)(bar_count * GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale * (beatPerBar * 60.0 / BPM));
        rect.sizeDelta = v;
        bar_order_trans.sizeDelta = v;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (CompositionEditor.Instance.BPMHasValue)
        {
            // 点击时创建一个note
            NoteCfg cfg = GetNoteCfgFromPointer(eventData, NoteSelector.Instance.GetNoteType);
            CompositionDisplay.Instance.CreateNewNote(cfg);
        }
    }

    /// <summary>
    /// 根据当前pointer的位置创建一个cfg
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="note_type"></param>
    /// <returns></returns>
    public NoteCfg GetNoteCfgFromPointer(PointerEventData eventData, int note_type)
    {
        NoteCfg cfg = new();
        double time = this.GetNearestTime(eventData.position.y);
        double position = GetPositionX(eventData);
        cfg.note_type = note_type;

        if (cfg.note_type == (int)NoteType.Hold)
        {
            cfg.AddCheckPoint(time, position - 0.05f, position + 0.05f);
            cfg.AddCheckPoint(time + GetOneCellTime, position - 0.05f, position + 0.05f);
        }
        else
        {
            cfg.AddCheckPoint(time, position);
        }
        return cfg;
    }

    /// <summary>
    /// 获取离y最近的水平线的index
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public int GetNearestLineIndex(float y)
    {
        int index = -1;
        float min_delta = float.MaxValue;
        for (int i = 0; i < transform.childCount; ++i)
        {
            float delta = Mathf.Abs(transform.GetChild(i).position.y - y);
            if (delta < min_delta)
            {
                min_delta = delta;
                index = i;
            }
            else break;
        }
        return index;
    }

    /// <summary>
    /// 获取离y最近的水平线的position.y
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public float GetNearestLineY(float y)
    {
        int idx = GetNearestLineIndex(y);
        return transform.GetChild(idx).position.y;
    }

    /// <summary>
    /// 获取离y最近的水平线对应的时刻
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public float GetNearestTime(float y)
    {
        return GetNearestLineIndex(y) * GetOneCellTime + CompositionEditor.Instance.GetTimeOffset;
    }

    public float GetPositionX(PointerEventData eventData)
    {
        return (eventData.position.x - (Screen.width -  CompositionEditor.Instance.EditorRealWidth) / 2) / CompositionEditor.Instance.GameWindowRealWidth;
    }

    /// <summary>
    /// 一节拍时长
    /// </summary>
    public float GetOneCellTime
    {
        get => 60.0f / this.BPM / this.grid_density;
    }
}
