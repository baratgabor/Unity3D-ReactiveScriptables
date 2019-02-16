using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class IntState_Writeable : IntState
    {
        new public void Set(int value)
            => SetAndNotify(value);
    }
}
