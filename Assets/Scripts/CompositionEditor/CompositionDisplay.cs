using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Music;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.EventSystems;

public class CompositionDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    #region Singleton
    private CompositionDisplay() { }
    private static CompositionDisplay instance;
    public static CompositionDisplay Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region prefab
    public GameObject tap_prefab;
    public GameObject leftslide_prefab;
    public GameObject rightslide_prefab;
    public GameObject hold_prefab;
    public GameObject catch_prefab;
    #endregion

    private ScrollRect note_scroll_view;
    private RectTransform content_trans;
    private float scroll_height;
    private Vector2 window_size;
    private HorizontalGridLine horizontal_grid_line;

    private List<EditorNote> notes;

    public RectTransform gameWindow;

    /// <summary>
    /// 鼠标进入区域时显示的预览note
    /// </summary>
    private Transform preview_notes;
    private RectTransform preview_note;

    private void Awake()
    {
        instance = this;
        note_scroll_view = transform.Find("GameManager/NoteScrollView").GetComponent<ScrollRect>();
        content_trans = note_scroll_view.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        horizontal_grid_line = content_trans.Find("horizontal_grid_line").GetComponent< HorizontalGridLine >();
        preview_notes = transform.Find("GameManager/preview_notes");
        scroll_height = note_scroll_view.GetComponent<RectTransform>().sizeDelta.y;
        notes = new List<EditorNote>();
    }

    // Start is called before the first frame update
    void Start()
    {
        window_size = gameWindow.sizeDelta;
        AudioWaveForm.Instance.RegisterSliderValueChange(UpdatePosition);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    /// <summary>
    /// 根据音乐播放的时间设置content的位置
    /// </summary>
    private void UpdatePosition(float audio_time)
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        audio_time *= AudioWaveForm.Instance.GetAudioLength;
        Vector3 content_pos = content_trans.localPosition;
        float content_height = content_trans.sizeDelta.y;
        content_pos.y = -scroll_height + content_height - drop_speed * audio_time + CompositionEditor.Instance.GetTimeOffset * drop_speed;
        content_trans.localPosition = content_pos;
    }

    /// <summary>
    /// 根据cfg创建note
    /// </summary>
    /// <param name="cfg"></param>
    /// <returns></returns>
    public int CreateNewNote(NoteCfg cfg)
    {
        GameObject new_note = null;
        switch (cfg.note_type)
        {
            case (int)Note.NoteType.Tap:
                new_note = Instantiate(tap_prefab, content_trans);
                break;
            case (int)Note.NoteType.LeftSlide:
                new_note = Instantiate(leftslide_prefab, content_trans);
                break;
            case (int)Note.NoteType.RightSlide:
                new_note = Instantiate(rightslide_prefab, content_trans);
                break;
            case (int)Note.NoteType.Hold:
                new_note = Instantiate(hold_prefab, content_trans);
                break;
            case (int)Note.NoteType.Catch:
                new_note = Instantiate(catch_prefab, content_trans);
                break;
        }
        EditorNote editorNote = new_note.GetComponent<EditorNote>();
        notes.Add(editorNote);
        int index = notes.Count - 1;
        editorNote.Set(index, cfg);
        PaintNote(index);
        //Debug.Log("create note: " + cfg.note_type + " time: " + cfg.time + " x: " + cfg.position_x);
        return index;
    }

    public void DeleteNote(int index)
    {
        Destroy(notes[index].gameObject);
        notes[index] = null;
    }

    public void PaintNote(int index)
    {
        if (index < 0 || index >= notes.Count) return;
        if (notes[index] == null) return;
        RectTransform trans = notes[index].GetComponent<RectTransform>();
        NoteCfg cfg = notes[index].cfg;

        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        float x = window_size.x * (float)cfg.FirstCheckPoint().Center();
        float y = drop_speed * (float)(cfg.FirstCheckPoint().time - CompositionEditor.Instance.GetTimeOffset) - content_trans.sizeDelta.y;
        if (cfg.note_type == (int)Note.NoteType.Hold)
        {
            PaintHold(trans, cfg);

            x = 0;
            y += trans.sizeDelta.y * trans.localScale.y / 2.0f;

            trans.anchoredPosition = Util.ChangeV2(trans.anchoredPosition, 0, x);
            trans.localPosition = Util.ChangeV3(trans.localPosition, 1, y);
        }
        else
        {
            trans.localPosition = new Vector3(x, y, 0);
        }
    }

    public void PaintHold(RectTransform note_trans, NoteCfg cfg)
    {
        float drop_speed = GameConst.editor_drop_speed * CompositionEditor.Instance.GetVerticalScale;
        var hold_icon = note_trans.GetComponent<HoldPolygonImage>();
        hold_icon.SetCheckPoints(cfg.checkPoints, drop_speed, window_size.x);

        var head_handle = hold_icon.transform.Find("head_handle") as RectTransform;
        var tail_handle = hold_icon.transform.Find("tail_handle") as RectTransform;
        head_handle.localPosition = hold_icon.HeadCenter;
        head_handle.sizeDelta = Util.ChangeV2(head_handle.sizeDelta, 0, hold_icon.HeadWidth / head_handle.localScale.x);
        tail_handle.localPosition = hold_icon.TailCenter;
        tail_handle.sizeDelta = Util.ChangeV2(tail_handle.sizeDelta, 0, hold_icon.TailWidth / tail_handle.localScale.x);
    }

    public void RepaintAllNote()
    {
        for (int i = 0; i < notes.Count; i++)
            PaintNote(i);
    }

    public void ClearNote()
    {
        foreach (var note in notes)
            Destroy(note.gameObject);
        notes.Clear();
    }

    public void LoadComposition(List<NoteCfg> cfgs)
    {
        horizontal_grid_line.RefreshGridLine();
        window_size = gameWindow.sizeDelta;
        ClearNote();
        foreach (var cfg in cfgs)
            CreateNewNote(cfg);
    }

    public List<NoteCfg> GetComposition()
    {
        List<NoteCfg> composition = new(notes.Count);
        foreach (var note in notes)
        {
            if (note != null)
                composition.Add(note.cfg);
        }
            
        return composition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (preview_note != null)
            preview_note.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (preview_note != null)
        {
            preview_note.gameObject.SetActive(false);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (preview_note == null || preview_note.name != NoteEditor.Instance.GetNoteTypeStr)
        {
            for (int i = 0; i < preview_notes.childCount; i++)
            {
                var child = preview_notes.GetChild(i);
                child.gameObject.SetActive(false);
                if (child.name == NoteEditor.Instance.GetNoteTypeStr)
                {
                    preview_note = child as RectTransform;
                }
            }
            preview_note.gameObject.SetActive(true);
        }

        if (preview_note != null)
        {
            if (preview_note.name == Note.NoteType.Hold.ToString())
            {
                NoteCfg cfg = horizontal_grid_line.GetNoteCfgFromPointer(eventData, (int)Note.NoteType.Hold);
                PaintHold(preview_note, cfg);
                preview_note.anchoredPosition = Util.ChangeV2(preview_note.anchoredPosition, 0, 0);
                preview_note.position = Util.ChangeV3(preview_note.position, 1, horizontal_grid_line.GetNearestLineY(eventData.position.y) + preview_note.sizeDelta.y * 0.5f);
            }
            else
            {
                float x = eventData.position.x;
                float y = horizontal_grid_line.GetNearestLineY(eventData.position.y);
                preview_note.position = new Vector3(x, y, 0);
            }
        }
    }
}
