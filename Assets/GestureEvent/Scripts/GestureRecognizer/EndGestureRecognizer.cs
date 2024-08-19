using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class EndMessage : IGestureMessage
    {
        public int fingerId;
        public Vector2 position;

        public EndMessage(int fingerId, Vector2 position)
        {
            this.fingerId = fingerId;
            this.position = position;
        }
    }

    /// <summary>
    /// Detect a finger releasing
    /// </summary>
    [CreateAssetMenu(fileName = "EndGestureRecognizer", menuName = "GestureRecognizer/EndGestureRecognizer")]
    public class EndGestureRecognizer : GestureRecognizer
    {
        public override void Recognize(TouchesInfo touchesInfo)
        {
            if (!touchesInfo.phaseDic.ContainsKey(TouchPhase.Ended)) return;
            foreach (var touch in touchesInfo.phaseDic[TouchPhase.Ended])
            {
                EndMessage message = new EndMessage(touch.fingerId, touch.position);
                DispatchEvent(message);
            }
        }
    }
}