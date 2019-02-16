using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class FloatProperty_Writeable : FloatProperty
    {
        new public void Set(float value)
            => SetAndNotify(value);
    }
}
