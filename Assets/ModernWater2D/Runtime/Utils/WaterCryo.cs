using System;
using UnityEngine;
using UnityEngine.Events;

namespace Water2D
{

    //used for variables that don't need to be updated every frame
    [System.Serializable]
    public class WaterCryo<T> where T : IEquatable<T>
    {
        [SerializeField]
        private T _value;

        [SerializeField]
        public T value
        {
            get { return _value; }
            set
            {
                T oldValue = _value;
                _value = value;
                if (!value.Equals(oldValue) && onValueChanged != null)
                    onValueChanged.Invoke();
            }
        }
        public UnityAction onValueChanged;

        public WaterCryo(T value, UnityAction onValueChanged)
        {
            this.value = value;
            this.onValueChanged = onValueChanged;
        }

        public WaterCryo(T value)
        {
            this.value = value;
        }

        public WaterCryo()
        {
        }

    }

}