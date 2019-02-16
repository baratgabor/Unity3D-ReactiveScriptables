namespace LeakyAbstraction.ReactiveScriptables
{
    public class GameEvent : GameEvent<GameEvent>
    {
        // The only purpose of this class is to allow MonoBehaviour components to declare fields of this type, for read-only access to events.
        // To these fields the invokable version of the event class will be assigned to, as read-only.

        // Having the type parameter set to itself means it will let the subsribers know which GameEvent instance is the origin of the event trigger they are receiving.
    }
}