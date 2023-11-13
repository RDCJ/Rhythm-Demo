using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMgr : MonoBehaviour
{
    public enum ScoreLevel
    {
        perfect,
        good,
        bad
    }

    #region Singleton
    private ScoreMgr() { }
    private static ScoreMgr instance;
    public static ScoreMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    float score;
    float accuracy;
    int note_count;
    float basic_score_once;
    float combo_score_point;

    int current_combo;
    int max_combo;

    Dictionary<ScoreLevel, int> score_level_count;

    private void Awake()
    {
        instance = this;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int note_count)
    {
        score = 0;
        accuracy = 0;
        this.note_count = note_count;
        basic_score_once = GameConst.basic_score / note_count;
        combo_score_point = GameConst.combo_score / ((1 + note_count) * note_count / 2);

        current_combo = 0;
        max_combo = 0;

        score_level_count = new Dictionary<ScoreLevel, int>
        {
            { ScoreLevel.perfect, 0},
            { ScoreLevel.good, 0 },
            { ScoreLevel.bad, 0 }
        };
    }

    public void AddScore(ScoreLevel scoreLevel)
    {
        // 计数
        score_level_count[scoreLevel]++;
        // 基础分
        float basic_score = basic_score_once * GameConst.score_factory[scoreLevel];
        // 连击分
        if (scoreLevel == ScoreLevel.bad)
            current_combo = 0;
        else
        {
            current_combo++;
            max_combo = Mathf.Max(max_combo, current_combo);
        }

        float combo_score = current_combo * combo_score_point;

        score += basic_score + combo_score;
    }

    public float GetAccuracy()
    {
        int total_weight = note_count * GameConst.acc_weight[ScoreLevel.perfect];
        int current_weight = 0;
        for (int i=0; i<3; i++)
        {
            current_weight += 1;
        }
        return (float)current_weight / total_weight;
    }
}
