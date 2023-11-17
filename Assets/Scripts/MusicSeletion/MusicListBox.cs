using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AirFishLab.ScrollingList.ContentManagement;
using UnityEngine.UI;
using AirFishLab.ScrollingList;

public class MusicListBox : ListBox
{
    [SerializeField]
    private Text _music_name_text;

    private MusicListContent content_data;
    // This function is invoked by the `CircularScrollingList` for updating the list content
    protected override void UpdateDisplayContent(IListContent listContent)
    {
        content_data = (MusicListContent)listContent;
        _music_name_text.text = content_data.music_name;
    }
}
