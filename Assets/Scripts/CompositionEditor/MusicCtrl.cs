using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicCtrl : MonoBehaviour
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
       

        play_btn.onClick.AddListener(() => { AudioWaveForm.Instance.Play(); });
        pause_btn.onClick.AddListener(() => { AudioWaveForm.Instance.Pause(); });
        width_up_btn.onClick.AddListener(() => { AudioWaveForm.Instance.WidthUp(); });
        width_down_btn.onClick.AddListener(() => { AudioWaveForm.Instance.WidthDown(); });

        
    }

    private void Start()
    {
        AudioWaveForm.Instance.RegisterSliderValueChange((float value) => {
            time_txt.text = new Times(AudioWaveForm.Instance.GetCurrentAudioTime) + " / " + new Times(AudioWaveForm.Instance.GetAudioLength);
        });
    }
}
