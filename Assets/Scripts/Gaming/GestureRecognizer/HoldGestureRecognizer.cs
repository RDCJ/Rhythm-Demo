using System;
using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class HoldMessage : IGestureMessage
    {
        public int fingerId;
        public float press_time;
        public Vector2 press_position;
        public Vector2 current_position;

        public HoldMessage(int fingerId, float press_time, Vector2 press_position, Vector2 current_position)
        {
            this.fingerId = fingerId;
            this.press_time = press_time;
            this.press_position = press_position;
            this.current_position = current_position;
        }
    }

    /// <summary>
    /// Detect one finger pressing consistently on the screen
    /// </summary>
    [CreateAssetMenu(fileName = "HoldGestureRecognizer", menuName = "GestureRecognizer/HoldGestureRecognizer")]
    public class HoldGestureRecognizer : GestureRecognizer
    {
        /// <summary>
        /// The event is triggered only after pressing for more than this time
        /// </summary>
        public float HoldTimeThreshold;
        /// <summary>
        /// Limit the moving of the finger
        /// </summary>
        public float DistanceLimit = 10;

        private FingerRecord<Tuple<float, Vector2>> fingerRecord = new();

        public override void Recognize(TouchesInfo touchesInfo)
        {
            var phaseDic = touchesInfo.phaseDic;
            fingerRecord.RemoveRecordInMap(phaseDic, TouchPhase.Canceled);
            fingerRecord.RemoveRecordInMap(phaseDic, TouchPhase.Ended);
            fingerRecord.AddRecordInMap(phaseDic, TouchPhase.Began, (touch) => new Tuple<float, Vector2>(Time.time, touch.position));
            foreach (var phase in new List<TouchPhase>() { TouchPhase.Stationary, TouchPhase.Moved })
            {
                if (!phaseDic.ContainsKey(phase)) continue;

                foreach (var touch in phaseDic[phase])
                {
                    int fingerId = touch.fingerId;
                    if (!fingerRecord.TryGetRecord(fingerId, out var data)) continue;
                    float dist = (touch.position - data.Item2).magnitude;
                    if (dist > DistanceLimit)
                    {
                        fingerRecord.RemoveRecord(fingerId);
                        continue;
                    }
                    float press_time = data.Item1;
                    if (Time.time - press_time < HoldTimeThreshold) continue;
                    DispatchEvent(new HoldMessage(fingerId, press_time, data.Item2, touch.position));
                }
            }
        }
    }
}


