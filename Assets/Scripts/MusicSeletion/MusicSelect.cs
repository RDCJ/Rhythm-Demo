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
    Button difficulty_change_btn;
    Text difficulty_txt;
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
        difficulty_change_btn = transform.Find("difficulty_change").GetComponent<Button>();
        difficulty_txt = difficulty_change_btn.transform.Find("Text").GetComponent<Text>();
        record = transform.Find("record/BG").GetComponent<CanvasGroup>();
        record_score_txt = record.transform.Find("score").GetComponent<Text>();
        record_acc_txt = record.transform.Find("accuracy").GetComponent<Text>();
        record_tag = record.transform.Find("tag").GetComponent<Text>();

        play_btn.onClick.AddListener(() =>
        {
            int current_id = _list.GetFocusingContentID();
            MusicListContent content =
            (MusicListContent)_list.ListBank.GetListContent(current_id);
            GameMgr.Instance.StartInitGame(content.music_name, GetSelectedDifficulty);
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
