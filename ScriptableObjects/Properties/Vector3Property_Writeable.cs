using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Properties/Vector3 Property - Writeable")]
    public class Vector3Property_Writeable : Vector3Property
    {
        new public void Set(Vector3 value)
            => SetAndNotify(value);
    }
}
