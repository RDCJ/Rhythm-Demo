using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    #region component
    Text score_txt;
    Text combo_txt;
    Transform final_score_panel;
    Text final_score_txt;
    Text final_acc_txt;
    Text final_perfect_count;
    Text final_good_count;
    Text final_bad_count;
    Text final_early_count;
    Text final_late_count;
    Text extra_tag;
    Button restart_btn;
    Button back_btn;
    #endregion

    #region data
    float score;
    int note_count;
    float basic_score_once;
    float combo_score_point;

    int current_combo;
    int max_combo;

    int[] score_level_count;
    int early_count;
    int late_count;

    float judge_fix;
    #endregion

    private void Awake()
    {
        instance = this;
        score_txt = transform.Find("score_txt").GetComponent<Text>();
        combo_txt = transform.Find("combo_txt").GetComponent<Text>();
        final_score_panel = transform.Find("final_score_panel");
        final_score_txt = final_score_panel.Find("final_score/score_txt").GetComponent<Text>();
        final_acc_txt = final_score_panel.Find("final_score/accuracy").GetComponent<Text>();
        final_perfect_count = final_score_panel.Find("final_score/perfect_count").GetComponent<Text>();
        final_good_count = final_score_panel.Find("final_score/good_count").GetComponent<Text>();
        final_bad_count = final_score_panel.Find("final_score/bad_count").GetComponent<Text>();
        final_early_count = final_score_panel.Find("final_score/early_count").GetComponent<Text>();
        final_late_count = final_score_panel.Find("final_score/late_count").GetComponent<Text>();
        extra_tag = final_score_panel.Find("final_score/extra_tag").GetComponent<Text>();
        restart_btn = final_score_panel.Find("restart_btn").GetComponent<Button>();
        back_btn = final_score_panel.Find("back_btn").GetComponent<Button>();
        restart_btn.onClick.AddListener(() =>
        {
            GameMgr.Instance.stateMachine.ChangeState(GameMgr.Instance.restartState);
        });
        back_btn.onClick.AddListener(() =>
        {
            GameMgr.Instance.Close();
        });
    }

    public void Init(int note_count)
    {
        score = 0;
        score_txt.text = score.ToString().PadLeft(7, '0');
        combo_txt.text = "0";
        this.note_count = note_count;
        basic_score_once = GameConst.basic_score / note_count;
        combo_score_point = GameConst.combo_score / ((1 + note_count) * note_count / 2);

        current_combo = 0;
        max_combo = 0;
        early_count = 0;
        late_count = 0;

        score_level_count = new int[3] { 0, 0, 0 };
        final_score_panel.gameObject.SetActive(false);
        judge_fix = JudgeFix.GetJudgeFix;
    }

    public void AddScore(ScoreLevel scoreLevel)
    {
        // 计数
        score_level_count[(int)scoreLevel]++;
        //Debug.Log("score_level_count: " + score_level_count[0] + " " + score_level_count[1] + " " + score_level_count[2]);
        if (score_level_count[(int)ScoreLevel.bad] > 0)
        {
            JudgeLine.Instance.ChangeColor(0);
        }
        else if (score_level_count[(int)ScoreLevel.good] > 0)
        {
            JudgeLine.Instance.ChangeColor(1);
        }
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
        int score_show = Mathf.RoundToInt(score);
        score_txt.text = (score_show).ToString().PadLeft(7, '0');
        combo_txt.text = current_combo.ToString();
    }

    public float GetAccuracy()
    {
        int total_weight = note_count * GameConst.acc_weight[ScoreLevel.perfect];
        int current_weight = 0;
        for (int i=0; i<3; i++)
        {
            current_weight += score_level_count[i] * GameConst.acc_weight[(ScoreLevel)i];
            Debug.Log(score_level_count[i] + " " + GameConst.acc_weight[(ScoreLevel)i]);
        }
        Debug.Log("current_weight: " + current_weight + " total_weight: " + total_weight);
        return (float)current_weight / total_weight;
    }

    public void ShowFinalScore()
    {
        CommonAnime.Scale(final_score_panel);

        score = Mathf.Round(score);
        float acc = GetAccuracy();
        Debug.Log("Score: " + score + " Accuracy: " + acc);
        final_score_panel.gameObject.SetActive(true);
        final_score_txt.text = ((int)score).ToString().PadLeft(7, '0');
        final_acc_txt.text = (acc * 100).ToString("N2") + "%";
        final_perfect_count.text = score_level_count[(int)ScoreLevel.perfect].ToString();
        final_good_count.text = score_level_count[(int)ScoreLevel.good].ToString();
        final_bad_count.text = score_level_count[(int)ScoreLevel.bad].ToString();
        final_early_count.text = early_count.ToString();
        final_late_count.text = late_count.ToString();

        string tag;
        if (score_level_count[(int)ScoreLevel.bad] > 0)
        {
            tag = "None";
        }
        else if (score_level_count[(int)ScoreLevel.good] > 0)
        {
            tag = "FC";
        }
        else
        {
            tag = "AP";
        }
        extra_tag.gameObject.SetActive(tag != "None");
        extra_tag.text = tag;
        PlayerData.UpdateScore(GameMgr.Instance.music_file_name, GameMgr.Instance.difficulty, (int)score, acc, tag);
    }

    /// <summary>
    /// 判定
    /// </summary>
    /// <param name="click_time"></param>
    /// <param name="ref_time"></param>
    /// <returns></returns>
    public ScoreLevel JudgeClickTime(double click_time, double ref_time)
    {
        double delta = System.Math.Abs(click_time + judge_fix - ref_time);
        if (delta <= GameConst.perfect_interval)
            return ScoreLevel.perfect;
        else if (delta <= GameConst.good_interval)
            return ScoreLevel.good;
        else
            return ScoreLevel.bad;
    }


    public ScoreLevel JudgeHoldEnd(double click_time, double ref_time)
    {
        double delta = ref_time - (click_time + judge_fix);
        if (delta <= GameConst.perfect_interval)
            return ScoreLevel.perfect;
        else if (delta <= GameConst.good_interval)
            return ScoreLevel.good;
        else
            return ScoreLevel.bad;
    }

    public void CountEarlyOrLate(double click_time, double ref_time)
    {
        if (click_time + judge_fix > ref_time)
            late_count += 1;
        else if (click_time + judge_fix < ref_time)
            early_count += 1;
    }
}
