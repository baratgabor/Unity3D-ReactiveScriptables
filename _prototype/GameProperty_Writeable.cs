using System;

namespace LeakyAbstraction.ReactiveScriptables.Prototyping
{
    /// <summary>
    /// Doesn't work. If you derive from this abstract class, your class won't be Editor-compatible
    /// with a readonly derivation of GameProperty<T>.
    /// So, for example FloatProperty_Writeable : GameProperty_Writeable<float> won't be assignable
    /// to readonly FloatVariable : GameProperty<float> fields in the editor.
    /// Thus, all writeable concrete classes NEED TO DERIVE DIRECTLY FROM THE READONLY VERSION.
    /// </summary>
    [Obsolete("Don't derive from this class. Read the class description.")]
    public abstract class GameProperty_Writeable<T> : GameProperty<T>
    {
        new public void Set(T value)
            => SetAndNotify(value);
    }
}