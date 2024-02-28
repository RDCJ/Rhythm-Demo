using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

[Serializable]
public class ScoreRecord
{
    public int max_score;
    public double max_accuracy;
    public string tag;

    public ScoreRecord()
    {
        max_score = 0;
        max_accuracy = 0;
        tag = "None";
    }
}

public class PlayerData
{
    public static Dictionary<string, int> tag_order = new Dictionary<string, int>()
    {
        {"None", 0},
        {"FC", 1 },
        {"AP",2 }
    };

    private static string ScoreRecordsKW = "ScoreRecords";
    private static Dictionary<string, Dictionary<string, ScoreRecord>> score_records;


    public static void Load()
    {
        string jsonStr = PlayerPrefs.GetString(ScoreRecordsKW);
        if (string.IsNullOrEmpty(jsonStr))
        {
            score_records = new Dictionary<string, Dictionary<string, ScoreRecord>>();
        }
        else
        {
            score_records = JsonMapper.ToObject<Dictionary<string, Dictionary<string, ScoreRecord>>>(jsonStr);
        }
    }

    public static void Save()
    {
        string jsonStr = JsonMapper.ToJson(score_records);
        PlayerPrefs.SetString(ScoreRecordsKW, jsonStr);
        PlayerPrefs.Save();
    }

    public static int GetMaxScore(string music_file_name, string difficulty)
    {
        if (score_records.TryGetValue(music_file_name, out var music_record))
        {
            if (music_record.TryGetValue(difficulty, out var value))
                return value.max_score;
            else
                return 0;
        }
        return 0;
    }

    public static float GetMaxAccuracy(string music_file_name, string difficulty)
    {
        if (score_records.TryGetValue(music_file_name, out var music_record))
        {
            if (music_record.TryGetValue(difficulty, out var value))
                return (float)value.max_accuracy;
            else
                return 0;
        }
        return 0;
    }

    public static string GetTag(string music_file_name,  string difficulty)
    {
        if (score_records.TryGetValue(music_file_name, out var music_record))
        {
            if (music_record.TryGetValue(difficulty, out var value))
                return value.tag;
            else
                return "None";
        }
        return "None";
    }

    public static void UpdateScore(string music_file_name, string difficulty, int score, float accuracy, string tag)
    {
        if (!score_records.ContainsKey(music_file_name))
            score_records[music_file_name] = new Dictionary<string, ScoreRecord>();
        if (!score_records[music_file_name].ContainsKey(difficulty))
            score_records[music_file_name][difficulty] = new ScoreRecord();

        ScoreRecord record = score_records[music_file_name][difficulty];

        if (score > GetMaxScore(music_file_name, difficulty))
            record.max_score = score;

        if (accuracy > GetMaxAccuracy(music_file_name, difficulty))
            record.max_accuracy = accuracy;

        if (tag_order[tag] > tag_order[GetTag(music_file_name, difficulty)])
            record.tag = tag;

        score_records[music_file_name][difficulty] = record;

        Save();
    }
}
