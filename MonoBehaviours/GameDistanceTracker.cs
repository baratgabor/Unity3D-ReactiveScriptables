using System;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class GameDistanceTracker : MonoBehaviour
    {
        [SerializeField]
        private FloatState _gameScrollingSpeed = default;
        [SerializeField]
        private FloatState_Writeable _gameDistanceOut = default;

        [SerializeField]
        private GameEvent _startGameEvent = default;
        [SerializeField]
        private GameEvent _stopGameEvent = default;

        [SerializeField]
        private GameEvent _pauseGameEvent = default;
        [SerializeField]
        private GameEvent _unpauseGameEvent = default;

        private bool _tracking;

        private void Start()
        {
            _gameDistanceOut.Set(0);

            if (AnyNull(_gameScrollingSpeed, _gameDistanceOut, _startGameEvent, _stopGameEvent))
                throw new Exception("Component failed: Dependencies not set.");
        }

        private bool AnyNull(params object[] values)
        {
            foreach (var v in values)
                if (v == null)
                    return true;

            return false;
        }

        private void OnEnable()
        {
            _startGameEvent.Event += EnableTracking;
            _unpauseGameEvent.Event += EnableTracking;

            _stopGameEvent.Event += DisableTracking;
            _pauseGameEvent.Event += DisableTracking;
        }

        private void OnDisable()
        {
            _startGameEvent.Event -= EnableTracking;
            _unpauseGameEvent.Event -= EnableTracking;

            _stopGameEvent.Event -= DisableTracking;
            _pauseGameEvent.Event -= DisableTracking;
        }

        private void EnableTracking(GameEvent sender)
            => _tracking = true;

        private void DisableTracking(GameEvent sender)
            => _tracking = false;

        private void Update()
        {
            if (_tracking)
                _gameDistanceOut.Set(_gameDistanceOut.Get() + Mathf.Abs(_gameScrollingSpeed.Get() * Time.deltaTime));
        }
    }
}