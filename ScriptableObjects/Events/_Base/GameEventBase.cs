using System;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public abstract class GameEvent<T> : ScriptableObject, IEventSource<T>
    {
        public event Action<T> Event;

        [SerializeField]
        [Tooltip("If set to true, will issue a warning if invocation is requested while there was no subscriber.")]
        private bool _mustHaveSubscriber = false;

        protected virtual void OnEvent(T eventData)
        {
            if (Event != null)
                Event.Invoke(eventData);
            else if (_mustHaveSubscriber)
                Debug.LogWarning("GameEvent flagged as must have subscriber did not have a subscriber.");
        }
    }
}