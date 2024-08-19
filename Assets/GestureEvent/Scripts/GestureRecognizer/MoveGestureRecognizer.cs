using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class MoveMessage:IGestureMessage
    {
        public int fingerId;
        public Vector2 position;
        public Vector2 delta_position;

        public MoveMessage(int fingerId, Vector2 position, Vector2 delta_position)
        {
            this.fingerId = fingerId;
            this.position = position;
            this.delta_position = delta_position;
        }
    }

    [CreateAssetMenu(fileName = "MoveGestureRecognizer", menuName = "GestureRecognizer/MoveGestureRecognizer")]
    public class MoveGestureRecognizer: GestureRecognizer
    {
        public override void Recognize(TouchesInfo touchesInfo)
        {
            if (!touchesInfo.phaseDic.ContainsKey(TouchPhase.Moved)) return;
            foreach (var touch in touchesInfo.phaseDic[TouchPhase.Moved])
            {
                DispatchEvent(new MoveMessage(touch.fingerId, touch.position, touch.deltaPosition));
            }
        }
    }
}