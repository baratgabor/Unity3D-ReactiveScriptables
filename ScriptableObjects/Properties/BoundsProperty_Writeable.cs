using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class BoundsProperty_Writeable : BoundsProperty
    {
        new public void Set(Bounds value)
            => SetAndNotify(value);
    }
}