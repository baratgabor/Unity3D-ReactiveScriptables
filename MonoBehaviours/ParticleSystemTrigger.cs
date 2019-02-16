using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemTrigger : ComponentTriggerBase<ParticleSystem>
    {
        protected override void DoTrigger()
        {
            _component.Play();
        }
    }
}