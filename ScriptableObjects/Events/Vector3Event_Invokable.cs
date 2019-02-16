using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Events/Vector3 Event - Invokable")]
    public class Vector3Event_Invokable : Vector3Event
    {
        public void Invoke(Vector3 eventData)
            => OnEvent(eventData);
    }
}
