using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class Vector3State_Writeable : Vector3State
    {
        new public void Set(Vector3 value)
            => SetAndNotify(value);
    }
}
