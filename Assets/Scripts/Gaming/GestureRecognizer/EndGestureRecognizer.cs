using NoteGesture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
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
                var message = new SimpleGestureMessage(touch.fingerId, touch.position, Util.RaycastFromBottom(touch.position));
                DispatchEvent(message);
            }
        }
    }
}