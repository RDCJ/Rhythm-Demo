using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicResMgr
{
    public static Dictionary<int, string> MusicIndex2Name = new()
    {
        { 1, "11.Entrance - Ice" }
    };

    public static AudioClip GetMusic(int index)
    {
        return Resources.Load<AudioClip>("Musics/" + MusicIndex2Name[index]);
    }
}
