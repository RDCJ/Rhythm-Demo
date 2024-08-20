using System;
using UnityEngine;

namespace GestureEvent
{
    public class ClickMessage : IGestureMessage
    {
        public int fingerId;
        public float press_time;
        public float release_time;
        public Vector2 press_position;
        public Vector2 release_position;

        public ClickMessage(int fingerId, float press_time, float release_time, Vector2 press_position, Vector2 release_position)
        {
            this.fingerId = fingerId;
            this.press_time = press_time;
            this.release_time = release_time;
            this.press_position = press_position;
            this.release_position = release_position;
        }
    }

    /// <summary>
    /// Detect a finger pressing with instant releasing 
    /// </summary>
    [CreateAssetMenu(fileName = "ClickGestureRecognizer", menuName = "GestureRecognizer/ClickGestureRecognizer")]
    public class ClickGestureRecognizer : GestureRecognizer
    {
        public float TimeLimit = 0.1f;
        public float DistanceLimit = 10;

        private FingerRecord<Tuple<float, Vector2>> fingerRecord = new();

        public override void Recognize(TouchesInfo touchesInfo)
        {
            var phaseDic = touchesInfo.phaseDic;
            fingerRecord.RemoveRecordInMap(phaseDic, TouchPhase.Canceled);
            fingerRecord.AddRecordInMap(phaseDic, TouchPhase.Began, (touch) => new Tuple<float, Vector2>(Time.time, touch.position));
            if (phaseDic.ContainsKey(TouchPhase.Ended))
            {
                foreach (var touch in phaseDic[TouchPhase.Ended])
                {
                    int fingerId = touch.fingerId;
                    if (!fingerRecord.TryGetRecord(fingerId, out var data)) continue;

                    float press_time = data.Item1;
                    float dist = (touch.position - data.Item2).magnitude;
                    if (Time.time - press_time < TimeLimit && dist < DistanceLimit)
                    {
                        DispatchEvent(
                            new ClickMessage(fingerId,
                            press_time, Time.time,
                            data.Item2, touch.position)
                            );
                    }
                    fingerRecord.RemoveRecord(fingerId);
                }
            }
        }
    }
}


