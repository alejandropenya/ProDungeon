using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class Door
    {
        public Room parentRoom;
        public Vector2Int localPosition;
        public Vector2Int globalPosition;
        public CardinalPoints orientation;
        public Room neighbourRoom;
    }
}