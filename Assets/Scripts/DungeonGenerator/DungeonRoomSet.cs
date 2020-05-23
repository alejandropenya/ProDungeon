using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace DungeonGenerator
{
    [CreateAssetMenu(fileName = "NewDungeonRoomSet", menuName = "ProDungeon/Dungeon Room Set", order = 1)]
    public class DungeonRoomSet : ScriptableObject
    {
        [SerializeField] private List<RoomScriptable> roomSet;

        public List<RoomScriptable> RoomSet => roomSet.CloneList();
    }
}