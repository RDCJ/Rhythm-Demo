using System;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class TouchesInfo
    {
        public int touchCount = 0;
        public Dictionary<TouchPhase, List<Touch>> phaseDic = new();
    }

    public class GestureMgr : MonoBehaviour
    {
        public List<GestureRecognizer> gestureRecognizers;
        private TouchesInfo touchesInfo = new();
        public int touchCount => touchesInfo == null ? 0 : touchesInfo.touchCount;
        private void Update()
        {
            touchesInfo.touchCount = Input.touchCount;
            touchesInfo.phaseDic.Clear();
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (!touchesInfo.phaseDic.ContainsKey(touch.phase)) touchesInfo.phaseDic[touch.phase] = new List<Touch>(2);
                    touchesInfo.phaseDic[touch.phase].Add(touch);
                }
                foreach (var gestureRecognizer in gestureRecognizers)
                {
                    gestureRecognizer.Recognize(touchesInfo);
                }
            }
        }

        public void AddListener<T>(Action<IGestureMessage> action) where T : GestureRecognizer
        {
            var recognizer = gestureRecognizers.Find((x) => x is T);
            if (recognizer == null) return;
            recognizer.AddListener(action);
        }
        public void RemoveListener<T>(Action<IGestureMessage> action) where T : GestureRecognizer
        {
            var recognizer = gestureRecognizers.Find((x) => x is T);
            if (recognizer == null) return;
            recognizer.RemoveListener(action);
        }

        private void OnDisable()
        {
            touchesInfo.touchCount = 0;
            touchesInfo.phaseDic.Clear();
        }
    }
}

