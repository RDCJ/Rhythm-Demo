using Note;
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

    Dictionary<NoteType, ObjectPool> pools;

    private void Awake()
    {
        instance = this;
        pools = new Dictionary<NoteType, ObjectPool>
        {
            {
                NoteType.Tap,
                new ObjectPool(10, FileConst.tap_prefab_path, transform)
            },

            {
                NoteType.LeftSlide,
                new ObjectPool(5, FileConst.leftslide_prefab_path, transform)
            },
            {
                NoteType.RightSlide,
                new ObjectPool(5, FileConst.rightslide_prefab_path, transform)
            },
            {
                NoteType.Hold,
                new ObjectPool(5, FileConst.hold_prefab_path, transform)
            }
        };
    }

    public GameObject GetObject(NoteType type)
    {
        GameObject obj = pools[type].GetObject();
        obj.transform.SetAsFirstSibling();
        return obj;
    }


    /// <summary>
    /// 回收object
    /// </summary>
    /// <param name="note"></param>
    public void ReturnObject(NoteBase note)
    {
        NoteType type = note.Type;
        note.OnReturnPool();
        pools[type].ReturnObject(note.gameObject);
    }

    /// <summary>
    /// 回收所有object
    /// </summary>
    public void Reload()
    {
        for (int i=0; i<transform.childCount; i++)
        {
            ReturnObject(transform.GetChild(i).GetComponent<NoteBase>());
        }
    }
}
