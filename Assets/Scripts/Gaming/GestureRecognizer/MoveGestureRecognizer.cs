using NoteGesture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class MoveMessage:SimpleGestureMessage
    {
        public Vector2 delta_position;

        public MoveMessage(int fingerId, Vector2 position, RaycastHit2D hit, Vector2 delta_position): base(fingerId, position, hit)
        {
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
                var hit = Util.RaycastFromBottom(touch.position);
                DispatchEvent(new MoveMessage(touch.fingerId, touch.position, hit, touch.deltaPosition));
            }
        }
    }
}