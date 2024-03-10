using UnityEngine;

namespace Services
{
    /// <summary>
    /// References an external singleton object in the scene.
    /// </summary>
    /// <typeparam name="T">The service to reference.</typeparam>
    public struct EasyService<T> where T: Service
    {
        private EasyService(T service)
        {
            _value = service;
        }

        /// <summary>
        /// Force retrieval of the service object.
        /// </summary>
        public void ForceGetService()
        {
            _value = ServiceLocator.GetService<T>();
        }

        /// <summary>
        /// Check if the service named exists in the current scene.
        /// </summary>
        public bool HasService()
        {
            return IsAssigned || ServiceLocator.GetService<T>() != null;
        }
		
        public bool IsAssigned =>_value != null;

        /// <summary>
        /// Gets a reference to the service object.
        /// </summary>
        public T Value
        {
            get
            {
                if (_value == null)
                {
                    _value = ServiceLocator.GetService<T>();

#if UNITY_EDITOR
                    Debug.Assert(_value != null, $"[LazyService<{typeof(T)}>] Could not get service. " +
                                                 $"Either it does not exist in any loaded scene, or it has not been subscribed to the ServiceLocator. " +
                                                 $"This could happen if Value is called before OnEnable or the objet that holds the Service has been disabled");
#endif
                }
			
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        private T _value;

        public static implicit operator T(EasyService<T> easy)
        {
            return easy.Value;
        }

    }
}