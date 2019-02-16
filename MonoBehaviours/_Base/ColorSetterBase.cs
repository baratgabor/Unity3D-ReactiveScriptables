using System;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [Serializable]
    public class ColorTransformation
    {
        public bool Enabled;
        [Range(0, 10f)]
        public float HueMultiplier = 1f;
        [Range(0, 10f)]
        public float SaturationMultiplier = 1f;
        [Range(0, 10f)]
        public float ValueMultiplier = 1f;
        [Range(0, 10f)]
        public float AlphaMultiplier = 1f;

        public Color Transform(Color color)
        {
            Color.RGBToHSV(color,
                out float H,
                out float S,
                out float V);

            H = Mathf.Clamp01(H * HueMultiplier);
            S = Mathf.Clamp01(S * SaturationMultiplier);
            V = Mathf.Clamp01(V * ValueMultiplier);

            var newColor = Color.HSVToRGB(H, S, V);
            newColor.a = Mathf.Clamp01(color.a * AlphaMultiplier);

            return newColor;
        }
    }

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public abstract class ColorSetterBase<T> : SubscriptionHelperMonoBehaviour
        where T : UnityEngine.Object
    {
        [SerializeField]
        private ColorState _colorVariable = default;

        [SerializeField]
        private ColorTransformation _colorTransformation = new ColorTransformation();

        protected T _component => ___component ?? (___component = GetComponent<T>());
        private T ___component;

        // To be implemented in concrete classes
        protected abstract void SetColor(Color color);

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Execute color update if settings are changed in the editor
            if (GUI.changed && _colorVariable != null)
            {
                OnColorChanged(_colorVariable.Get());
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Execute subscription and initial color update in Editor
            if (_colorVariable != null)
            {
                ___component = GetComponent<T>(); // Pull component manually because of that bloody Unity hack that assignes some bogus shit in the Editor which is not actually the object, but still not null.
                AddSubscription(_colorVariable, OnColorChanged, Resume.Push);
                OnColorChanged(_colorVariable.Get());
            }
        }
#endif

        void Start()
        {
            ___component = GetComponent<T>();

            if (_colorVariable == null)
            {
                Debug.LogWarning("ColorVariable dependency not set.");
                return;
            }

            AddSubscription(_colorVariable, OnColorChanged, Resume.Push);
            OnColorChanged(_colorVariable.Get());
        }

        private void OnColorChanged(Color color)
        {
            if (_colorTransformation.Enabled)
                color = _colorTransformation.Transform(color);

            SetColor(color);
        }
    }
}