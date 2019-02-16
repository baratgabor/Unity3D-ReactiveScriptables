using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    /// <summary>
    /// Generic base class for concrete classes that store state and provide change notifications.
    /// Concrete GameProperty types has to be derived from this class, because the Unity Editor doesn't support generic classes.
    /// T is the type of the data stored, also the type of the event payload.
    /// </summary>
    public abstract class GameProperty<T> : ScriptableObject, IEventSource<T>
    {
        protected virtual void Set(T t)
            => SetAndNotify(t);

        public T Get()
            => _value;

        public event Action<T> Event;

        [SerializeField]
        protected T _value;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // If the user made changes in the Editor, send the new value to the subscribers, both in Play mode and Edit mode.
            // Important for supporting components that run in edit mode (i.e. marked with the [ExecuteInEditMode] attribute).
            if (GUI.changed)
                Event?.Invoke(_value);
        }
#endif

        protected void SetAndNotify(T newValue)
        {
            //TODO: Look into performance/allocation concerns with different types
            if (EqualityComparer<T>.Default.Equals(newValue, _value))
                return;

            _value = newValue;
            Event?.Invoke(_value);
        }
    }
}