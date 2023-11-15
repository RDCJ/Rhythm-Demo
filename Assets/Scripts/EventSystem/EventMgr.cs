using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMgr : MonoBehaviour, IEventMgr
{
/*    #region Singleton
    private EventMgr() { }
    private static EventMgr instance;
    public static EventMgr Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion*/

    private Dictionary<int, List<IEventListener>> event_dic = new();

    public void AddListener(int event_id, IEventListener listener)
    {
        List<IEventListener> listener_list = null;
        event_dic.TryGetValue(event_id, out listener_list);
        if (listener_list == null)
        {
            listener_list = new List<IEventListener>();
            event_dic[event_id] = listener_list;
        }
        listener_list.Add(listener);
    }

    public void RemoveListener(int event_id, IEventListener listener)
    {
        List<IEventListener> listener_list = null;
        event_dic.TryGetValue(event_id, out listener_list);
        if (listener_list != null && listener_list.Contains(listener))
            listener_list.Remove(listener);
    }

    public void Dispatch(int event_id, params object[] args)
    {
        List<IEventListener> listener_list = null;
        event_dic.TryGetValue(event_id, out listener_list);

        if (listener_list == null) 
            return;

        for (int i=0; i<listener_list.Count; i++)
        {
            if (listener_list[i] != null)
                listener_list[i].HandleEvent(event_id, args);
        }
    }
}
