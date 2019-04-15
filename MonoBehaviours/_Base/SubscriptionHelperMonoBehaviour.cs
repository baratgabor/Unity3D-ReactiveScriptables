using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    /// <summary>
    /// MonoBehaviour extension with helper features for subcribing to and unsubscribing from ScriptableObject-based GameEvent and GameProperty objects.
    /// </summary>
    public abstract class SubscriptionHelperMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Specifies what should happen when the GameObject is re-enabled:
        /// 'Push' means that the event callback will be instantly called with the current value
        /// </summary>
        protected enum Resume
        {
            Push,
            NoPush
        }

        //private List<(Action Subscribe, Action Unsubscribe, Action Push)> _actionSets = new List<(Action, Action, Action)>();
        private Dictionary<object, (Action Subscribe, Action Unsubscribe, Action Push)> _actionMap = new Dictionary<object, (Action Subscribe, Action Unsubscribe, Action Push)>();

        /// <summary>
        /// Subscribes to a GameEvent, and handles all relevant responsibilities, i.e. automatically suspends subscription while the GameObject is disabled.
        /// </summary>
        /// <param name="gameEvent">The GameEvent to subscribe to.</param>
        /// <param name="callback">The method to be called when the event fires.</param>
        protected void AddSubscription<T>(GameEvent<T> gameEvent, Action<T> callback)
            => AddSubscription_internal(gameEvent, callback, null);

        /// <summary>
        /// Subscribes to a GameProperty, and handles all relevant responsibilities, i.e. automatically suspends subscription while the GameObject is disabled.
        /// </summary>
        /// <param name="gameProperty">The GameProperty to subscribe to.</param>
        /// <param name="callback">The method to be called when the change notification event fires.</param>
        /// <param name="resumeBehaviour">'Push' means the current value will be instantly sent when the GameObject becomes enabled after it was disabled. Can be useful for making sure that the component is up to date with the current game state.
        protected void AddSubscription<T>(GameProperty<T> gameProperty, Action<T> callback, Resume resumeBehaviour)
        {
            Action pushAction = null;
            if (resumeBehaviour == Resume.Push)
                pushAction = () => callback(gameProperty.Get());

            AddSubscription_internal(gameProperty, callback, pushAction);
        }

        private void AddSubscription_internal<T>(IEventSource<T> eventSource, Action<T> callback, Action pushAction)
        {
            // Can be null if a field of this type is not assigned in Editor
            if (eventSource == null)
            {
                Debug.LogWarning($"Cannot add subscription, event source is Null. Field probably not assigned in Editor (which might be normal).");
                return;
            }

            // Already subscribed to given event source
            if (_actionMap.ContainsKey(eventSource))
                return;

            eventSource.Event += callback;

            // Store actions for subsequent subscription suspend and resume
            {
                _actionMap.Add(
                    key: eventSource,
                    value:
                        //TODO: Factor out closure allocations, ideally
                        (Subscribe: () => eventSource.Event += callback,
                        Unsubscribe: () => eventSource.Event -= callback,
                        Push: pushAction)
                );
            }
        }

        /// <summary>
        /// Entirely removes the subscription to a given GameProperty or GameEvent.
        /// Don't use it for OnDisable/OnEnable safety; this functionality is automatic.
        /// </summary>
        protected void RemoveSubscription<T>(IEventSource<T> eventSource)
            => RemoveSubscription_internal(eventSource);

        /// <summary>
        /// If you override this Unity Magic™ method, BE SURE TO CALL base.OnEnable() from your method
        /// </summary>
        protected virtual void OnEnable()
            => ResumeSubscriptions();

        /// <summary>
        /// If you override this Unity Magic™ method, BE SURE TO CALL base.OnDisable() from your method
        /// </summary>
        protected virtual void OnDisable()
            => UnsubscribeAll();

        /// <summary>
        /// If you override this Unity Magic™ method, BE SURE TO CALL base.OnDestroy() from your method
        /// </summary>
        protected virtual void OnDestroy()
            => UnsubscribeAll(); // No need for record removal, right? It will be destroyed after all.

        private void RemoveSubscription_internal(object key)
        {
            if (!_actionMap.TryGetValue(key,
                out var actionSet)) // Out
                return;

            actionSet.Unsubscribe();
            _actionMap.Remove(key);
        }

        private void ResumeSubscriptions()
        {
            foreach (var e in _actionMap) // Iterate over KVPs instead of .Values to avoid heap allocation of ValueCollection
            {
                e.Value.Subscribe();
                e.Value.Push?.Invoke(); // Push delegate is non-null only if push was specifically requested at subscription
            }
        }

        private void UnsubscribeAll()
        {
            foreach (var e in _actionMap)
                e.Value.Unsubscribe();
        }
    }
}