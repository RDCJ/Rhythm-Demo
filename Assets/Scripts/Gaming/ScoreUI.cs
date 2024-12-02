using UnityEngine;
using UnityEngine.UI;

public class ScoreUI
{
    #region component
    Transform transform;
    Text score_txt;
    Text combo_txt;
    Transform final_score_panel;
    Text final_score_txt;
    Text final_acc_txt;
    Text final_maxCombo_txt;
    Text final_perfect_count;
    Text final_good_count;
    Text final_bad_count;
    Text final_early_count;
    Text final_late_count;
    Text extra_tag;
    Button restart_btn;
    Button back_btn;
    #endregion

    public ScoreUI(Transform tf)
    {
        transform = tf;
        score_txt = transform.Find("score_txt").GetComponent<Text>();
        combo_txt = transform.Find("combo_txt").GetComponent<Text>();
        final_score_panel = transform.Find("final_score_panel");

        Transform record_tf = final_score_panel.Find("final_score/Record");
        final_score_txt = record_tf.Find("score_txt").GetComponent<Text>();
        final_acc_txt = record_tf.Find("accuracy").GetComponent<Text>();
        extra_tag = record_tf.Find("extra_tag").GetComponent<Text>();

        Transform counter_tf = final_score_panel.Find("final_score/Counters");
        final_maxCombo_txt = counter_tf.Find("maxCombo_count").GetComponent<Text>();
        final_perfect_count = counter_tf.Find("perfect_count").GetComponent<Text>();
        final_good_count = counter_tf.Find("good_count").GetComponent<Text>();
        final_bad_count = counter_tf.Find("bad_count").GetComponent<Text>();
        final_early_count = counter_tf.Find("early_count").GetComponent<Text>();
        final_late_count = counter_tf.Find("late_count").GetComponent<Text>();

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

    public void Init()
    {
        score_txt.text = 0.ToString().PadLeft(7, '0');
        combo_txt.text = "0";
        final_score_panel.gameObject.SetActive(false);
    }

    public void OnAddScore(GameResultScore currentGameResultScore)
    {
        int score_show = Mathf.RoundToInt(currentGameResultScore.score);
        score_txt.text = (score_show).ToString().PadLeft(7, '0');
        combo_txt.text = currentGameResultScore.current_combo.ToString();
    }

    public void ShowFinalScore(GameResultScore gameResultScore)
    {
        CommonAnime.Scale(final_score_panel);

        gameResultScore.score = Mathf.Round(gameResultScore.score);
        final_score_panel.gameObject.SetActive(true);
        final_score_txt.text = ((int)gameResultScore.score).ToString().PadLeft(7, '0');
        final_acc_txt.text = (gameResultScore.accuracy * 100).ToString("N2") + "%";
        final_maxCombo_txt.text = gameResultScore.max_combo.ToString();
        final_perfect_count.text = gameResultScore.score_level_count[(int)ScoreMgr.ScoreLevel.perfect].ToString();
        final_good_count.text = gameResultScore.score_level_count[(int)ScoreMgr.ScoreLevel.good].ToString();
        final_bad_count.text = gameResultScore.score_level_count[(int)ScoreMgr.ScoreLevel.bad].ToString();
        final_early_count.text = gameResultScore.early_count.ToString();
        final_late_count.text = gameResultScore.late_count.ToString();

        extra_tag.gameObject.SetActive(gameResultScore.tag != "None");
        extra_tag.text = gameResultScore.tag;
    }
}
