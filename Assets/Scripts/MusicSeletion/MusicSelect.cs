using AirFishLab.ScrollingList;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelect : MonoBehaviour
{
    #region Singleton
    private MusicSelect() { }
    private static MusicSelect instance;
    public static MusicSelect Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
    public GameObject game_mgr;

    [SerializeField]
    private CircularScrollingList _list;
    private Button difficulty_change_btn;
    private Text difficulty_txt;
    private Button play_btn;
    private CanvasGroup record;
    private Text record_score_txt;
    private Text record_acc_txt;
    private Text record_tag;

    private Slider DropSpeedScaleSlider;
    private Text DropSpeedScaleText;

    private Text JudgeTimeOffsetValueText;
    private Button JudgeTimeOffsetAddBtn;
    private Button JudgeTimeOffsetDecBtn;

    private Toggle test_mode_switch;
    private InputField test_mode_start_time_input;
    private bool IsTestMode
    {
        get => GameConst.enable_test_mode && test_mode_switch.isOn;
    }

    private float TestModeStartTime
    {
        get
        {
            if (test_mode_switch.isOn)
            {
                try
                {
                    float t = float.Parse(test_mode_start_time_input.text);
                    return t;
                }
                catch
                {
                    return 0;
                }
            }
            else
                return 0;
        }
    }


    public static string SelectedDifficultyKW = "SelectedDifficulty";

    private void Awake()
    {
        instance = this;
        Transform rightUITf = transform.Find("RightUI");
        play_btn = rightUITf.Find("play_btn").GetComponent<Button>();
        difficulty_change_btn = transform.Find("difficulty_change").GetComponent<Button>();
        difficulty_txt = difficulty_change_btn.transform.Find("Text").GetComponent<Text>();
        record = rightUITf.Find("record/BG").GetComponent<CanvasGroup>();
        record_score_txt = record.transform.Find("score").GetComponent<Text>();
        record_acc_txt = record.transform.Find("accuracy").GetComponent<Text>();
        record_tag = record.transform.Find("tag").GetComponent<Text>();

        Transform gameConfigTf = rightUITf.Find("game_config");

        DropSpeedScaleSlider = gameConfigTf.Find("drop_speed").GetComponent<Slider>();
        DropSpeedScaleText = DropSpeedScaleSlider.transform.Find("scale_txt").GetComponent<Text>();
        DropSpeedScaleSlider.value = PlayerPersonalSetting.NormalizedDropSpeedScale;
        DropSpeedScaleText.text = PlayerPersonalSetting.DropSpeedScale.ToString("N2");
        DropSpeedScaleSlider.onValueChanged.AddListener((float value) =>
        {
            PlayerPersonalSetting.NormalizedDropSpeedScale = value;
            DropSpeedScaleText.text = PlayerPersonalSetting.DropSpeedScale.ToString("N2");
        });

        JudgeTimeOffsetValueText = gameConfigTf.Find("judge_fix/value_txt").GetComponent<Text>();
        JudgeTimeOffsetAddBtn = gameConfigTf.Find("judge_fix/add_btn").GetComponent<Button>();
        JudgeTimeOffsetDecBtn = gameConfigTf.Find("judge_fix/dec_btn").GetComponent<Button>();
        RefreshJudgeTimeOffsetValueText();
        JudgeTimeOffsetAddBtn.onClick.AddListener(() => {
            PlayerPersonalSetting.JudgeTimeOffsetMS += 1;
            RefreshJudgeTimeOffsetValueText();
        });

        JudgeTimeOffsetDecBtn.onClick.AddListener(() => {
            PlayerPersonalSetting.JudgeTimeOffsetMS -= 1;
            RefreshJudgeTimeOffsetValueText();
        });

        Transform test_mode_cfg = rightUITf.Find("test_mode");
        test_mode_switch = test_mode_cfg.Find("Switch").GetComponent<Toggle>();
        test_mode_start_time_input = test_mode_cfg.Find("StartTime").GetComponent<InputField>();
        test_mode_cfg.gameObject.SetActive(GameConst.enable_test_mode);

        play_btn.onClick.AddListener(() =>
        {
            int current_id = _list.GetFocusingContentID();
            MusicListContent content =
            (MusicListContent)_list.ListBank.GetListContent(current_id);
            GameMgr.Instance.StartInitGame(content.music_name, GetSelectedDifficulty, IsTestMode, TestModeStartTime);
        });


        difficulty_txt.text = GetSelectedDifficulty;
        difficulty_change_btn.onClick.AddListener(() =>
        {
            int k = PlayerPrefs.GetInt(SelectedDifficultyKW, 0);
            PlayerPrefs.SetInt(SelectedDifficultyKW, (k + 1) % 3);
            PlayerPrefs.Save();
            difficulty_txt.text = GetSelectedDifficulty;
            RefreshRecord();
        });


    }

    private void RefreshJudgeTimeOffsetValueText()
    {
        JudgeTimeOffsetValueText.text = $"{PlayerPersonalSetting.JudgeTimeOffsetMS} ms";
    }

    public string GetSelectedDifficulty
    {
        get => GameConst.DifficultyIndex[PlayerPrefs.GetInt(SelectedDifficultyKW, 0)];
    }

    public void DisplayFocusingContent()
    {
        var contentID = _list.GetFocusingContentID();
        var centeredContent =
            (MusicListContent)_list.ListBank.GetListContent(contentID);
    }

    public void OnBoxSelected(ListBox MusicListBox)
    {
/*        if (_list.GetFocusingContentID() == MusicListBox.ContentID)
        {
            
        }*/
    }

    public void OnFocusingBoxChanged(
        ListBox prevFocusingBox, ListBox curFocusingBox)
    {
        RefreshRecord(curFocusingBox.ContentID);
    }

    public void OnMovementEnd()
    {

        
    }

    public void Exit()
    {
        Destroy(this.gameObject);
    }

    public void RefreshRecord()
    {
        RefreshRecord(_list.GetFocusingContentID());
    }

    private void RefreshRecord(int current_id)
    {
        record.DOKill();
        record.alpha = 0;
        record.DOFade(1, 0.5f);

        MusicListContent content = (MusicListContent)_list.ListBank.GetListContent(current_id);

        string difficulty = GetSelectedDifficulty;
        int max_score = PlayerData.GetMaxScore(content.music_file_name, difficulty);
        float acc = PlayerData.GetMaxAccuracy(content.music_file_name, difficulty);
        string tag = PlayerData.GetTag(content.music_file_name, difficulty);
        record_score_txt.text = max_score.ToString().PadLeft(7, '0');
        record_acc_txt.text = (acc * 100).ToString("N2") + "%";
        record_tag.text = tag;
        record_tag.gameObject.SetActive(tag != "None");
    }
}
