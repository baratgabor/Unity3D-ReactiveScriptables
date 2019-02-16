using System;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class GameDistanceTracker : SubscriptionHelperMonoBehaviour
    {
        [SerializeField]
        private FloatProperty _speedInput = default;
        [SerializeField]
        private FloatProperty_Writeable _distanceOutput = default;

        [SerializeField]
        private GameEvent _startGameEvent = default;
        [SerializeField]
        private GameEvent _stopGameEvent = default;

        [SerializeField]
        private GameEvent _pauseGameEvent = default;
        [SerializeField]
        private GameEvent _unpauseGameEvent = default;

        private bool _tracking;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GUI.changed)
                AddSubscriptions();
        }
#endif

        private void Start()
        {
            if (_speedInput == null)
                Debug.LogException(new Exception("Speed input dependency not assigned."));

            if (_distanceOutput == null)
                Debug.LogException(new Exception("Distance output dependency not assigned."));

            _distanceOutput.Set(0);

            AddSubscriptions();
        }

        private void AddSubscriptions()
        {
            AddSubscription(_startGameEvent, EnableTracking);
            AddSubscription(_unpauseGameEvent, EnableTracking);
            AddSubscription(_stopGameEvent, DisableTracking);
            AddSubscription(_pauseGameEvent, DisableTracking);
        }

        private void EnableTracking(GameEvent sender)
            => _tracking = true;

        private void DisableTracking(GameEvent sender)
            => _tracking = false;

        private void Update()
        {
            if (_tracking)
                _distanceOutput.Set(_distanceOutput.Get() + Mathf.Abs(_speedInput.Get() * Time.deltaTime));
        }
    }
}