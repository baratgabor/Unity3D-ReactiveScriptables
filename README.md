# ScriptableObjects-based game architectural scaffolding for Unity3D

**- Readme is under construction; will take some time ;) -**

Essentially this is my adaptation (and extension) of the ScriptableObject-based architectural approach introduced in this Unite-talk:

### [ - YouTube: Unite Austin 2017 - Game Architecture with Scriptable Objects](https://www.youtube.com/watch?v=raQ3iHhE_Kk)

I don't claim that this architecture is universally applicable to all problems; indeed, it might be even seen as an anti-pattern from a strict SOLID/OOP standpoint. But our toolset is quite limited in Unity, and it's extremely easy to work with this approach, and seems to promote a generally positive, more loosely coupled design compared to the direct referencing between components you can often see in Unity3D projects.

## Main Features

### Differentiated read-only and writeable use
  - You can create read-only and writeable instances of the state-holding classes.
  - You can associate your writeable state classes (and invokable event classes) with readonly fields in the Editor.
    - This means you can express clearly the intent that an event or data is an input of your component, and have automaticly enforced write-protection.
  - Essentially you can avoid the situation of creating mutable shared state in your architecture. Which almost always leads to problems.
    - My recommendation is that for each writeable instance you should have a single component that writes to it, and the rest should only listen.
    
### Built-in change notifiations
  - Not just the event classes, but the state-holding classes have an event too that notifies of state changes.

### Easily extendable generic base classes
  - The common types, e.g. `float`, `int`, `Vector3`, `Bounds` already have built-in concrete classes, but you can also create your own classes, including ones based on your custom types. Basically this is all you need to create a concrete type that you can use in the Editor (as we know, the Editor doesn't support generics, so you need to create non-generic derived classes):
  
```csharp
    [CreateAssetMenu]
    public class BoundsState : GameState<Bounds>
    { }
```
  
### Subscription helper MonoBehaviour extension class
  - If you derive your components from `SubscriptionHelperMonoBehaviour` instead of `MonoBehaviour`, you can add subscriptions easily by invoking the `AddSubscription()` method.
  - This automatically handles all subscription-related responsibilities, i.e. unsubscribing in `OnDisable()`, resubscribing in `OnEnable()`, and again unsubscribing in `OnDestroy()`.
  - Supports all data types automatically, including your own custom made types which derive from `GameEvent<T>` or `GameState<T>`.
  
### Built-in, ready to use generic MonoBehaviour components
  - I included plenty of `MonoBehaviour` components I made over the last few weeks for myself; generally these are:
    - Component triggers and event counters
    - **GUI interactivity helpers** for replacing the inflexible `Button` component
    - **GUI skinning helpers** for defining and associating colors which synchronize automatically (even in Edit mode).
      - Includes color transformation capability, for defining H, S, V, A transformation on the received color.
      - Based And a generic base class you can use for making color setters.
    - **Audio playback modulation** for creating dynamic car engine, etc. sounds based on a float (e.g. speed)
  
### Lightweight, not too OOP
  - Has a 2-3 levels deep inheritance hierarchy here and there, but generally it's not overstructured and overcomplicated. I'd be glad to rely more on interfaces, composition and abstractions, but sadly it seems nearly impossible in Unity.
