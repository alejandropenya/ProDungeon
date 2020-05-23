using System.Collections.Generic;
using UnityEngine;

namespace Utils.EventSystem
{
    public abstract class CustomEvent : ScriptableObject
    {
        public static List<CustomEvent> currentEvents = new List<CustomEvent>();
        public abstract void TriggerEvent(float value);

        protected virtual void OnEnable()
        {
            currentEvents.Add(this);
        }

        public abstract void ResetEvent();
    }
}