using System.Collections.Generic;
using UnityEngine;

namespace GestureEvent
{
    public class FingerRecord<DataType>
    {
        public FingerRecord()
        {
            RecordDic = new();
        }

        public Dictionary<int, DataType> RecordDic { get; private set; }

        public DataType GetRecord(int fingerId)
        {
            return RecordDic[fingerId];
        }

        public bool TryGetRecord(int fingerId, out DataType value)
        {
            if (RecordDic.TryGetValue(fingerId, out DataType data))
            {
                value = data;
                return true;
            }
            value = default;
            return false;
        }

        public void AddRecord(int fingerId, DataType data)
        {
            RecordDic[fingerId] = data;
        }

        public void RemoveRecord(int fingerId)
        {
            RecordDic.Remove(fingerId);
        }

        /// <summary>
        /// Record data
        /// </summary>
        /// <param name="phaseDic">All touch in current frame, ordered by TouchPhase</param>
        /// <param name="phase">TouchPhase</param>
        /// <param name="build_data">How to build data</param>
        /// <param name="filter"></param>
        public void AddRecordInMap(Dictionary<TouchPhase, List<Touch>> phaseDic, TouchPhase phase, System.Func<Touch, DataType> build_data, System.Func<Touch, bool> filter = null)
        {
            if (!phaseDic.ContainsKey(phase)) return;
            foreach (var touch in phaseDic[phase])
            {
                if (filter == null || filter(touch))
                {
                    AddRecord(touch.fingerId, build_data(touch));
                }
            }
        }

        public void RemoveRecordInMap(Dictionary<TouchPhase, List<Touch>> phaseDic, TouchPhase phase, System.Func<Touch, bool> filter = null)
        {
            if (!phaseDic.ContainsKey(phase)) return;
            foreach (var touch in phaseDic[phase])
            {
                if (filter == null || filter(touch))
                {
                    RecordDic.Remove(touch.fingerId);
                }
            }
        }
    }
}