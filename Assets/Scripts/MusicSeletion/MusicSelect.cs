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

    private void Awake()
    {
        difficulty = transform.Find("difficulty").GetComponent<Dropdown>();
        foreach (KeyValuePair<int, string> keyValue in GameConst.DifficultyIndex)
            difficulty.options.Add(new Dropdown.OptionData(keyValue.Value));
    }

    public void DisplayFocusingContent()
    {
        var contentID = _list.GetFocusingContentID();
        var centeredContent =
            (MusicListContent)_list.ListBank.GetListContent(contentID);
    }

    public void OnBoxSelected(ListBox MusicListBox)
    {
        Debug.Log("current_box_id: " + _list.GetFocusingContentID());
        if (_list.GetFocusingContentID() == MusicListBox.ContentID)
        {
            MusicListContent content =
            (MusicListContent)_list.ListBank.GetListContent(MusicListBox.ContentID);
            Debug.Log("OnBoxSelected: " + MusicListBox.ContentID);
            GameMgr new_game = Instantiate(game_mgr, transform.parent).GetComponent<GameMgr>();
            new_game.SetMusic(content.music_id.ToString(), GameConst.DifficultyIndex[difficulty.value]);
        }
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
