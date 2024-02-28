using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JudgeFix : MonoBehaviour
{
    public static string JudgeFixKW = "JudgeFix";

    Text value_txt;
    Button add_btn;
    Button dec_btn;

    private void Awake()
    {
        value_txt = transform.Find("value_txt").GetComponent<Text>();
        add_btn = transform.Find("add_btn").GetComponent<Button>();
        dec_btn = transform.Find("dec_btn").GetComponent<Button>();
        add_btn.onClick.AddListener(() => {
            int value = PlayerPrefs.GetInt(JudgeFixKW, 0);
            value += 1;
            PlayerPrefs.SetInt(JudgeFixKW, value);
            PlayerPrefs.Save();
            RefreshText();
        });

        dec_btn.onClick.AddListener(() => {
            int value = PlayerPrefs.GetInt(JudgeFixKW, 0);
            value -= 1;
            PlayerPrefs.SetInt(JudgeFixKW, value);
            PlayerPrefs.Save();
            RefreshText();
        });
    }

    private void Start()
    {
        RefreshText();
    }

    private void RefreshText()
    {
        value_txt.text = PlayerPrefs.GetInt(JudgeFixKW, 0).ToString() + " ms";
    }

    public static float GetJudgeFix
    {
        get => PlayerPrefs.GetInt(JudgeFixKW, 0) * 0.001f;
    }
}
