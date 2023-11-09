using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Music;

public class CompositionDisplay : MonoBehaviour
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

    private ScrollRect note_scroll_view;
    private RectTransform content_trans;
    private float scroll_height;

    // ≈‰÷√
    private MusicCfg music_cfg;
    private List<NoteCfg> composition;

    private void Awake()
    {
        instance = this;
        note_scroll_view = transform.Find("GameManager/NoteScrollView").GetComponent<ScrollRect>();
        content_trans = note_scroll_view.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        scroll_height = note_scroll_view.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float audio_time = AudioWaveForm.Instance.GetCurrentAudioTimeNormalize;
        Vector3 content_pos = content_trans.localPosition;
        float content_height = content_trans.sizeDelta.y;
        content_pos.y = - scroll_height + content_height - content_height * audio_time;
        content_trans.localPosition = content_pos;
    }
}
