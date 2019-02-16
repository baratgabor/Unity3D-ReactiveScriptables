using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class IntProperty_Writeable : IntProperty
    {
        new public void Set(int value)
            => SetAndNotify(value);
    }
}
