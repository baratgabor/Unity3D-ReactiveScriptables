using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class Vector3Event_Invokable : Vector3Event
    {
        public void Invoke(Vector3 eventData)
            => OnEvent(eventData);
    }
}
