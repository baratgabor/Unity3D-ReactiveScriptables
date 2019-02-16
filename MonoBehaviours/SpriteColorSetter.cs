using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteColorSetter : ColorSetterBase<SpriteRenderer>
    {
        protected override void SetColor(Color color)
            => _component.color = color;
    }
}