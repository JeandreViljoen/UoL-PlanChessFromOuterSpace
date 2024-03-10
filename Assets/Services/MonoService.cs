using UnityEngine;

namespace Services
{
    /// <summary>
    /// Represents a singleton script that can be invoked from
    /// everywhere else in the scene.
    /// </summary>
    public abstract class MonoService : MonoBehaviour, Service
    {
        private void OnEnable()
        {
            ServiceLocator.AddService(this);
        }

        private void OnDisable()
        {
            ServiceLocator.RemoveService(this);
        }
    }
}