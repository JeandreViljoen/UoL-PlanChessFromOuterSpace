using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanChess
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<T>(typeof(T).Name);
                }

                return instance;
            }
        }
    }
}

