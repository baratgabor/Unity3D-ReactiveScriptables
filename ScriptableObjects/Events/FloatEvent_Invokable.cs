using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Events/Float Event - Invokable")]
    public class FloatEvent_Invokable : FloatEvent
    {
        public void Invoke(float eventData)
            => OnEvent(eventData);
    }
}