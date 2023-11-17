using AirFishLab.ScrollingList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelect : MonoBehaviour
{
    [SerializeField]
    private CircularScrollingList _list;

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
            var content =
            (MusicListContent)_list.ListBank.GetListContent(MusicListBox.ContentID);
            Debug.Log("OnBoxSelected: " + MusicListBox.ContentID);
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
