using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables.Prototyping
{
    public class CompositeGameEvent : GameEvent
    {
        public enum Logic
        {
            All,
            Any
        }

        // Timeout settings? How long does an activation count?

        [SerializeField]
        private Logic _logic = Logic.All;

        [SerializeField]
        private GameEvent[] _events;

        private Dictionary<GameEvent, bool> _eventStates = new Dictionary<GameEvent, bool>();
        private int _eventsNum = 0;
        private int _eventsNum_Activated = 0;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GUI.changed)
                SubscribeAll();
        }
#endif

        private void OnEnable()
        {
            SubscribeAll();
        }

        private void SubscribeAll()
        {
            _eventStates.Clear();

            foreach (var e in _events)
            {
                e.Event += OnSubEventActivate;
                _eventStates.Add(e, false);
            }

            _eventsNum = _eventStates.Count;
        }

        private void OnSubEventActivate(GameEvent eventSource)
        {
            if (_eventStates[eventSource] == false)
            {
                _eventStates[eventSource] = true;
                _eventsNum_Activated++;
            }

            // All fired?
            if (_eventsNum_Activated >= _eventsNum)
                ActivateAggregate();
        }

        private void ActivateAggregate()
        {
            OnEvent(this);
        }
    }



    public class GameEventLogic : GameEvent
    {
        [Serializable]
        public class LogicEntity
        {
            public Logic logic;
            public GameEvent gameEvent;
        }

        public enum Logic
        {
            And,
            Or
        }

        [Header("")]

        [SerializeField]
        LogicEntity[] _logicEntities = default;

        private void Awake()
        {

        }

        private void OnEnable()
        {
            foreach (var e in _logicEntities)
            {
                e.gameEvent.Event += OnTrigger;
            }
        }

        private void OnTrigger(GameEvent sender)
        {

        }

        private void OnDisable()
        {

        }
    }
}