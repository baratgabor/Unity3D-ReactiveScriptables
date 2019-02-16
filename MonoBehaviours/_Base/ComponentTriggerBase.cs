using UnityEngine;
using System;

namespace LeakyAbstraction.ReactiveScriptables
{
    public abstract class ComponentTriggerBase<T> : MonoBehaviour
where T : Component
    {
        [SerializeField]
        private GameEvent _event = default;

        [SerializeField]
        private GameSound _soundToPlay = GameSound.None;

        protected T _component;

        private void Awake()
        {
            _component = GetComponent<T>();

            if (_component == null)
                throw new Exception($"Cannot get component of type '{nameof(T)}'.");

            if (_event == null)
                Debug.LogWarning("No GameEvent set. This event trigger will never trigger.");
        }

        private void OnEnable()
        {
            if (_event != null)
                _event.Event += OnTrigger;
        }

        private void OnDisable()
        {
            if (_event != null)
                _event.Event -= OnTrigger;
        }

        private void OnTrigger(GameEvent sender)
        {
            if (_soundToPlay != GameSound.None)
                SoundManager.Instance.PlaySound(_soundToPlay);

            DoTrigger();
        }

        // Implement in concrete classes to execute expected trigger behavior
        protected abstract void DoTrigger();
    }
}