using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    /// <summary>
    /// MonoBehaviour extension with helper features for subcribing to and unsubscribing from ScriptableObject-based GameEvent and GameState objects.
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

        private List<(Action Subscribe, Action Unsubscribe, Action Push)> _actionSets = new List<(Action, Action, Action)>();
        private Dictionary<object, int> _indexMap = new Dictionary<object, int>();

        /// <summary>
        /// Subscribes to a GameEvent, and handles all relevant responsibilities, i.e. automatically suspends subscription while the GameObject is disabled.
        /// </summary>
        /// <param name="gameState">The GameEvent to subscribe to.</param>
        /// <param name="callback">The method to be called when the event fires.</param>
        protected void AddSubscription<T>(GameEvent<T> gameEvent, Action<T> callback)
            => AddSubscription_internal(gameEvent, callback, null);

        /// <summary>
        /// Subscribes to a GameState, and handles all relevant responsibilities, i.e. automatically suspends subscription while the GameObject is disabled.
        /// </summary>
        /// <param name="gameState">The GameState to subscribe to.</param>
        /// <param name="callback">The method to be called when the change notification event fires.</param>
        /// <param name="resumeBehaviour">'Push' means the current state will be instantly sent when the GameObject becomes enabled after it was disabled. Can be useful for making sure that the component is up to date with the current game state.
        protected void AddSubscription<T>(GameState<T> gameState, Action<T> callback, Resume resumeBehaviour)
        {
            Action pushAction = null;
            if (resumeBehaviour == Resume.Push)
                pushAction = () => callback(gameState.Get());

            AddSubscription_internal(gameState, callback, pushAction);
        }

        private void AddSubscription_internal<T>(IEventSource<T> eventSource, Action<T> callback, Action pushAction)
        {
            // Can be null if a field of this type is not assigned in Editor
            if (eventSource == null)
            {
                Debug.LogWarning($"Cannot add subscription, event source is Null. Field probably not assigned in Editor (which might be normal).");
                return;
            }

            // Return if already subscribed
            if (_indexMap.ContainsKey(eventSource))
                return;

            // Subscribe immediately
            eventSource.Event += callback;

            // Store actions for subsequent subscription suspend and resume
            {
                //TODO: Factor out closure allocations, ideally :D
                _actionSets.Add((
                    Subscribe: () => eventSource.Event += callback,
                    Unsubscribe: () => eventSource.Event -= callback,
                    Push: pushAction
                ));

                _indexMap.Add(eventSource, _actionSets.Count - 1);
            }
        }

        /// <summary>
        /// Entirely removes the subscription to a given GameState or GameEvent.
        /// Don't use it for OnDisable/OnEnable safety; this functionality is automatic.
        /// </summary>
        /// <param name="gameState"></param>
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

        private void RemoveSubscription_internal(object keyObject)
        {
            if (!_indexMap.TryGetValue(keyObject,
                out int indexToRemoveAt)) // Out
                return;

            // Unsubscribe first
            _actionSets[indexToRemoveAt].Unsubscribe();

            // Remove records
            _actionSets.RemoveAt(indexToRemoveAt); // Not performant on List, but the assumption is that Count is small and invokations are rare.
            _indexMap.Remove(keyObject);
        }

        private void ResumeSubscriptions()
        {
            foreach (var a in _actionSets)
            {
                a.Subscribe();
                a.Push?.Invoke(); // Push delegate is non-null only if push was specifically requested at subscription
            }
        }

        private void UnsubscribeAll()
        {
            foreach (var a in _actionSets)
                a.Unsubscribe();
        }
    }
}