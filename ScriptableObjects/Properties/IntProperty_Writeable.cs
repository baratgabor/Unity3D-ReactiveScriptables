using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Properties/Int Property - Writeable")]
    public class IntProperty_Writeable : IntProperty
    {
        new public void Set(int value)
            => SetAndNotify(value);
    }
}
