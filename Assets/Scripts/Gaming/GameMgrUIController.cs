using System;
using UnityEngine;
using UnityEngine.UI;

public class GameMgrUIController : MonoBehaviour
{
    public GameMgr gameMgr;
    private Transform UICanvas_tf;
    private GameObject pause_panel;
    private Button pause_btn;
    private Button continue_btn;
    private Button restart_btn;
    private Button back_btn;
    private Text time_txt;
    private Slider time_progress;
    private ScoreUI scoreUI;

    private void Awake()
    {
        UICanvas_tf = transform.Find("UICanvas");
        pause_btn = UICanvas_tf.Find("pause_btn").GetComponent<Button>();
        pause_panel = UICanvas_tf.Find("pause_panel").gameObject;
        continue_btn = pause_panel.transform.Find("btn/continue_btn").GetComponent<Button>();
        restart_btn = pause_panel.transform.Find("btn/restart_btn").GetComponent<Button>();
        back_btn = pause_panel.transform.Find("btn/back_btn").GetComponent<Button>();
        time_txt = UICanvas_tf.Find("time_txt").GetComponent<Text>();
        time_progress = UICanvas_tf.Find("time_progress").GetComponent<Slider>();
        scoreUI = new ScoreUI(UICanvas_tf.Find("ScoreUI"));

        pause_btn.onClick.AddListener(() => {
            gameMgr.FSM.TriggerAnyTransition((int)GameMgr.GameState.Pause);
        });
        continue_btn.onClick.AddListener(() => {
            gameMgr.FSM.TriggerAnyTransition(gameMgr.FSM.LastState);
        });
        restart_btn.onClick.AddListener(() => {
            gameMgr.FSM.TriggerAnyTransition((int)GameMgr.GameState.Restart);
        });
        back_btn.onClick.AddListener(() => {
            gameMgr.Close();
        });

        gameMgr.OnGameInit += OnGameInit;
        gameMgr.OnGamePause += OnGamePause;
        gameMgr.OnGameContinue += OnGameContinue;
        gameMgr.OnAddScore += OnAddScore;
        gameMgr.OnGameEnd += scoreUI.ShowFinalScore;

#if UNITY_EDITOR
        time_txt.gameObject.SetActive(true);
#else
        time_txt.gameObject.SetActive(false);
#endif
    }

    private void Update()
    {
        if (gameMgr.audioSource.clip != null)
        {
            time_txt.text = gameMgr.CurrentTime.ToString("N2") + " / " + gameMgr.audioSource.clip.length.ToString("N2");
            time_progress.value = MathF.Max(0, (float)gameMgr.CurrentTime) / gameMgr.audioSource.clip.length;
        }
    }

    public void OnGameInit()
    {
        pause_btn.gameObject.SetActive(true);
        pause_panel.gameObject.SetActive(false);
        scoreUI.Init();
    }

    private void OnGamePause()
    {
        pause_panel.SetActive(true);
    }

    public void OnGameContinue()
    {
        pause_panel.SetActive(false);
    }

    private void OnAddScore(ScoreMgr.ScoreLevel scoreLevel, GameResultScore currentGameResultScore)
    {
        scoreUI.OnAddScore(currentGameResultScore);
    }
}
