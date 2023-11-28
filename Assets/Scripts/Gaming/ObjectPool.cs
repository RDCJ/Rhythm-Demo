using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    Transform transform;
    GameObject prefab;
    Stack<GameObject> pool;

    public delegate void DoAfterAddNew(ref GameObject obj);
    DoAfterAddNew addNewCallBack;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pool_size">��ʼ�Ķ�������</param>
    /// <param name="prefab_path"></param>
    /// <param name="transform">��������</param>
    /// <param name="doAfterAddNew">�Զ���</param>
    public ObjectPool(int pool_size, string prefab_path, Transform transform, DoAfterAddNew doAfterAddNew=null)
    {
        pool = new Stack<GameObject>();
        prefab = Resources.Load<GameObject>(prefab_path);
        this.transform = transform;
        addNewCallBack = doAfterAddNew;

        for (int i = 0; i < pool_size; ++i)
            AddNewObj();
    }

    /// <summary>
    /// ����activeInHierarchy����һ��δʹ�õ�object���������򴴽�һ���µ�
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            AddNewObj();
        }
        GameObject obj = pool.Pop();
        obj.SetActive(true);
        return obj;
    }

    protected virtual GameObject AddNewObj()
    {
        GameObject obj = GameObject.Instantiate(prefab, this.transform);
        addNewCallBack?.Invoke(ref obj);
        obj.SetActive(false);
        pool.Push(obj);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Push(obj);
    }

    /// <summary>
    /// ����pool������object
    /// </summary>
    public virtual void Reload()
    {
/*        for (int i = 0; i < pool_size; ++i)
            pool[i].SetActive(false);*/
    }
}
