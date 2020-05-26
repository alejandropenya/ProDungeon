using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utils.EventSystem;

namespace DungeonGenerator
{
    [CreateAssetMenu(fileName = "CustomDungeonModificator", menuName = "ProDungeon/Dungeon Modificator", order = 3)]
    public class DungeonModificator : ScriptableObject
    {
        [SerializeField] private List<CustomEvent> registeredEvents;

        public List<CustomEvent> RegisteredEvents => registeredEvents;

        [SerializeField] private float maxExpectedValue;
        public float MaxExpectedValue => maxExpectedValue;

        public float GetModificatorValue()
        {
            var result = 0f;
            var numericalEvents = registeredEvents.Where(currentEvent => currentEvent is NumericalEvent).Select(currentEvent => currentEvent as NumericalEvent).ToList();
            numericalEvents.ForEach(currentEvent => result += currentEvent.VisibleCount * currentEvent.weight);
            return result;
        }

        public void AddEvent(CustomEvent customEvent)
        {
            if(registeredEvents == null) registeredEvents = new List<CustomEvent>();
            registeredEvents.Add(customEvent);
        }
    }
}