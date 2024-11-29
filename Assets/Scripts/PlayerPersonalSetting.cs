using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPersonalSetting
{
    public static ES3Settings esSetting = new ES3Settings(ES3.Location.File, ES3.Directory.DataPath);
    public static string PersonalGlobalSettingFile = "ES3SaveData/PersonalGlobalSettingFile.es3";

    public static T Load<T>(string key, string filePath, T defultValue = default)
    {
        return ES3.Load(key, filePath, defultValue, esSetting);
    }
    public static void Save<T>(string key, string filePath, T value)
    {
        ES3.Save(key, value, filePath, esSetting);
    }

    private static float _NormalizedDropSpeedScale = -1;
    /// <summary>
    /// 标准化的下落速度系数，0~1
    /// </summary>
    public static float NormalizedDropSpeedScale
    {
        get
        {
            if (_NormalizedDropSpeedScale < 0)
            {
                _NormalizedDropSpeedScale = Load<float>("NormalizedDropSpeedScale", PersonalGlobalSettingFile, 0);
            }
            return _NormalizedDropSpeedScale;
        }
        set
        {
            _NormalizedDropSpeedScale = value;
            Save("NormalizedDropSpeedScale", PersonalGlobalSettingFile, value);
        }
    }

    /// <summary>
    /// 下落速度系数，1~5
    /// </summary>
    /// <returns></returns>
    public static float DropSpeedScale => NormalizedDropSpeedScale * 4 + 1;

    /// <summary>
    /// 原始下落速度*自定义系数
    /// </summary>
    /// <returns></returns>
    public static float ScaledDropSpeed
    {
        get => DropSpeedScale * GameConst.drop_speed;
    }
}
