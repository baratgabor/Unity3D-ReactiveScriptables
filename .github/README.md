# ScriptableObjects-based game architectural scaffolding for Unity3D game development
### Inspired by: [Unite Austin 2017 - Game Architecture with Scriptable Objects](https://www.youtube.com/watch?v=raQ3iHhE_Kk)
---
**State:** Work in progress, but fully usable; not unit-tested, but master should be stable.

**Requirements:** Tested in Unity 2018.3, requires scripting runtime set to '.Net 4.x equivalent' (uses C# 7 features).

---

**Readme is under construction; will take some time ;)**

Essentially this is an adapted/extended version of the ScriptableObject-based architectural approach introduced in the Unite talk linked above.

I will try to show with some example scenarios and pictures why I think this architecture works pretty well for a lot of common problems.

This is used as a Git submodule in my project; in 2-3 classes I still need to work out some solution to a few dependencies on my sound and animation manager (notably in [Interactable.cs](https://github.com/baratgabor/Unity3D-ReactiveScriptables/blob/master/MonoBehaviours/GUI/Interactable.cs)). However, you can actually find the [SoundManager here on GitHub](https://github.com/baratgabor/Unity3D-SoundManager). I'm working on a system that couples these service components to interfaces and sends them out to other, dependent component listeners, wrapped into a `GameProperty<T> ScriptableObject`.

*(FYI the naming is a real struggle for me here. I spent literally like 2 days thinking about how to call this module, plus how to call the state holding class, etc. I didn't like the original GameVariable name, and I wanted to differentiate its use from the normal variables/fields we use (that's why I also went with `Get()` and `Set()` instead of property accessors). So it's entirely possible that I'll rename a bunch of things.)*

### Main selling point:

  - Powerful Editor-configurability for teams with non-programmer workers, e.g. artists and designers. Because obviously it would be much easier to just use for example a message bus / event aggregator to send payloads to listeners.

### Suitable for:

  - Injecting pre-defined data or configuration into `MonoBehaviour` components.
  - Exchanging pre-defined types of data between `MonoBehaviour` components, either through polling or event subscription, without creating hard references between them.
  - Creating reactive, or event-driven, workflow between components with events and change notifications. (But for now don't expect real reactive features, like map, filter, etc. ;))
  - Driving GUI behaviour and interactivity.

### Probably not suitable for:

  - Highly complex games, because the data types are really fine-grained, and if you need to create hundreds of them, that would probably get messy. However, you can easily extend this system with your own, less fine grained types.
  - Scenarios where you need to create and propagate state dynamically, since this is all about using pre-defined `ScriptableObject` instances. Of course in a lot of cases what you actually need is to hook the components onto a communication channel, and these channels are usually pre-definable.

## Example Usage Scenario: Handling item pickups / projectile impacts

The common scenario of item pickups (e.g. coins in platformers), or projectile impacts in 3D games. This often requires triggering `ParticleSystems`, `AudioSources`, and updating game statistics / UI, especially in more polished projects.

### • Simplistic Unity-approach:
 
![GitHub Logo](/.github/ExampleScenario1-Simplistic.png)
 
**Workflow:** 

  - You add a `ParticleSystem` and an `AudioSource` component directly to your `GameObject`.
  - You reference various other components via e.g. singletons or Editor-associations, and directly call methods on them.
  - You hide the `GameObject`'s renderer, and destroy/disable the `GameObject` in a delayed manner (since the `ParticleSystem` and `AudioSource` still need to finish).

**Key characteristics:**

  - Very simple and easy to learn approach, which doesn't require any framework, or understanding of software architecture.
  - The `GameObject` itself assumes responsibility for everything that needs to happen when it's triggered or collided into.
  - Many components, e.g. `ParticleSystems` and `AudioSources`, are duplicated on each `GameObject` instance.  
  - Even simple `GameObjects` and prefabs start to feel tangled and bloated as you add more polish to the game and include particle effects, sounds, UI updates, game statistics, etc.    

### • ScriptableObject-based event-driven approach:

![GitHub Logo](/.github/ExampleScenario1-EventDriven.png)

**Workflow:**

  - *(Required only first time)* You create a `struct` or `class` that will contain all the data relevant to your event (or skip this, and just use a primitive type, if that's enough).
  - *(Required only first time)* You create a `ScriptableObject`-based asset that will serve as an Editor-assignable send/receive channel for your event data.
  - You assign this created `ScriptableObject`-based communication asset to all your `GameObject` script (as *invokable*), and to all other components that want to listen and react (as *readonly*).
  - Your script simply invokes the event, which notifies all subscribers.

**Key characteristics:**

  - Requires more work and understanding to set up first.
  - The `GameObject` has a single responsibility, and the other components which subscribe to this event take care of their own relevant responsibility.
  - The number of components on each `GameObject` instances can be minimized; often even `ParticleSystems` and `AudioSources` can be removed and handled in a separate single component which is responsible for reacting to events at world coordinates.
  - Your `GameObjects` and prefabs can remain very simple, even in a game that is highly polished with dozens of various audio/visual/UI reactions to events.

## Main Features

### Differentiated read-only and writeable use
  - You can create read-only and writeable instances of the data-holding classes.
  - You can associate your writeable data classes (and invokable event classes) with readonly fields in the Editor.
    - This means you can express clearly the intent that an event or data is an input of your component, and have automaticly enforced write-protection.
  - Essentially you can avoid the situation of creating mutable shared state in your architecture. Which almost always leads to problems.
    - My recommendation is that for each writeable instance you should have a single component that writes to it, and the rest should only listen.
    
### Built-in change notifiations
  - Not just the event classes, but the data-holding classes have an event too that notifies of value changes.
  - Works sort of similarly to the `INotifyPropertyChanged` interface in .Net, in the sense that you can listen to changes related to a unit of data, and react in an event-driven manner.

### Easily extendable generic base classes
  - The common types, e.g. `float`, `int`, `Vector3`, `Bounds` already have built-in concrete classes, but you can also create your own classes, including ones based on your custom types. Basically this is all you need to create a concrete type that you can use in the Editor (as we know, the Editor doesn't support generics, so you need to create non-generic derived classes):
  
```csharp
    [CreateAssetMenu]
    public class BoundsProperty : GameProperty<Bounds>
    { }
```
  
### Subscription helper MonoBehaviour extension class
  - If you derive your components from `SubscriptionHelperMonoBehaviour` instead of `MonoBehaviour`, you can add subscriptions easily by invoking the `AddSubscription()` method.
  - This automatically handles all subscription-related responsibilities, i.e. unsubscribing in `OnDisable()`, resubscribing in `OnEnable()`, and again unsubscribing in `OnDestroy()`.
  - Supports all data types automatically, including your own custom made types which derive from `GameEvent<T>` or `GameProperty<T>`.
  - (Still need to refactor this, because it uses closing/allocating lambdas currently.)
  
### Built-in, ready to use generic MonoBehaviour components
  - I included plenty of `MonoBehaviour` components I made over the last few weeks for myself; generally these are:
    - Component triggers and event counters
    - **GUI interactivity helpers** for replacing the inflexible `Button` component
    - **GUI skinning helpers** for defining and associating colors which synchronize automatically (even in Edit mode).
      - Includes color transformation capability, for defining H, S, V, A transformation on the received color.
      - Based And a generic base class you can use for making color setters.
    - **Audio playback modulation** for creating dynamic car engine, etc. sounds based on a float (e.g. speed)
  
### Lightweight, not too OOP
  - Has a 2-3 levels deep inheritance hierarchy here and there, but generally it's not overstructured and overcomplicated. I'd be glad to rely more on interfaces, composition and abstractions, but sadly it seems nearly impossible in Unity (if you want to keep things Editor-compatible).
