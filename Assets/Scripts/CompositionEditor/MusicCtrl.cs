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
    AudioWaveForm wave_form;

    private void Awake()
    {
        play_btn = transform.Find("btn/play_btn").GetComponent<Button>();
        pause_btn = transform.Find("btn/pause_btn").GetComponent<Button>();
        width_up_btn = transform.Find("btn/width_up_btn").GetComponent<Button>();
        width_down_btn = transform.Find("btn/width_down_btn").GetComponent<Button>();
        wave_form = transform.Find("Scroll View/Viewport/Content/wave_form").GetComponent<AudioWaveForm>();

        play_btn.onClick.AddListener(() => { wave_form.Play(); });
        pause_btn.onClick.AddListener(() => { wave_form.Pause(); });
        width_up_btn.onClick.AddListener(() => { wave_form.WidthUp(); });
        width_down_btn.onClick.AddListener(() => { wave_form.WidthDown(); });
    }
}
