using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicResMgr
{
    public static Dictionary<int, string> MusicIndex2Name = new()
    {
        {1, "Noël" },
        {2, "Entrance" },
        {3, "Invention No.1"},
        {4,"Test" }
    };

    public static AudioClip GetMusic(int index)
    {
        return Resources.Load<AudioClip>("Musics/" + MusicIndex2Name[index]);
    }
}
