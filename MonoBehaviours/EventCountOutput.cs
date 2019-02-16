using System;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class EventCountOutput : SubscriptionHelperMonoBehaviour
    {
        [SerializeField]
        private GameEvent _gameEvent = default;

        [SerializeField]
        private IntProperty_Writeable _countOutput = default;

        private int _counter = 0;

        void Start()
        {
            if (_gameEvent == null || _countOutput == null)
                throw new Exception("Depencencies not set.");

            _countOutput.Set(0);
            AddSubscription(_gameEvent, Increment);
        }

        private void Increment(GameEvent _)
           => _countOutput.Set(++_counter);
    }
}