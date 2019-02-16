using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(Text))]
    public class GameEventCounter : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _gameEvent = default;

        private Text Text
        {
            get => _text ?? (_text = GetComponent<Text>());
            set => _text = value;
        }
        private Text _text;

        private int _counter;

        private void Start()
        {
            if (_gameEvent == null)
                throw new Exception("Dependency not set.");

            Text = GetComponent<Text>();
        }

        private void OnEnable()
            => _gameEvent.Event += IncrementCount;

        private void OnDisable()
            => _gameEvent.Event -= IncrementCount;

        private void IncrementCount(GameEvent sender)
            => Text.text = (++_counter).ToString();
    }
}