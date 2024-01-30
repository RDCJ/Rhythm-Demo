using AirFishLab.ScrollingList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelect : MonoBehaviour
{
    [SerializeField]
    private CircularScrollingList _list;
    private Dropdown difficulty;
    public GameObject game_mgr;
    Button play_btn;


    private void Awake()
    {
        difficulty = transform.Find("difficulty").GetComponent<Dropdown>();
        play_btn = transform.Find("play_btn").GetComponent<Button>();
        foreach (KeyValuePair<int, string> keyValue in GameConst.DifficultyIndex)
            difficulty.options.Add(new Dropdown.OptionData(keyValue.Value));

        play_btn.onClick.AddListener(() =>
        {
            int current_id = _list.GetFocusingContentID();
            MusicListContent content =
            (MusicListContent)_list.ListBank.GetListContent(current_id);
            GameMgr new_game = Instantiate(game_mgr, transform.parent).GetComponent<GameMgr>();
            new_game.SetMusic(content.music_name, GameConst.DifficultyIndex[difficulty.value]);
        });
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

    }

    public void OnMovementEnd()
    {
        
    }

    public void Exit()
    {
        Destroy(this.gameObject);
    }
}
