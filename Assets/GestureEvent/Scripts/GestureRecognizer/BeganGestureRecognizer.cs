using UnityEngine;

namespace GestureEvent
{
    public class BeganMessage : IGestureMessage
    {
        public int fingerId;
        public float time;
        public Vector2 position;

        public BeganMessage(int fingerId, float time, Vector2 position)
        {
            this.fingerId = fingerId;
            this.time = time;
            this.position = position;
        }
    }

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
                BeganMessage message = new BeganMessage(touch.fingerId, Time.time, touch.position);
                DispatchEvent(message);
            }
        }
    }
}

