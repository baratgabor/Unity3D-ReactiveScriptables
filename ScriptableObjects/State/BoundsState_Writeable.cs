using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class BoundsState_Writeable : BoundsState
    {
        new public void Set(Bounds value)
            => SetAndNotify(value);
    }
}