using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoomEditor
{
    public class ClickListener : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] public UnityEvent onClicked;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onClicked?.Invoke();
        }
    }
}