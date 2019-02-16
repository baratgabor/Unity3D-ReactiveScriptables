using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class ColorProperty_Writeable : ColorProperty
    {
        new public void Set(Color value)
            => SetAndNotify(value);
    }
}
