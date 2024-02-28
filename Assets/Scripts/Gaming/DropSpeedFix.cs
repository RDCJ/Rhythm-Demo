using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 调整下落速度
/// </summary>
public class DropSpeedFix : MonoBehaviour
{
    public static string DropSpeedScaleKW = "DropSpeedScale";
    Text scale_txt;
    Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        scale_txt = transform.Find("scale_txt").GetComponent<Text>();
    }

    private void Start()
    {
        float drop_scale = PlayerPrefs.GetFloat(DropSpeedScaleKW, 0);
        slider.value = drop_scale;
        scale_txt.text = GetDropSpeedScale.ToString("N2");
        slider.onValueChanged.AddListener((float value) =>
        {
            PlayerPrefs.SetFloat(DropSpeedScaleKW, value);
            PlayerPrefs.Save();
            scale_txt.text = GetDropSpeedScale.ToString("N2");
        });
    }

    /// <summary>
    /// 下落速度系数，1~5
    /// </summary>
    /// <returns></returns>
    public static float GetDropSpeedScale
    {
        get
        {
            float drop_scale = PlayerPrefs.GetFloat(DropSpeedScaleKW, 0);
            return drop_scale * 4 + 1;
        }
    }

    /// <summary>
    /// 原始下落速度*自定义系数
    /// </summary>
    /// <returns></returns>
    public static float GetScaledDropSpeed
    {
        get => GetDropSpeedScale * GameConst.drop_speed;
    }
}
