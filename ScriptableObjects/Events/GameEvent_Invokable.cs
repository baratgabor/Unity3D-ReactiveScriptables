using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu]
    public class GameEvent_Invokable : GameEvent
    {
        public void Invoke()
            => OnEvent(this);
    }
}