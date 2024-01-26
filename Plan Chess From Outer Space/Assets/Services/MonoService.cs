using UnityEngine;

namespace Services
{
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