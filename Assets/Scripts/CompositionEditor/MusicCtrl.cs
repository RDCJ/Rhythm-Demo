using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicCtrl : MonoBehaviour, IEventListener
{
    Button play_btn;
    Button pause_btn;
    Button width_up_btn;
    Button width_down_btn;
    Text time_txt;

    private void Awake()
    {
        play_btn = transform.Find("btn/play_btn").GetComponent<Button>();
        pause_btn = transform.Find("btn/pause_btn").GetComponent<Button>();
        width_up_btn = transform.Find("btn/width_up_btn").GetComponent<Button>();
        width_down_btn = transform.Find("btn/width_down_btn").GetComponent<Button>();
        time_txt = transform.Find("time").GetComponent<Text>();
       

        play_btn.onClick.AddListener(() => { EditorEventMgr.Instance.Dispatch((int)EditorEventMgr.EventID.MusicPlay); });
        pause_btn.onClick.AddListener(() => { EditorEventMgr.Instance.Dispatch((int)EditorEventMgr.EventID.MusicPause); });
        width_up_btn.onClick.AddListener(() => { EditorEventMgr.Instance.Dispatch((int)EditorEventMgr.EventID.WaveFormWidthUp); });
        width_down_btn.onClick.AddListener(() => { EditorEventMgr.Instance.Dispatch((int)EditorEventMgr.EventID.WaveFormWidthDown); });
    }

    private void Start()
    {
        EditorEventMgr.Instance.AddListener((int)EditorEventMgr.EventID.WaveFormSliderValueChange, this);
    }

    public void HandleEvent(int event_id, params object[] args)
    {
        switch ((EditorEventMgr.EventID)event_id)
        {
            case EditorEventMgr.EventID.WaveFormSliderValueChange:
                float value = (float)args[0];
                float total_time = AudioWaveForm.Instance.GetAudioLength;
                time_txt.text = new Times(value * total_time) + " / " + new Times(total_time);
                break;
        }
    }
}
