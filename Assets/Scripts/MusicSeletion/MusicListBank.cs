using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using System;
using UnityEngine;

// The bank for providing the content for the box to display
// Must be inherit from the class BaseListBank
public class MusicListBank : BaseListBank
{
    // The content to be passed to the list box
    // must inherit from the class `IListContent`.

    [SerializeField]
    private MusicListContent[] _contents;
    // This function will be invoked by the `CircularScrollingList`
    // to get the content to display.
    public override IListContent GetListContent(int index)
    {
        return _contents[index];
    }

    public override int GetContentCount()
    {
        return _contents.Length;
    }
}

[Serializable]
public class MusicListContent : IListContent
{
    public int music_id;
    public string music_name;
    public string music_author;
}