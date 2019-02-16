using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class AudioPlaybackModulator : MonoBehaviour
    {
        [Header("Identifier (cosmetic):")]
        [SerializeField]
        private string _modulatorName = default;

        [Header("Audio source to modulate:")]
        [SerializeField]
        private AudioSource _audioSource = default;

        [Header("Source settings:")]
        [SerializeField]
        private FloatState _modulationSource = default;
        [SerializeField]
        private float _sourceRangeStart = default;
        [SerializeField]
        private float _sourceRangeEnd = default;

        [Header("Pitch modulation:")]
        [SerializeField]
        private bool _modulatePitch = false;
        [SerializeField]
        [Range(0, 3)]
        private float _pitchRangeStart = 1;
        [SerializeField]
        [Range(0, 3)]
        private float _pitchRangeEnd = 1;

        [Header("Volume modulation:")]
        [SerializeField]
        private bool _modulateVolume = false;
        [SerializeField]
        [Range(0, 1)]
        private float _volumeRangeStart = 1;
        [SerializeField]
        [Range(0, 1)]
        private float _volumeRangeEnd = 1;

        [Header("Smoothing:")]
        [SerializeField]
        private bool _applySmoothing = false;
        [SerializeField]
        private float _smoothingTimeInSeconds = 1;

        [Header("Easing:")]
        [SerializeField]
        private bool _applyEasing = false;
        [SerializeField]
        private MyMath.Easing _easingType = MyMath.Easing.None;

        private float _previousFraction = 0;

        private void Start()
        {
            if (_audioSource == null || _modulationSource == null)
                throw new System.Exception("Dependencies not set.");

            if (_audioSource.clip == null)
                throw new System.Exception("Dependency not configured: no AudioClip selected.");
        }

        private void Update()
        {
            if (!_modulateVolume && !_modulatePitch)
                return;

            var fractionValue = _modulationSource.Get()
                .MapTo01(_sourceRangeStart, _sourceRangeEnd);

            if (_applyEasing)
                fractionValue = MyMath.Ease01(fractionValue, _easingType);

            if (_applySmoothing)
                fractionValue = Mathf.MoveTowards(_previousFraction, fractionValue, Time.deltaTime * (1 / _smoothingTimeInSeconds));

            if (_modulateVolume)
                _audioSource.volume = fractionValue.Map01To(_volumeRangeStart, _volumeRangeEnd);

            if (_modulatePitch)
                _audioSource.pitch = fractionValue.Map01To(_pitchRangeStart, _pitchRangeEnd);

            _previousFraction = fractionValue;
        }
    }
}