public interface IEventMgr
{
    void AddListener(int event_id, IEventListener listener);
    void RemoveListener(int event_id, IEventListener listener);
    void Dispatch(int event_id, params object[] args);
}
