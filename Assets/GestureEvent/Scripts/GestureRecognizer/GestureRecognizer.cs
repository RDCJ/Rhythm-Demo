using System;
using UnityEngine;

namespace GestureEvent
{
    /// <summary>
    /// Recognize gesture and dispatch event
    /// </summary>
    public abstract class GestureRecognizer : ScriptableObject
    {
        public bool PrintLog = false;
        private Action<IGestureMessage> actions = null;
        /// <summary>
        /// Override this to custom gesture
        /// </summary>
        /// <param name="phaseDic"></param>
        public virtual void Recognize(TouchesInfo touchesInfo) { }
        public void AddListener(Action<IGestureMessage> action)
        {
            actions += action;
        }
        public void RemoveListener(Action<IGestureMessage> action)
        {
            actions -= action;
        }
        public void RemoveAllListener()
        {
            actions = null;
        }
        public void DispatchEvent(IGestureMessage message)
        {
            if (PrintLog ) Debug.Log($"Gesture event: {GetType()}, time: {Time.time}");
            actions?.Invoke(message);
        }
    }
}