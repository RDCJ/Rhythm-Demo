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

    

    public void ReturnObject(GameObject obj)
    {
        Note.NoteType type = obj.GetComponent<Note.NoteBase>().Type;
        obj.GetComponent<Note.NoteBase>().ReturnPool();
        pools[type].ReturnObject(obj);
    }

    public void Reload()
    {
        foreach (KeyValuePair<Note.NoteType, ObjectPool> pool in pools)
            pool.Value.Reload();
    }
}
