using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Properties/Bounds Property - Writeable")]
    public class BoundsProperty_Writeable : BoundsProperty
    {
        new public void Set(Bounds value)
            => SetAndNotify(value);
    }
}