using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    Transform transform;
    GameObject prefab;
    HashSet<GameObject> pool;

    public delegate void DoAfterAddNew(ref GameObject obj);
    DoAfterAddNew addNewCallBack;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pool_size">初始的对象数量</param>
    /// <param name="prefab_path"></param>
    /// <param name="transform">对象父物体</param>
    /// <param name="doAfterAddNew">自定义</param>
    public ObjectPool(int pool_size, string prefab_path, Transform transform, DoAfterAddNew doAfterAddNew=null)
    {
        pool = new HashSet<GameObject>();
        prefab = Resources.Load<GameObject>(prefab_path);
        this.transform = transform;
        addNewCallBack = doAfterAddNew;

        for (int i = 0; i < pool_size; ++i)
            AddNewObj();
    }

    /// <summary>
    /// 根据activeInHierarchy返回一个未使用的object，不存在则创建一个新的
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            AddNewObj();
        }

        var iterator = pool.GetEnumerator();
        iterator.MoveNext();
        GameObject obj = iterator.Current;
        pool.Remove(obj);
        obj.SetActive(true);
        return obj;
    }

    protected virtual GameObject AddNewObj()
    {
        GameObject obj = GameObject.Instantiate(prefab, this.transform);
        addNewCallBack?.Invoke(ref obj);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (pool.Contains(obj))
            return;
        obj.SetActive(false);
        pool.Add(obj);
    }

    /// <summary>
    /// 回收pool中所有object
    /// </summary>
    public virtual void Reload()
    {
/*        for (int i = 0; i < pool_size; ++i)
            pool[i].SetActive(false);*/
    }
}
