using System;

namespace LeakyAbstraction.ReactiveScriptables
{
    /// <summary>
    /// Facilitates the unified handling of GameEvent<T> and GameProperty<T> objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventSource<T>
    {
        event Action<T> Event;
    }
}
