using UnityEngine;

namespace Services
{
    

    public struct EasyService<T> where T: Service
    {
        private EasyService(T service)
        {
            _value = service;
        }


        public void ForceGetService()
        {
            _value = ServiceLocator.GetService<T>();
        }

        public bool HasService()
        {
            return IsAssigned || ServiceLocator.GetService<T>() != null;
        }
		
        public bool IsAssigned =>_value != null;

        private T _value;
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
		
        public static implicit operator T(EasyService<T> easy)
        {
            return easy.Value;
        }

    }
}