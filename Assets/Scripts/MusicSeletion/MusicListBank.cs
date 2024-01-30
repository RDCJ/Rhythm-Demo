using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using System;
using UnityEngine;
using Music;
using System.Collections.Generic;

// The bank for providing the content for the box to display
// Must be inherit from the class BaseListBank
public class MusicListBank : BaseListBank
{
    // The content to be passed to the list box
    // must inherit from the class `IListContent`.

    private List<MusicListContent> _contents;
    // This function will be invoked by the `CircularScrollingList`
    // to get the content to display.
    public override IListContent GetListContent(int index)
    {
        return _contents[index];
    }

    public override int GetContentCount()
    {
        return _contents.Count;
    }

    private void Awake()
    {
        _contents = new List<MusicListContent>();
        foreach (var k in MusicResMgr.music_list.Keys)
        {
            MusicCfg cfg = MusicResMgr.GetCfg(k);
            _contents.Add(new MusicListContent(cfg));
        }
    }
}

[Serializable]
public class MusicListContent : IListContent
{
    public string music_name;
    public string music_author;

    public MusicListContent(MusicCfg cfg)
    {
        music_author = cfg.author;
        music_name = cfg.music_name;
    }
}