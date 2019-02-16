using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections;

public enum GameAnimation
{
    None,
    AppearByUpscale,
    DisappearByDownscale,
    ExpandSnap,
    HorizontalSnap,
    FadeIn,
    FadeOut,
    ExpandSnapStrong
}

namespace LeakyAbstraction.ReactiveScriptables
{
    /// <summary>
    /// Adds configurable interactability to objects, primarily designed for UI elements.
    /// </summary>
    public class Interactable : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler
    {
        [Serializable]
        public class Interaction
        {
            [Header("Interaction sound and animation:")]
            public GameSound sound;
            public GameAnimation animation;

            [Header("Delay execution of actions:")]
            public bool waitForSound;
            public bool waitForAnimation;

            [Header("Actions to execute:")]
            public GameEvent_Invokable gameEvent;
            public UnityEvent unityEvent;
        }

        [Header("Pointer Down:")]
        [SerializeField]
        private Interaction _pointerDown = new Interaction();

        [Header("Pointer Click:")]
        [SerializeField]
        private Interaction _pointerClick = new Interaction();

        [Header("Pointer Up:")]
        [SerializeField]
        private Interaction _pointerUp = new Interaction();

        [Header("Pointer Drag Begin:")]
        [SerializeField]
        private Interaction _pointerDragBegin = new Interaction();

        [Header("Pointer Drag End:")]
        [SerializeField]
        private Interaction _pointerDragEnd = new Interaction();

        // Return Animation component, or set and return it if null
        private Animation _animation => ___animation ?? (___animation = GetAnimationComponent());
        private Animation ___animation;

        private Interaction[] _interactions;
        private HashSet<AnimationClip> _registeredClips = new HashSet<AnimationClip>();
        private bool _awaitedExecutionRunning = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // If we modify the settings in the Inspector while playing, animations need to be re-registered to ensure they can play
            if (GUI.changed && EditorApplication.isPlaying && AnyAnimationRequested())
            {
                Debug.Log($"{nameof(Interactable)} detected GUI change. Re-registering AnimationClips.");
                RegisterAnimationClips();
            }
        }
#endif

        private void Start()
        {
            // IMPORTANT: Extend this if new interactions are added to the class.
            // This array facilitates operating executed on all interactions, without having to hardcode them individually in multiple places.
            _interactions = new Interaction[] {
            _pointerDown,
            _pointerClick,
            _pointerUp,
            _pointerDragBegin,
            _pointerDragEnd
        };

            if (AnyAnimationRequested())
                RegisterAnimationClips();
        }

        private Animation GetAnimationComponent()
        {
            Animation animation = GetComponent<Animation>();

            if (animation == null)
                animation = gameObject.AddComponent<Animation>();

            return animation;
        }

        /// <summary>
        /// Register animations clips in Animation component.
        /// Unfortunately this has to be done, because setting the default clip doesn't add it automatically.
        /// </summary>
        private void RegisterAnimationClips()
        {
            foreach (var i in _interactions)
                TryRegister(i);

            void TryRegister(Interaction interaction)
            {
                if (interaction.animation == GameAnimation.None)
                    return;

                var clip = TestLookupBehaviour.Instance.Get(interaction.animation);

                if (!_registeredClips.Contains(clip))
                {
                    //Debug.Log($"Registering Animation Clip '{clip.name}'");
                    _animation.AddClip(clip, clip.name);
                    _registeredClips.Add(clip);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
            => ExecuteInteraction(_pointerClick);

        public void OnPointerDown(PointerEventData eventData)
            => ExecuteInteraction(_pointerDown);

        public void OnPointerUp(PointerEventData eventData)
            => ExecuteInteraction(_pointerUp);

        public void OnBeginDrag(PointerEventData eventData)
        => ExecuteInteraction(_pointerDragBegin);

        public void OnEndDrag(PointerEventData eventData)
            => ExecuteInteraction(_pointerDragEnd);

        private void ExecuteInteraction(Interaction interaction)
        {
            AudioSource audioSource = null;
            if (interaction.sound != GameSound.None)
                audioSource = SoundManager.Instance.PlaySound(interaction.sound);

            if (interaction.animation != GameAnimation.None)
                PlayAnimation(TestLookupBehaviour.Instance.Get(interaction.animation));

            // If any action is requested, and currently no delayed execution is running
            if (!_awaitedExecutionRunning && (interaction.gameEvent != null || interaction.unityEvent != null))
            {
                // If any waiting is requested
                if (interaction.waitForAnimation || interaction.waitForSound)
                    StartCoroutine(ExecuteActionsAwaited(interaction, _animation, audioSource));
                else
                    ExecuteActions(interaction);
            }
        }

        //TODO: Consider encapsulating interaction-processing into a separate class, or make AwaitedExecutionRunning checks per-interaction.
        private IEnumerator ExecuteActionsAwaited(Interaction interaction, Animation animation, AudioSource audioSource)
        {
            _awaitedExecutionRunning = true;

            //TODO: Consider less simplistic (but similarly dependable) waiting implementations, preferably without polling

            if (interaction.waitForAnimation && animation != null)
                yield return animation.WaitForFinish();

            if (interaction.waitForSound && audioSource != null)
                while (audioSource.isPlaying) yield return null;

            ExecuteActions(interaction);

            _awaitedExecutionRunning = false;
        }

        private void ExecuteActions(Interaction interaction)
        {
            if (interaction.gameEvent != null)
                interaction.gameEvent.Invoke();

            if (interaction.unityEvent != null)
                interaction.unityEvent.Invoke();
        }

        private void PlayAnimation(AnimationClip clip)
        {
            //TODO: Make this less naive
            if (_animation.isPlaying)
                return;

            _animation.clip = clip;
            _animation.Play();
        }

        private bool AnyAnimationRequested()
        {
            bool any = false;
            foreach (var i in _interactions)
                any = any || i.animation != GameAnimation.None;

            return any;
        }
    }
}