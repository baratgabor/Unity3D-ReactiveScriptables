using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class ColorState_Writeable : ColorState
    {
        new public void Set(Color value)
            => SetAndNotify(value);
    }
}
