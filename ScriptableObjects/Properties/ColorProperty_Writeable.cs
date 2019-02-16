using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Properties/Color Property - Writeable")]
    public class ColorProperty_Writeable : ColorProperty
    {
        new public void Set(Color value)
            => SetAndNotify(value);
    }
}
