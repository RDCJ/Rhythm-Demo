using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[Serializable]
public class ScoreRecord
{
    public int max_score;
    public float max_accuracy;
    public string tag;
}

public class PlayerData
{
    public static Dictionary<string, int> tag_order = new Dictionary<string, int>()
    {
        {null, 0},
        {"FC", 1 },
        {"AP",2 }
    };

    private static string ScoreRecordsKW = "ScoreRecords";
    private static Dictionary<string, ScoreRecord> score_records;


    public static void Load()
    {
        string jsonStr = PlayerPrefs.GetString(ScoreRecordsKW);
        if (string.IsNullOrEmpty(jsonStr))
        {
            score_records = new Dictionary<string, ScoreRecord>();
        }
        else
        {
            score_records = JsonMapper.ToObject<Dictionary<string, ScoreRecord>>(jsonStr);
        }
    }

    public static void Save()
    {
        string jsonStr = JsonMapper.ToJson(score_records);
        PlayerPrefs.SetString(ScoreRecordsKW, jsonStr);
    }

    public static int GetMaxScore(string music_file_name)
    {
        if (score_records.TryGetValue(music_file_name, out var value))
            return value.max_score;
        return 0;
    }

    public static float GetMaxAccuracy(string music_file_name)
    {
        if (score_records.TryGetValue(music_file_name, out var value))
            return value.max_accuracy;
        return 0;
    }

    public static string GetTag(string music_file_name)
    {
        if (score_records.TryGetValue(music_file_name, out var value))
            return value.tag;
        return null;
    }

    public static void UpdateScore(string music_file_name, int score, float accuracy, string tag)
    {
        if (!score_records.ContainsKey(music_file_name))
            score_records[music_file_name] = new ScoreRecord();
        if (score > GetMaxScore(music_file_name))
            score_records[music_file_name].max_score = score;
        if (accuracy > GetMaxAccuracy(music_file_name))
            score_records[music_file_name].max_accuracy = accuracy;
        if (tag_order[tag] > tag_order[GetTag(music_file_name)])
            score_records[music_file_name].tag = tag;
        Save();
    }
}
