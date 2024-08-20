using GestureEvent;
using UnityEngine;

namespace NoteGesture
{
    [CreateAssetMenu(fileName = "StationaryGestureRecognizer", menuName = "GestureRecognizer/StationaryGestureRecognizer")]
    public class StationaryGestureRecognizer : GestureRecognizer
    {
        public override void Recognize(TouchesInfo touchesInfo)
        {
            if (!touchesInfo.phaseDic.ContainsKey(TouchPhase.Stationary)) return;
            foreach (var touch in touchesInfo.phaseDic[TouchPhase.Stationary])
            {
                var message = new SimpleGestureMessage(touch.fingerId, touch.position, Util.RaycastFromBottom(touch.position));
                DispatchEvent(message);
            }
        }
    }
}