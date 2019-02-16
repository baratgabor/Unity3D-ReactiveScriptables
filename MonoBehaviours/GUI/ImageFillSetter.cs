using UnityEngine;
using UnityEngine.UI;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class ImageFillSetter : MonoBehaviour
    {
        [Tooltip("Value to use as the current ")]
        public FloatState Variable;

        [Tooltip("Min value that Variable to have no fill on Image.")]
        public float Min;

        [Tooltip("Max value that Variable can be to fill Image.")]
        public FloatState Max;

        [Tooltip("Image to set the fill amount on.")]
        public Image Image;

        private void Update()
        {
            Image.fillAmount = Mathf.Clamp01(
                Mathf.InverseLerp(Min, Max.Get(), Variable.Get()));
        }
    }
}