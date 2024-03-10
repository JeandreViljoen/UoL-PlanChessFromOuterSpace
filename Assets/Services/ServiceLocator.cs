using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Manages all services in the current scene and exposes them to
    /// other scripts.
    /// </summary>
    public static class ServiceLocator
    {
        private static List<Service> _services = new List<Service>();

        public static T GetService<T>() where T : Service
        {
            foreach (Service service in _services)
            {
                if (typeof(T) == service.GetType())
                {
                    return (T) service;
                }
            }

            return default;
        }

        /// <summary>
        /// Add and expose a service.
        /// </summary>
        /// <param name="service">The service to add to the locater.</param>
        public static void AddService<T>(T service) where T : Service
        {
            foreach (Service existing in _services)
            {
                if (typeof(T) == existing.GetType())
                {
                    Debug.LogError(
                        $"[ServiceLocator] Cannot register multiple services of the same type: {typeof(T)}. Not registering duplicate.");
                    return;
                }
            }

            _services.Add(service);
        }

        /// <summary>
        /// Remove a service from the locator.
        /// </summary>
        /// <param name="service">The service to remove from the locater.</param>
        public static void RemoveService(Service service)
        {
            for (int i = 0; i < _services.Count; i++)
            {
                Service existing = _services[i];
                if (service.GetType() == existing.GetType())
                {
                    _services.RemoveAt(i);
                }
            }
        }
        
    }

}