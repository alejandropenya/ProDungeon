using System;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class Door : ScriptableObject 
    {
        public Room parentRoom;
        public Vector2Int localPosition;
        public Vector2Int globalPosition;
        public CardinalPoints orientation;
        public Room neighbourRoom;
        
        public Door Clone()
        {
            var resultDoor = ScriptableObject.CreateInstance<Door>();
            resultDoor.parentRoom = parentRoom;
            resultDoor.localPosition = localPosition;
            resultDoor.globalPosition = globalPosition;
            resultDoor.orientation = orientation;
            resultDoor.neighbourRoom = neighbourRoom;
            return resultDoor;
        }
    }
}