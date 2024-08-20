using GestureEvent;
using UnityEngine;

namespace NoteGesture
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "BeganGestureRecognizer", menuName = "GestureRecognizer/BeganGestureRecognizer")]
    public class BeganGestureRecognizer : GestureRecognizer
    {
        public override void Recognize(TouchesInfo touchesInfo)
        {
            if (!touchesInfo.phaseDic.ContainsKey(TouchPhase.Began)) return;
            foreach (var touch in touchesInfo.phaseDic[TouchPhase.Began])
            {
                var message = new SimpleGestureMessage(touch.fingerId, touch.position, Util.RaycastFromBottom(touch.position));
                DispatchEvent(message);
            }
        }
    }
}

