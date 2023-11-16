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
    Button music_cfg_btn;
    Transform music_cfg_panel;
    Text cfg_panel_music_id;
    InputField cfg_panel_music_name;
    InputField cfg_panel_author;
    Text cfg_panel_time;
    Dropdown difficulty;
    Button save_btn;
    Button close_btn;

    string current_difficulty;

    private void Awake()
    {
        instance = this;
        music_cfg_btn = transform.Find("music_cfg_btn").GetComponent<Button>();
        music_cfg_panel = transform.Find("music_cfg_panel");
        cfg_panel_music_id = music_cfg_panel.Find("music_id").GetComponent<Text>();
        cfg_panel_music_name = music_cfg_panel.Find("music_name").GetComponent<InputField>();
        cfg_panel_author = music_cfg_panel.Find("author").GetComponent<InputField>();
        cfg_panel_time = music_cfg_panel.Find("time").GetComponent<Text>();
        difficulty = music_cfg_panel.Find("difficulty").GetComponent<Dropdown>();
        save_btn = music_cfg_panel.Find("save_btn").GetComponent<Button>();
        close_btn = transform.Find("close_btn").GetComponent<Button>();

        foreach (KeyValuePair<int, string> keyValue in GameConst.DifficultyIndex)
            difficulty.options.Add(new Dropdown.OptionData(keyValue.Value));

        music_cfg_btn.onClick.AddListener(() =>{
            music_cfg_panel.gameObject.SetActive(true);
        });

        cfg_panel_music_name.onValueChanged.AddListener((string value) => {
            music_cfg.music_name = value;
        });
        cfg_panel_author.onValueChanged.AddListener((string value) => {
            music_cfg.author = value;
        });
        difficulty.onValueChanged.AddListener(this.ChangeDifficulty);

        save_btn.onClick.AddListener(this.SaveMusicCfg);
        close_btn.onClick.AddListener(() => {
            Destroy(this.gameObject);
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMusic(1);
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
        cfg_panel_music_id.text = music_cfg.music_id;
        cfg_panel_music_name.text = music_cfg.music_name;
        music_cfg.time = AudioWaveForm.Instance.GetAudioLength;
        cfg_panel_time.text = music_cfg.time.ToString();
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
}
