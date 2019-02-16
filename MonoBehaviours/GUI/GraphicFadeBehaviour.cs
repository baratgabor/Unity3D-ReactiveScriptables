using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(Graphic))]
    public class GraphicFadeBehaviour : SubscriptionHelperMonoBehaviour
    {
        enum StartupAction
        {
            None,
            FadeIn,
            FadeOut
        }

        [Header("Startup settings:")]
        [SerializeField]
        StartupAction _startupAction = StartupAction.None;

        [SerializeField]
        [Range(0, 1)]
        private float _startupFadeValue = 0;

        [Header("No fade behavior:")]
        [SerializeField]
        private bool _disableGraphicOnZeroFade = true;

        [Header("Fade duration:")]
        [SerializeField]
        [Range(0.1f, 2)]
        private float _duration = 0.5f;
        [SerializeField]
        private bool _constantDuration = true;

        [Header("Fade In settings:")]
        [SerializeField]
        private GameEvent _fadeInOn = default;

        [SerializeField]
        [Range(0, 1)]
        private float _fadeInTarget = 1;

        [Header("Fade Out settings:")]
        [SerializeField]
        private GameEvent _fadeOutOn = default;

        [SerializeField]
        [Range(0, 1)]
        private float _fadeOutTarget = 0;

        [Header("Invoke when fade finished:")]
        [SerializeField]
        private GameEvent_Invokable _onFadeFinished = default;

        private Graphic _graphic;
        private Coroutine _activeFadeJob;
        private bool _graphicDisabled = false;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();

            // Start subscription handling of events we're interested in
            AddSubscription(_fadeInOn, OnFadeIn);
            AddSubscription(_fadeOutOn, OnFadeOut);

            // Push initial value
            SetAlpha(_startupFadeValue);

            DoStartupAction();
        }

        private void DoStartupAction()
        {
            switch (_startupAction)
            {
                case StartupAction.None:
                    return;
                case StartupAction.FadeIn:
                    if (_startupFadeValue != _fadeInTarget)
                        FadeIn();
                    break;
                case StartupAction.FadeOut:
                    if (_startupFadeValue != _fadeOutTarget)
                        FadeOut();
                    break;
                default:
                    throw new Exception($"'{nameof(StartupAction)}' Enum value '{_startupAction}' unknown.");
            }
        }

        private void OnFadeIn(GameEvent sender)
            => FadeIn();

        public void FadeIn()
            => Fade(_fadeInTarget);

        private void OnFadeOut(GameEvent sender)
            => FadeOut();

        public void FadeOut()
            => Fade(_fadeOutTarget);

        // Only here for UnityEvent binding
        public void Fade(float targetAlpha)
            => Fade(targetAlpha, null);

        public void Fade(float targetAlpha, Action callbackOnFinished = null)
        {
            // Interrupt running job, if any; to start approaching our new target
            if (_activeFadeJob != null)
                StopCoroutine(_activeFadeJob);

            // Make sure value is in valid range
            targetAlpha = Mathf.Clamp01(targetAlpha);

            float alphaDifference = Mathf.Abs(targetAlpha - _graphic.color.a);

            // No difference, nothing to do
            if (alphaDifference == 0)
                return;

            // Decrease duration to fraction of full, if requested
            float duration = _duration;
            if (!_constantDuration)
                duration = _duration * alphaDifference;

            _activeFadeJob = StartCoroutine(
                FadeJob(targetAlpha, duration, callbackOnFinished)
            );
        }

        private IEnumerator FadeJob(float targetAlpha, float duration, Action callback)
        {
            float newAlpha = 0;
            float originalAlpha = _graphic.color.a;
            float elapsedTime = 0;

            do
            {
                elapsedTime += Time.deltaTime;

                newAlpha = elapsedTime
                    .MapTo01(0, duration)
                    .Map01To(originalAlpha, targetAlpha);

                SetAlpha(newAlpha);

                yield return null;
            }
            while (newAlpha != targetAlpha);

            // Job finished, clear the cached instance
            // Not the responsibily of this method; shouldn't be here
            _activeFadeJob = null;
            // Same here, really
            callback?.Invoke();
            _onFadeFinished?.Invoke();
        }

        public void SetAlpha(float value)
        {
            // Enable if previously was disabled, and now we're setting non-zero alpha
            if (value != 0 && _graphicDisabled)
                SetImage(enabled: true);

            var color = _graphic.color;
            color.a = value;
            _graphic.color = color;

            // Disable when alpha is 0, if requested
            if (value == 0 && _disableGraphicOnZeroFade)
                SetImage(enabled: false);

            void SetImage(bool enabled)
            {
                _graphic.enabled = enabled;
                _graphicDisabled = !enabled;
            }
        }
    }
}