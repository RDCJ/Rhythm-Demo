using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Music;
using UnityEngine.UI;

public class CompositionEditor : MonoBehaviour
{
    #region Singleton
    private CompositionEditor() { }
    private static CompositionEditor instance;
    public static CompositionEditor Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    // ≈‰÷√
    private MusicCfg music_cfg;
    
    // component
    InputField cfg_panel_music_name;
    InputField cfg_panel_author;
    Dropdown difficulty;
    InputField cfg_panel_bpm;
    InputField beatPerBar;

    Button save_btn;
    Button close_btn;

    Transform music_select;
    Dropdown music_list;
    Button music_confirm_btn;


    string current_difficulty;

    private void Awake()
    {
        instance = this;
        Transform music_cfg_panel = transform.Find("music_cfg_panel");
        cfg_panel_music_name = music_cfg_panel.Find("music_name").GetComponent<InputField>();
        cfg_panel_author = music_cfg_panel.Find("author").GetComponent<InputField>();
        difficulty = music_cfg_panel.Find("difficulty").GetComponent<Dropdown>();
        cfg_panel_bpm = music_cfg_panel.Find("BPM").GetComponent<InputField>();
        beatPerBar = music_cfg_panel.Find("beatPerBar").GetComponent<InputField>();

        save_btn = transform.Find("save_btn").GetComponent<Button>();
        close_btn = transform.Find("close_btn").GetComponent<Button>();

        music_select = transform.Find("music_select");
        music_list = music_select.Find("music_list").GetComponent<Dropdown>();
        music_confirm_btn = music_select.Find("confirm_btn").GetComponent<Button>();

        foreach (KeyValuePair<int, string> keyValue in GameConst.DifficultyIndex)
            difficulty.options.Add(new Dropdown.OptionData(keyValue.Value));
        foreach (KeyValuePair<int, string> keyValue in MusicResMgr.MusicIndex2Name)
            music_list.options.Add(new Dropdown.OptionData(keyValue.Value));

        cfg_panel_music_name.onValueChanged.AddListener((string value) => {
            music_cfg.music_name = value;
        });
        cfg_panel_author.onValueChanged.AddListener((string value) => {
            music_cfg.author = value;
        });
        difficulty.onValueChanged.AddListener(this.ChangeDifficulty);
        cfg_panel_bpm.onValueChanged.AddListener((string value) =>
        {
            music_cfg.BPM = int.Parse(value);
            HorizontalGridLine.Instance.RefreshGridLine();
        });
        beatPerBar.onValueChanged.AddListener((string value) => {
            HorizontalGridLine.Instance.RefreshGridLine();
        });

        save_btn.onClick.AddListener(this.SaveMusicCfg);
        close_btn.onClick.AddListener(() => {
            Destroy(this.gameObject);
        });

        music_confirm_btn.onClick.AddListener(() => {
            LoadMusic(music_list.value + 1);
            music_select.gameObject.SetActive(false);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        music_select.gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadMusic(int index)
    {
        // º”‘ÿ“Ù∆µ
        AudioWaveForm.Instance.LoadAudio(index);

        // º”‘ÿ≈‰÷√–≈œ¢
        music_cfg = MusicCfg.GetCfgFromEditor(index.ToString());
        cfg_panel_music_name.text = music_cfg.music_name;
        music_cfg.time = AudioWaveForm.Instance.GetAudioLength;
        cfg_panel_bpm.text = music_cfg.BPM.ToString();

        // º”‘ÿ∆◊√Ê
        current_difficulty = GameConst.DifficultyIndex[0];
        List<NoteCfg> composition = music_cfg.GetComposition(current_difficulty);
        CompositionDisplay.Instance.LoadComposition(composition);
    }

    private void ChangeDifficulty(int value)
    {
        if (GameConst.DifficultyIndex[value] != current_difficulty)
        {
            SaveMusicCfg();
            current_difficulty = GameConst.DifficultyIndex[value];
            List<NoteCfg> composition = music_cfg.GetComposition(current_difficulty);
            CompositionDisplay.Instance.LoadComposition(composition);
        }
    }

    private void SaveMusicCfg()
    {
        List<NoteCfg> composition = CompositionDisplay.Instance.GetComposition();
        music_cfg.CompositionSerialize(ref composition, current_difficulty);
        music_cfg.Save();
    }

    public int GetBPM
    {
        get => music_cfg.BPM;
    }

    public int GetBeatPerBar
    {
        get => int.Parse(beatPerBar.text);
    }
}
