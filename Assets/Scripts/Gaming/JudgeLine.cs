using UnityEngine;
using UnityEngine.UI;

public class JudgeLine : MonoBehaviour
{
    private Image line_img;
    private RectTransform rectTransform;

    Color[] colors = new Color[3] { Color.white, new(107f/255, 226f/255, 1, 1), new(228f/255, 228f/255, 135f/255, 1)};
    private void Awake()
    {
        line_img = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        GameMgr.Instance.OnGameInit += Reset;
        GameMgr.Instance.OnAddScore += OnAddScore;
    }

    public void Reset()
    {
        rectTransform.localPosition = Util.ChangeV3(rectTransform.localPosition, 1, Screen.height * (GameConst.judge_line_y - 0.5f));
        rectTransform.sizeDelta = Util.ChangeV2(rectTransform.sizeDelta, 0, Screen.width);
        ChangeColor(2);
    }

    public void ChangeColor(int index)
    {
        line_img.color = colors[index];
    }

    private void OnAddScore(ScoreMgr.ScoreLevel scoreLevel, GameResultScore currentGameResultScore)
    {
        if (currentGameResultScore.score_level_count[(int)ScoreMgr.ScoreLevel.bad] > 0)
        {
            ChangeColor(0);
        }
        else if (currentGameResultScore.score_level_count[(int)ScoreMgr.ScoreLevel.good] > 0)
        {
            ChangeColor(1);
        }
    }
}
