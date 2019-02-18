using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables.Prototyping
{

    // 1) Resolve dependencies by defining services as proper abstractions; i.e. interfaces
    // Still doesn't resolve the dependency on these enums, but that will probably stay as it is
    public interface ISoundPlaybackService
    {
        AudioSource PlaySound(GameSound soundType, Action<GameSound> playFinishedCallback = null);
        AudioSource PlaySoundPositioned(GameSound soundType, Vector3 soundPosition, Action<GameSound> playFinishedCallback = null);
        AudioSource PlaySoundPositioned(GameSound soundType, Transform targetTransform, Action<GameSound> playFinishedCallback = null);
        AudioSource PlaySound(GameSound soundType, float volumeMultiplier, float pitchMultiplier, Action<GameSound> playFinishedCallback = null);
        AudioSource PlaySoundPositioned(GameSound soundType, float volumeMultiplier, float pitchMultiplier, Vector3 soundPosition, Action<GameSound> playFinishedCallback = null);
        AudioSource PlaySoundPositioned(GameSound soundType, float volumeMultiplier, float pitchMultiplier, Transform targetTransform, Action<GameSound> playFinishedCallback = null);
    }

    public interface IAnimationLookup
    {
        Animation Get(GameAnimation animationType);
    }
    // Etc.

    // 2) Create service interface broadcast containers for injection into dependent components
    // 'Property' sounds horrible here - rename to Container perhaps? Sigh...
    // 'Writeable' also sound crap here - Settable would be more universal?
    public class SoundServiceProperty : GameProperty<ISoundPlaybackService>
    { }

    public class SoundServiceProperty_Writeable : SoundServiceProperty
    {
        new public void Set(ISoundPlaybackService service)
            => SetAndNotify(service);
    }

    public class AnimationLookupProperty : GameProperty<IAnimationLookup>
    { }

    public class AnimationLookupProperty_Writeable : AnimationLookupProperty
    {
        new public void Set(IAnimationLookup animationLookup)
            => SetAndNotify(animationLookup);
    }
    // Etc.

    // Component that couples servicecomponents to service interface containers in a centralized way,
    // and sends them out (injecting them) into the dependent components in the scene
    public class ServiceCoupler : MonoBehaviour
    {
        // Editor-exposed field to assign component to
        // Normal MonoBehaviour component, for Editor-assignability (and access to MonoBehaviour-based services)
        //[SerializeField]
        //private SoundManager _soundManager = default;

        // Better yet: Instead of assigning, try to directly instantiate service components, i.e. make them non-MonoBehaviour, 
        // inject the useful MonoBehaviour base class members (transform, gameObject) into them as an interface, e.g. IUnityGateway.
        // This would mean this class can serve as sort of a 'composition root'.
        // There doesn't appear to be any benefit of Editor-assignability here for service components; no reason for assigning 'different versions', etc.

        // Editor-exposed field to assign the broadcasting container that will send the service to other components as an interface
        // All need to be tied to specific service component representations
        [SerializeField]
        private SoundServiceProperty_Writeable _soundServiceOut = default;

        // Nope
        //[Serializable]
        //public class CoupledEntity
        //{
        //    // Run assignability checks in OnValidate()?
        //    public Component Component;
        //    public ScriptableObject ScriptableObject;
        //}
    }

    // new name idea for submodule: ScriptableInject? sigh...

    public interface IUnityGateway
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        T GetComponent<T>();
    }

}