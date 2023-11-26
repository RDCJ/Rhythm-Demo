using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicResMgr
{
    public static Dictionary<int, string> MusicIndex2Name = new()
    {
        {1, "11.Entrance - Ice" },
        {2, "06.Invention No.1 - J.S Bach"},
        {3, "Noël" }
    };

    public static AudioClip GetMusic(int index)
    {
        return Resources.Load<AudioClip>("Musics/" + MusicIndex2Name[index]);
    }
}
