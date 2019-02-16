using System;
using UnityEngine;
using UnityEngine.UI;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(Selectable))]
    public class SelectableColorSetter : MonoBehaviour
    {
        [SerializeField]
        private ColorState _colorVariable = default;

        [SerializeField]
        private ColorTransformation _normalColorSetter = new ColorTransformation();
        [SerializeField]
        private ColorTransformation _highlightedColorSetter = new ColorTransformation();
        [SerializeField]
        private ColorTransformation _pressedColorSetter = new ColorTransformation();

        private Selectable _selectable => ___selectable ?? (___selectable = GetComponent<Selectable>());
        private Selectable ___selectable;

        void Start()
        {
            ___selectable = GetComponent<Selectable>();

            if (_colorVariable == null)
                throw new Exception("Color preset missing.");
            OnColorChanged(_colorVariable.Get());
        }

        private void OnEnable()
            => _colorVariable.Event += OnColorChanged;

        private void OnDisable()
            => _colorVariable.Event -= OnColorChanged;

        private void OnColorChanged(Color color)
        {
            var colorBlock = _selectable.colors;

            if (_normalColorSetter.Enabled)
                colorBlock.normalColor = _normalColorSetter.Transform(color);
            if (_highlightedColorSetter.Enabled)
                colorBlock.highlightedColor = _highlightedColorSetter.Transform(color);
            if (_pressedColorSetter.Enabled)
                colorBlock.pressedColor = _pressedColorSetter.Transform(color);

            _selectable.colors = colorBlock;
        }
    }
}