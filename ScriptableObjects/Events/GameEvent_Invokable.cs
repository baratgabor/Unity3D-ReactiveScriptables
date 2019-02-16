using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [CreateAssetMenu(menuName = "Game Events/Game Event - Invokable")]
    public class GameEvent_Invokable : GameEvent
    {
        public void Invoke()
            => OnEvent(this);
    }
}