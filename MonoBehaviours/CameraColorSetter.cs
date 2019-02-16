using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraColorSetter : ColorSetterBase<Camera>
    {
        protected override void SetColor(Color color)
            => _component.backgroundColor = color;
    }
}