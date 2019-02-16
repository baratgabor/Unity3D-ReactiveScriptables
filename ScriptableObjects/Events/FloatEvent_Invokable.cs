using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class FloatEvent_Invokable : FloatEvent
    {
        public void Invoke(float eventData)
            => OnEvent(eventData);
    }
}