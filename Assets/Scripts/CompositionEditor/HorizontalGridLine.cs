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

    public GameObject horizontal_line;
    public GameObject bar_order_txt;
    public RectTransform bar_order_trans;

    private RectTransform rect;

    private int BPM;
    private int beatPerBar;
    private int grid_density;
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

    // 设置高度
    public void RefreshHeight()
    {
        Vector2 v = rect.sizeDelta;
        v.y = (float)(bar_count * GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale * (beatPerBar * 60.0 / BPM));
        rect.sizeDelta = v;
        bar_order_trans.sizeDelta = v;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        NoteCfg cfg = new NoteCfg();
        double time = this.GetNearestTime(eventData.position.y);
        double position = eventData.position.x * 1920 / Screen.width / CompositionDisplay.Instance.gameWindow.sizeDelta.x;
        cfg.note_type = NoteEditor.Instance.GetNoteType;
        cfg.AddCheckPoint(time, position);
        if (cfg.note_type == (int)NoteType.Hold)
            cfg.AddCheckPoint(time + NoteEditor.Instance.GetDuration * GetOneCellTime, position);
        CompositionDisplay.Instance.CreateNewNote(cfg);
    }

    public float GetNearestTime(float y)
    {
        int index = -1;
        float min_delta = float.MaxValue;
        for (int i=0; i<transform.childCount; ++i)
        {
            float delta = Mathf.Abs(transform.GetChild(i).position.y - y);
            if (delta < min_delta)
            {
                min_delta = delta;
                index = i;
            }
            else break;
        }
        return index * GetOneCellTime + CompositionEditor.Instance.GetTimeOffset;
    }

    public float GetOneCellTime
    {
        get => 60.0f / this.BPM / this.grid_density;
    }
}
