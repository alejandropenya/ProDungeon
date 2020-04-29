﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utils.RoomEditor
{
    public class ClickListener : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private UnityEvent onClicked;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onClicked.Invoke();
        }
    }
}