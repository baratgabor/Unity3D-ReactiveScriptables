using System;
using UnityEngine;
using UnityEngine.UI;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(Text))]
    public class EventCountDisplay : SubscriptionHelperMonoBehaviour
    {
        [SerializeField]
        private GameEvent _gameEvent = default;

        private Text _text;

        private int _counter = 0;

        private void Start()
        {
            if (_gameEvent == null)
                throw new Exception("Dependency not set.");

            _text = GetComponent<Text>();
            AddSubscription(_gameEvent, Increment);
        }

        private void Increment(GameEvent _)
            => _text.text = (++_counter).ToString();
    }
}