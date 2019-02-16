using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables.Prototyping
{

    public interface IMonoBehaviourComponent
    {
        void MyOnEnable();
        void MyOnDisable();
        void MyOnAwake();
    }

    public abstract class CompositeMonoBehaviour : MonoBehaviour
    {
        // Expose component methods how exactly?

        private List<IMonoBehaviourComponent> _components = new List<IMonoBehaviourComponent>();

        private void Awake()
        {
            foreach (var c in _components)
                c.MyOnAwake();
        }

        private void OnEnable()
        {
            foreach (var c in _components)
                c.MyOnEnable();
        }

        private void OnDisable()
        {
            foreach (var c in _components)
                c.MyOnDisable();
        }
    }
}
