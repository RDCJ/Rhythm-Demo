using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePoolManager : MonoBehaviour
{
    private NotePoolManager() { }
    private static NotePoolManager instance;
    public static NotePoolManager Instance
    {
        get
        {
            return instance;
        }
    }

    Dictionary<Note.NoteType, ObjectPool> pools;

    private void Awake()
    {
        instance = this;
        pools = new Dictionary<Note.NoteType, ObjectPool>
        {
            {
                Note.NoteType.Tap,
                new GameObject("TapNotePool").AddComponent<ObjectPool>().Initialize(10, FileConst.tap_prefab_path, transform)
            },

            {
                Note.NoteType.LeftSlide,
                new GameObject("LeftSlideNotePool").AddComponent<ObjectPool>().Initialize(5, FileConst.leftslide_prefab_path, transform)
            },
            {
                Note.NoteType.RightSlide,
                new GameObject("RightSlideNotePool").AddComponent<ObjectPool>().Initialize(5, FileConst.rightslide_prefab_path, transform)
            },
            {
                Note.NoteType.Hold,
                new GameObject("HoldNotePool").AddComponent<ObjectPool>().Initialize(5, FileConst.hold_prefab_path, transform)
            }
        };
    }

    public GameObject GetObject(Note.NoteType type)
    {
        return pools[type].GetObject();
    }


    /// <summary>
    /// 回收object
    /// </summary>
    /// <param name="note"></param>
    public void ReturnObject(Note.NoteBase note)
    {
        Note.NoteType type = note.Type;
        note.OnReturnPool();
        pools[type].ReturnObject(note.gameObject);
    }

    /// <summary>
    /// 回收所有object
    /// </summary>
    public void Reload()
    {
        foreach (KeyValuePair<Note.NoteType, ObjectPool> pool in pools)
            foreach (var obj in pool.Value.pool)
            {
                ReturnObject(obj.GetComponent<Note.NoteBase>());
            }
    }
}
