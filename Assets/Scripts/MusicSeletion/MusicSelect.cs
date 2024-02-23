using AirFishLab.ScrollingList;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private CircularScrollingList _list;
    private Toggle[] toggles;
    public GameObject game_mgr;
    Button play_btn;
    CanvasGroup record;
    Text record_score_txt;
    Text record_acc_txt;
    Text record_tag;


    public static string SelectedDifficultyKW = "SelectedDifficulty";

    private void Awake()
    {
        instance = this;
        play_btn = transform.Find("play_btn").GetComponent<Button>();
        record = transform.Find("record").GetComponent<CanvasGroup>();
        record_score_txt = transform.Find("record/score").GetComponent<Text>();
        record_acc_txt = transform.Find("record/accuracy").GetComponent<Text>();
        record_tag = transform.Find("record/tag").GetComponent<Text>();

        play_btn.onClick.AddListener(() =>
        {
            int current_id = _list.GetFocusingContentID();
            MusicListContent content =
            (MusicListContent)_list.ListBank.GetListContent(current_id);
            GameMgr.Instance.StartInitGame(content.music_name, PlayerPrefs.GetString(SelectedDifficultyKW, "Easy"));
        });

        toggles = new Toggle[3];
        for (int i=0; i<3; i++)
        {
            toggles[i] = transform.Find($"difficulty_toggle/Toggle ({i+1})").GetComponent<Toggle>();
            Text difficulty_text = toggles[i].transform.Find("Background/Label").GetComponent<Text>();
            if (difficulty_text.text == PlayerPrefs.GetString(SelectedDifficultyKW, "Easy"))
                toggles[i].isOn = true;
            else
                toggles[i].isOn = false;
            toggles[i].onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    PlayerPrefs.SetString(SelectedDifficultyKW, difficulty_text.text);
                    RefreshRecord();
                }
            });
        }
    }

    public void DisplayFocusingContent()
    {
        var contentID = _list.GetFocusingContentID();
        var centeredContent =
            (MusicListContent)_list.ListBank.GetListContent(contentID);
        Debug.Log("DisplayFocusingContent");
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

        string difficulty = PlayerPrefs.GetString(SelectedDifficultyKW, "Easy");
        int max_score = PlayerData.GetMaxScore(content.music_file_name, difficulty);
        float acc = PlayerData.GetMaxAccuracy(content.music_file_name, difficulty);
        string tag = PlayerData.GetTag(content.music_file_name, difficulty);
        record_score_txt.text = max_score.ToString().PadLeft(7, '0');
        record_acc_txt.text = (acc * 100).ToString("N2") + "%";
        record_tag.text = tag;
        record_tag.gameObject.SetActive(tag != "None");
    }
}
