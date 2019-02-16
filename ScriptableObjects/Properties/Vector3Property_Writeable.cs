using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    public class Vector3Property_Writeable : Vector3Property
    {
        new public void Set(Vector3 value)
            => SetAndNotify(value);
    }
}
