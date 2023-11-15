using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventListener
{
    void HandleEvent(int event_id, params object[] args);
}
