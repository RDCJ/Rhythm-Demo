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

    private List<EditorNotePainter> notes;

    public RectTransform gameWindow;

    /// <summary>
    /// 鼠标进入区域时显示的预览note
    /// </summary>
    private Transform preview_notes;
    private RectTransform preview_note;

    public float WindowWidth
    {
        get => window_size.x;
    }

    public float ContentHeight
    {
        get => content_trans.sizeDelta.y;
    }

    private void Awake()
    {
        instance = this;
        note_scroll_view = transform.Find("GameManager/NoteScrollView").GetComponent<ScrollRect>();
        content_trans = note_scroll_view.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        horizontal_grid_line = content_trans.Find("horizontal_grid_line").GetComponent< HorizontalGridLine >();
        preview_notes = transform.Find("GameManager/preview_notes");
        scroll_height = note_scroll_view.GetComponent<RectTransform>().sizeDelta.y;
        notes = new List<EditorNotePainter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        window_size = gameWindow.sizeDelta;
        AudioWaveForm.Instance.RegisterSliderValueChange(UpdatePosition);
        preview_notes.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        EditorNotePainter editorNote = new_note.GetComponent<EditorNotePainter>();
        notes.Add(editorNote);
        int index = notes.Count - 1;
        editorNote.Set(index, cfg);
        PaintNote(index);
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
        notes[index].OnPaint();
    }

    public void RepaintAllNote()
    {
        for (int i = 0; i < notes.Count; i++)
            PaintNote(i);
    }

    public void ClearNote()
    {
        foreach (var note in notes)
        {
            if (note != null)
                Destroy(note.gameObject);
        }
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
        if (!CompositionEditor.Instance.BPMHasValue) return;
        if (preview_note != null)
            preview_note.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CompositionEditor.Instance.BPMHasValue) return;
        if (preview_note != null)
        {
            preview_note.gameObject.SetActive(false);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!CompositionEditor.Instance.BPMHasValue) return;
        if (eventData.pointerEnter.name != horizontal_grid_line.gameObject.name)
        {
            if (preview_note != null)
            {
                preview_note.gameObject.SetActive(false);
            }
            return;
        }

        if (preview_note == null || preview_note.name != NoteSelector.Instance.GetNoteTypeStr)
        {
            for (int i = 0; i < preview_notes.childCount; i++)
            {
                var child = preview_notes.GetChild(i);
                child.gameObject.SetActive(false);
                if (child.name == NoteSelector.Instance.GetNoteTypeStr)
                {
                    preview_note = child as RectTransform;
                }
            }
        }

        // 跟随光标移动
        if (preview_note != null)
        {
            preview_note.gameObject.SetActive(true);
            if (preview_note.name == Note.NoteType.Hold.ToString())
            {
                NoteCfg cfg = horizontal_grid_line.GetNoteCfgFromPointer(eventData, (int)Note.NoteType.Hold);
                EditorHoldPainter.PaintHold(preview_note, cfg);
                preview_note.anchoredPosition = Util.ChangeV2(preview_note.anchoredPosition, 0, 0);
                preview_note.position = Util.ChangeV3(preview_note.position, 1, horizontal_grid_line.GetNearestLineY(eventData.position.y));
                preview_note.anchoredPosition = Util.ChangeV2(preview_note.anchoredPosition, 1, preview_note.anchoredPosition.y + preview_note.sizeDelta.y * 0.5f);
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
