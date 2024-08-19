using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class StationaryMessage : IGestureMessage
    {
        public int fingerId;
        public Vector2 position;

        public StationaryMessage(int fingerId, Vector2 position)
        {
            this.fingerId = fingerId;
            this.position = position;
        }
    }

    [CreateAssetMenu(fileName = "StationaryGestureRecognizer", menuName = "GestureRecognizer/StationaryGestureRecognizer")]
    public class StationaryGestureRecognizer : GestureRecognizer
    {
        public override void Recognize(TouchesInfo touchesInfo)
        {
            if (!touchesInfo.phaseDic.ContainsKey(TouchPhase.Stationary)) return;
            foreach (var touch in touchesInfo.phaseDic[TouchPhase.Stationary])
            {
                StationaryMessage message = new StationaryMessage(touch.fingerId, touch.position);
                DispatchEvent(message);
            }
        }
    }
}