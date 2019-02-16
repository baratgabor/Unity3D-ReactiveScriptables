using UnityEngine;
using UnityEngine.UI;

namespace LeakyAbstraction.ReactiveScriptables
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Graphic))]
    public class GraphicColorSetter : ColorSetterBase<Graphic>
    {
        protected override void SetColor(Color color)
            => _component.color = color;
    }
}