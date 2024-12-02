using System;

namespace SimpleFSM
{
    public interface IFSM
    {
        public int CurrentState { get; }
        public bool AddState(int state, Action<int> onEnter, Action<int> onExit, Action<float, float> onUpdate);
        public bool RemoveState(int state);
        public bool AddTransition(int fromState, int toState, int triggerCode);
        public bool TriggerTransition(int triggerCode);
        public bool TriggerAnyTransition(int toState);
        public bool Start(int startState);
        public void Update(float time);
        public void Stop();
    }
}