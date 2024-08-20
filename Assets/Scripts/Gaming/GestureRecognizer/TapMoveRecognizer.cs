using GestureEvent;
using System;
using UnityEngine;

namespace NoteGesture
{
    public class TapMoveMessage : IGestureMessage
    {
        public int fingerId;
        public float press_time;
        public float move_time;
        public Vector2 press_position;
        public Vector2 delta_position;
        public RaycastHit2D hit;

        public TapMoveMessage(int fingerId, float press_time, float move_time, Vector2 press_position, Vector2 delta_position, RaycastHit2D hit)
        {
            this.fingerId = fingerId;
            this.press_time = press_time;
            this.move_time = move_time;
            this.press_position = press_position;
            this.delta_position = delta_position;
            this.hit = hit;
        }
    }

    /// <summary>
    /// Detect one finger pressing and then moving
    /// </summary>
    [CreateAssetMenu(fileName = "TapMoveRecognizer", menuName = "GestureRecognizer/TapMoveRecognizer")]
    public class TapMoveRecognizer : GestureRecognizer
    {
        private FingerRecord<Tuple<float, Vector2>> fingerRecord = new();

        public override void Recognize(TouchesInfo touchesInfo)
        {
            var phaseDic = touchesInfo.phaseDic;
            fingerRecord.RemoveRecordInMap(phaseDic, TouchPhase.Ended);
            fingerRecord.RemoveRecordInMap(phaseDic, TouchPhase.Canceled);
            fingerRecord.AddRecordInMap(phaseDic, TouchPhase.Began, (touch) => new Tuple<float, Vector2>(Time.time, touch.position));

            if (phaseDic.ContainsKey(TouchPhase.Moved))
            {
                foreach (var touch in phaseDic[TouchPhase.Moved])
                {
                    int fingerId = touch.fingerId;
                    if (!fingerRecord.TryGetRecord(fingerId, out var data)) continue;
                    var hit = Util.RaycastFromBottom(data.Item2);
                    DispatchEvent(
                        new TapMoveMessage(fingerId,
                        data.Item1, Time.time,
                        data.Item2, touch.deltaPosition, hit)
                       );
                    fingerRecord.RemoveRecord(fingerId);
                }
            }
        }
    }
}

