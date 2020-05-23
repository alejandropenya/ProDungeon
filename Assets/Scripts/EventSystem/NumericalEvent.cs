using UnityEngine;

namespace Utils.EventSystem
{
    [CreateAssetMenu(fileName = "NumericalEvent", menuName = "ProDungeon/Event/Numerical Event", order = 0)]
    public class NumericalEvent : CustomEvent
    {
        [SerializeField] private float count;

        public float VisibleCount => count;

        public static NumericalEvent CreateInstance(string eventName)
        {
            var result = CreateInstance<NumericalEvent>();
            result.name = eventName;
            result.count = 0;
            return result;
        }

        public override void TriggerEvent(float value)
        {
            count += value;
        }
        
        public override void ResetEvent()
        {
            count = 0;
        }
    }
}