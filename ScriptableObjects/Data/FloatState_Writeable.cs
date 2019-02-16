using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class FloatState_Writeable : FloatState
    {
        new public void Set(float value)
            => SetAndNotify(value);
    }
}
