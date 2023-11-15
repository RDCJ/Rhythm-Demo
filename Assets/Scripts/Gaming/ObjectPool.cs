using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int pool_size;
    public List<GameObject> pool;

    public delegate void DoAfterAddNew(ref GameObject obj);
    DoAfterAddNew addNewCallBack;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        pool = new List<GameObject>();
    }

    public virtual ObjectPool Initialize(int psize, string prefab_path, Transform parent=null, DoAfterAddNew doAfterAddNew=null)
    {
        pool_size = psize;
        prefab = Resources.Load<GameObject>(prefab_path);
        if (parent != null) this.transform.parent = parent;
        addNewCallBack = doAfterAddNew;

        for (int i = 0; i < pool_size; ++i)
            AddNewObj();
        return this;
    }

    /// <summary>
    /// 根据activeInHierarchy返回一个未使用的object，不存在则创建一个新的
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newObj = AddNewObj();
        pool_size++;
        newObj.SetActive(true);
        return newObj;
    }

    protected virtual GameObject AddNewObj()
    {
        GameObject obj = Instantiate(prefab, transform);
        addNewCallBack?.Invoke(ref obj);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary>
    /// 回收pool中所有object
    /// </summary>
    public virtual void Reload()
    {
        for (int i = 0; i < pool_size; ++i)
            pool[i].SetActive(false);
    }
}
