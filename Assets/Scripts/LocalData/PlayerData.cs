using System;
using System.Collections.Generic;

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

    private static string ScoreRecordsSaveFile = "ES3SaveData/ScoreRecords.es3";
    private static ES3SaveObject<Dictionary<string, Dictionary<string, ScoreRecord>>> ES3ScoreRecords;
    private static Dictionary<string, Dictionary<string, ScoreRecord>> ScoreRecords => ES3ScoreRecords.Data;


    public static void Load()
    {
        ES3ScoreRecords = new ES3SaveObject<Dictionary<string, Dictionary<string, ScoreRecord>>>("ScoreRecords", ScoreRecordsSaveFile, PlayerPersonalSetting.esSetting);
    }

    public static void Save()
    {
        ES3ScoreRecords.SaveData();
    }

    public static int GetMaxScore(string music_file_name, string difficulty)
    {
        if (ScoreRecords.TryGetValue(music_file_name, out var music_record))
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
        if (ScoreRecords.TryGetValue(music_file_name, out var music_record))
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
        if (ScoreRecords.TryGetValue(music_file_name, out var music_record))
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
        if (!ScoreRecords.ContainsKey(music_file_name))
            ScoreRecords[music_file_name] = new Dictionary<string, ScoreRecord>();
        if (!ScoreRecords[music_file_name].ContainsKey(difficulty))
            ScoreRecords[music_file_name][difficulty] = new ScoreRecord();

        ScoreRecord record = ScoreRecords[music_file_name][difficulty];

        if (score > GetMaxScore(music_file_name, difficulty))
            record.max_score = score;

        if (accuracy > GetMaxAccuracy(music_file_name, difficulty))
            record.max_accuracy = accuracy;

        if (tag_order[tag] > tag_order[GetTag(music_file_name, difficulty)])
            record.tag = tag;

        ScoreRecords[music_file_name][difficulty] = record;

        Save();
    }
}
