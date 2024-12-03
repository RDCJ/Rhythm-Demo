using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleFSM
{
    public class StateInfo
    {
        public Action<int> onEnter;
        public Action<int> onExit;
        public Action<float, float> onUpdate;
        public float elapsedTime;

        public StateInfo(Action<int> onEnter, Action<int> onExit, Action<float, float> onUpdate)
        {
            this.onEnter = onEnter;
            this.onExit = onExit;
            this.onUpdate = onUpdate;
        }
    }

    public class TransitionInfo
    {
        public int fromState;
        public int toState;

        public TransitionInfo(int fromState, int toState)
        {
            this.fromState = fromState;
            this.toState = toState;
        }
    }

    public class SimpleFSM : IFSM
    {
        private Dictionary<int, StateInfo> RegisteredStates;
        private Dictionary<int, List<TransitionInfo>> RegisteredTransitions;
        public int LastState { get; private set; }
        public const int StopState = -1;
        public bool IsRunning => CurrentState >= 0;

        public SimpleFSM()
        {
            CurrentState = -1;
            RegisteredStates = new Dictionary<int, StateInfo>();
            RegisteredTransitions = new Dictionary<int, List<TransitionInfo>>();
        }

        public int CurrentState { get; private set; }

        public bool AddState(int state, Action<int> onEnter, Action<int> onExit, Action<float, float> onUpdate)
        {
            if (state < 0)
            {
                Debug.LogError($"[SimpleFSM.AddState] new state should not be negative");
                return false;
            }
            if (RegisteredStates.ContainsKey(state))
            {
                Debug.LogWarning($"[SimpleFSM.AddState] state {state} already exists");
                return false;
            }
            RegisteredStates[state] = new StateInfo(onEnter, onExit, onUpdate);
            return true;
        }

        public bool AddTransition(int fromState, int toState, int triggerCode)
        {
            if (fromState < 0 || toState < 0)
            {
                Debug.LogError($"[SimpleFSM.AddTransition] state should not be negative");
                return false;
            }
            if (RegisteredTransitions.ContainsKey(triggerCode))
            {
                Debug.LogWarning($"[SimpleFSM.AddTransition] triggerCode {triggerCode} already exists");
                return false;
            }
            if (!RegisteredStates.ContainsKey(fromState))
            {
                Debug.LogWarning($"[SimpleFSM.AddTransition] fromState {fromState} not exists");
                return false;
            }
            if (!RegisteredStates.ContainsKey(toState))
            {
                Debug.LogWarning($"[SimpleFSM.AddTransition] toState {toState} not exists");
                return false;
            }
            if (fromState == toState)
            {
                Debug.LogWarning($"[SimpleFSM.AddTransition] fromState is equal to toState {toState}. Please ensure it is what you want.");
            }
            if (!RegisteredTransitions.ContainsKey(triggerCode))
            {
                RegisteredTransitions[triggerCode] = new List<TransitionInfo>();
            }
            RegisteredTransitions[triggerCode].Add(new TransitionInfo(fromState, toState));
            return true;
        }

        public bool RemoveState(int state)
        {
            if (RegisteredStates.ContainsKey(state))
            {
                RegisteredStates.Remove(state);
                return true;
            }
            Debug.LogWarning($"[SimpleFSM.RemoveState] state {state} not exists");
            return false;
        }

        public bool TriggerTransition(int triggerCode)
        {
            if (!IsRunning)
            {
                Debug.LogWarning("[SimpleFSM.TriggerTransition] FSM is not running");
                return false;
            }
            if (!RegisteredTransitions.TryGetValue(triggerCode, out var transitionInfos))
            {
                Debug.LogWarning($"[SimpleFSM.TriggerTransition] triggerCode {triggerCode} not exists");
                return false;
            }
            bool success = false;
            List<TransitionInfo> invalid_transitions = new List<TransitionInfo>();
            foreach (var transitionInfo in transitionInfos)
            {
                if (!RegisteredStates.TryGetValue(transitionInfo.fromState, out var fromStateInfo))
                {
                    Debug.LogWarning($"[SimpleFSM.TriggerTransition] fromState {transitionInfo.fromState} not exists");
                    invalid_transitions.Add(transitionInfo);
                    continue;
                }
                if (transitionInfo.fromState != CurrentState) continue;
                if (!RegisteredStates.TryGetValue(transitionInfo.toState, out var toStateInfo))
                {
                    Debug.LogWarning($"[SimpleFSM.TriggerTransition] toState {transitionInfo.toState} not exists");
                    invalid_transitions.Add(transitionInfo);
                    break;
                }

                LastState = CurrentState;
                fromStateInfo.onExit?.Invoke(transitionInfo.toState);
                CurrentState = transitionInfo.toState;
                toStateInfo.onEnter?.Invoke(LastState);

                toStateInfo.elapsedTime = 0;
                success = true;
                break;
            }
            if (invalid_transitions.Count > 0)
            {
                foreach (var transitionInfo in invalid_transitions)
                    transitionInfos.Remove(transitionInfo);
            }
            return success;
        }


        public bool TriggerAnyTransition(int toState)
        {
            if (!IsRunning)
            {
                Debug.LogWarning("[SimpleFSM.TriggerAnyTransition] FSM is not running");
                return false;
            }
            if (!RegisteredStates.TryGetValue(CurrentState, out var fromStateInfo))
            {
                Debug.LogWarning($"[SimpleFSM.TriggerAnyTransition] CurrentState {CurrentState} not exists");
                return false;
            }
            if (!RegisteredStates.TryGetValue(toState, out var toStateInfo))
            {
                Debug.LogWarning($"[SimpleFSM.TriggerTransition] toState {toState} not exists");
                return false;
            }
            LastState = CurrentState;
            fromStateInfo.onExit?.Invoke(toState);
            CurrentState = toState;
            toStateInfo.onEnter?.Invoke(LastState);
            
            toStateInfo.elapsedTime = 0;
            return true;
        }

        public bool Start(int startState)
        {
            if (IsRunning)
            {
                Debug.LogWarning("[SimpleFSM.Start] FSM has already started");
                return false;
            }
            if (RegisteredStates.TryGetValue(startState, out var stateInfo))
            {
                LastState = CurrentState;
                stateInfo.onEnter?.Invoke(CurrentState);
                CurrentState = startState;
                return true;
            }
            Debug.LogWarning($"[SimpleFSM.Start] startState {startState} not exists");
            return false;
        }

        public void Update(float time)
        {
            if (IsRunning && RegisteredStates.TryGetValue(CurrentState, out var stateInfo))
            {
                stateInfo.elapsedTime += time;
                stateInfo.onUpdate?.Invoke(time, stateInfo.elapsedTime);
            }
        }

        public void Stop()
        {
            if (IsRunning && RegisteredStates.TryGetValue(CurrentState, out var stateInfo))
            {
                stateInfo.onExit?.Invoke(StopState);
            }
            LastState = CurrentState;
            CurrentState = StopState;
        }
    }
}