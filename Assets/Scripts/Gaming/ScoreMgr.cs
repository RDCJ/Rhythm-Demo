using UnityEngine;

public class GameResultScore
{
    public float score = 0;
    public int current_combo = 0;
    public int max_combo = 0;
    public int[] score_level_count = new int[3] { 0, 0, 0 };
    public int early_count = 0;
    public int late_count = 0;
    public float accuracy;
    public string tag;
}

public class ScoreMgr
{
    public enum ScoreLevel
    {
        perfect,
        good,
        bad
    }

    #region data
    int note_count;
    float basic_score_once;
    float combo_score_point;
    float judgeTimeOffset;
    public GameResultScore gameResultScore { get; private set; }
    #endregion

    public void Init(int note_count)
    {
        gameResultScore = new GameResultScore();
        
        this.note_count = note_count;
        basic_score_once = GameConst.basic_score / note_count;
        combo_score_point = GameConst.combo_score / ((1 + note_count) * note_count / 2);
        judgeTimeOffset = PlayerPersonalSetting.JudgeTimeOffset;
    }

    public void AddScore(ScoreLevel scoreLevel)
    {
        // 计数
        gameResultScore.score_level_count[(int)scoreLevel]++;
        //Debug.Log("score_level_count: " + score_level_count[0] + " " + score_level_count[1] + " " + score_level_count[2]);
        // 基础分
        float basic_score = basic_score_once * GameConst.score_factory[scoreLevel];
        // 连击分
        if (scoreLevel == ScoreLevel.bad)
            gameResultScore.current_combo = 0;
        else
        {
            gameResultScore.current_combo++;
            gameResultScore.max_combo = Mathf.Max(gameResultScore.max_combo, gameResultScore.current_combo);
        }
        float combo_score = gameResultScore.current_combo * combo_score_point;

        gameResultScore.score += basic_score + combo_score;
        
    }

    public float GetAccuracy()
    {
        int total_weight = note_count * GameConst.acc_weight[ScoreLevel.perfect];
        int current_weight = 0;
        for (int i=0; i<3; i++)
        {
            current_weight += gameResultScore.score_level_count[i] * GameConst.acc_weight[(ScoreLevel)i];
            Debug.Log(gameResultScore.score_level_count[i] + " " + GameConst.acc_weight[(ScoreLevel)i]);
        }
        Debug.Log($"current_weight: {current_weight}, total_weight: {total_weight}");
        return (float)current_weight / total_weight;
    }

    public void OnMusicEnd()
    {
        gameResultScore.score = Mathf.Round(gameResultScore.score);
        gameResultScore.accuracy = GetAccuracy();
        Debug.Log($"Score: {gameResultScore.score}, Accuracy: {gameResultScore.accuracy}");
        if (gameResultScore.score_level_count[(int)ScoreLevel.bad] > 0)
        {
            gameResultScore.tag = "None";
        }
        else if (gameResultScore.score_level_count[(int)ScoreLevel.good] > 0)
        {
            gameResultScore.tag = "FC";
        }
        else
        {
            gameResultScore.tag = "AP";
        }
        PlayerData.UpdateScore(GameMgr.Instance.music_file_name, GameMgr.Instance.difficulty, (int)gameResultScore.score, gameResultScore.accuracy, gameResultScore.tag);
    }

    /// <summary>
    /// 判定
    /// </summary>
    /// <param name="click_time"></param>
    /// <param name="ref_time"></param>
    /// <returns></returns>
    public ScoreLevel JudgeClickTime(JudgeInterval JudgeIntervalConfig, double click_time, double ref_time)
    {
        double delta = System.Math.Abs(click_time + judgeTimeOffset - ref_time);
        if (delta <= JudgeIntervalConfig.perfect_interval)
            return ScoreLevel.perfect;
        else if (delta <= JudgeIntervalConfig.good_interval)
            return ScoreLevel.good;
        else
            return ScoreLevel.bad;
    }

    public ScoreLevel JudgeHoldEnd(JudgeInterval JudgeIntervalConfig, double click_time, double ref_time)
    {
        double delta = ref_time - (click_time + judgeTimeOffset);
        if (delta <= JudgeIntervalConfig.perfect_interval)
            return ScoreLevel.perfect;
        else if (delta <= JudgeIntervalConfig.good_interval)
            return ScoreLevel.good;
        else
            return ScoreLevel.bad;
    }

    public void CountEarlyOrLate(double click_time, double ref_time)
    {
        if (click_time + judgeTimeOffset > ref_time)
            gameResultScore.late_count += 1;
        else if (click_time + judgeTimeOffset < ref_time)
            gameResultScore.early_count += 1;
    }
}
