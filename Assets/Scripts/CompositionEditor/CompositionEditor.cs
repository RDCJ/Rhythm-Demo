using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Music;
using UnityEngine.UI;
using System;

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

    // 配置
    private MusicCfg music_cfg;
    
    // component
    InputField cfg_panel_music_name;
    InputField cfg_panel_author;
    Dropdown difficulty;
    InputField cfg_panel_bpm;
    InputField beatPerBar;
    InputField cfg_prepare_time;
    InputField cfg_time_offset;
    Dropdown grid_density;

    Button save_btn;
    Button close_btn;

    Transform music_select;
    Dropdown music_list;
    Button music_confirm_btn;

    Slider vertical_scale;

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
        cfg_prepare_time = music_cfg_panel.Find("prepare_time").GetComponent<InputField>();
        cfg_time_offset = music_cfg_panel.Find("time_offset").GetComponent <InputField>();
        grid_density = music_cfg_panel.Find("grid_density").GetComponent<Dropdown>();

        save_btn = transform.Find("save_btn").GetComponent<Button>();
        close_btn = transform.Find("close_btn").GetComponent<Button>();

        music_select = transform.Find("music_select");
        music_list = music_select.Find("music_list").GetComponent<Dropdown>();
        music_confirm_btn = music_select.Find("confirm_btn").GetComponent<Button>();

        vertical_scale = transform.Find("display/vertical_scale").GetComponent<Slider>();

        // 难度选项
        foreach (KeyValuePair<int, string> keyValue in GameConst.DifficultyIndex)
            difficulty.options.Add(new Dropdown.OptionData(keyValue.Value));
        // 音乐选项
        foreach (var music_file_name in MusicResMgr.music_list.Keys)
            music_list.options.Add(new Dropdown.OptionData(music_file_name));

        cfg_panel_music_name.onValueChanged.AddListener((string value) => {
            music_cfg.music_name = value;
        });
        cfg_panel_author.onValueChanged.AddListener((string value) => {
            music_cfg.author = value;
        });
        difficulty.onValueChanged.AddListener(this.ChangeDifficulty);
        cfg_panel_bpm.onValueChanged.AddListener((string value) =>
        {
            try
            {
                music_cfg.BPM = int.Parse(value);
            }
            catch (FormatException)
            {
                music_cfg.BPM = 1;
            }
            HorizontalGridLine.Instance.RefreshGridLine();
        });
        beatPerBar.onValueChanged.AddListener((string value) => {
            HorizontalGridLine.Instance.RefreshGridLine();
        });

        grid_density.onValueChanged.AddListener((int value) => {
            HorizontalGridLine.Instance.RefreshGridLine();
        });
        cfg_prepare_time.onValueChanged.AddListener((string value) => {
            try
            {
                music_cfg.prepare_time = double.Parse(value);
            }
            catch (FormatException)
            {
                music_cfg.prepare_time = 0;
            }
        });
        cfg_time_offset.onValueChanged.AddListener((string value) =>{
            try
            {
                music_cfg.time_offset = double.Parse(value);
            }
            catch (FormatException) {
                music_cfg.time_offset = 0;
            }
            CompositionDisplay.Instance.RepaintAllNote();
        });

        save_btn.onClick.AddListener(this.SaveMusicCfg);
        close_btn.onClick.AddListener(() => {
            Destroy(this.gameObject);
        });

        music_confirm_btn.onClick.AddListener(() => {
            int idx = music_list.value;
            LoadMusic(music_list.options[idx].text);
        });

        vertical_scale.onValueChanged.AddListener((float value) => {
            HorizontalGridLine.Instance.RefreshHeight();
            CompositionDisplay.Instance.RepaintAllNote();
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

    private void LoadMusic(string music_file_name)
    {
        music_confirm_btn.interactable = false;
        // 加载音频
        AudioWaveForm.Instance.LoadAudio(music_file_name, () =>
        {
            // 加载配置信息
            music_cfg = MusicResMgr.GetCfg(music_file_name);
            cfg_panel_music_name.text = music_cfg.music_name;
            music_cfg.time = AudioWaveForm.Instance.GetAudioLength;
            cfg_panel_bpm.text = music_cfg.BPM.ToString();
            cfg_prepare_time.text = music_cfg.prepare_time.ToString();
            cfg_time_offset.text = music_cfg.time_offset.ToString();
            // 加载谱面
            current_difficulty = GameConst.DifficultyIndex[0];
            List<NoteCfg> composition = music_cfg.GetComposition(current_difficulty);
            CompositionDisplay.Instance.LoadComposition(composition);

            music_confirm_btn.interactable = true;
            music_select.gameObject.SetActive(false);
        });
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

    #region Get data
    public int GetBPM
    {
        get => music_cfg.BPM;
    }

    public float GetPrepareTime
    {
        get => (float)music_cfg.prepare_time;
    }
    public float GetTimeOffset
    {
        get => (float)music_cfg.time_offset;
    }

    public int GetBeatPerBar
    {
        get => int.Parse(beatPerBar.text);
    }

    public int GetGridDensity
    {
        get => grid_density.value;
    }

    public float GetVerticalScale
    {
        get => vertical_scale.value * 2.5f + 0.5f;
    }
    #endregion
}
