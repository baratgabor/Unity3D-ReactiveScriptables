using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Properties/Float Property - Writeable")]
    public class FloatProperty_Writeable : FloatProperty
    {
        new public void Set(float value)
            => SetAndNotify(value);
    }
}
