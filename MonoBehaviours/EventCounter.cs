using System;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class EventCounter : SubscriptionHelperMonoBehaviour
    {
        [SerializeField]
        private GameEvent _gameEvent = default;

        [SerializeField]
        private IntState_Writeable _countOutput = default;

        private int _score = 0;

        void Start()
        {
            if (_gameEvent == null || _countOutput == null)
                throw new Exception("Depencencies not set.");

            AddSubscription(_gameEvent, Increment);

            _countOutput.Set(0);
        }

        private void Increment(GameEvent sender)
        {
            _score++;
            _countOutput.Set(_score);
        }
    }
}