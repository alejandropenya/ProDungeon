using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class RoomScriptable : ScriptableObject, ISerializationCallbackReceiver
    {
        public int cols;
        public int rows;
        public Matrix<DungeonTileType> roomMatrix;
        public List<DungeonTileType> roomList;

        public void OnBeforeSerialize()
        {
            if (roomMatrix == null)
            {
                return;
            }

            roomList = roomMatrix.ToList();
        }

        public void OnAfterDeserialize()
        {
            roomMatrix = new Matrix<DungeonTileType>(cols, rows);
            if(roomList == null) return;
            roomMatrix.FillMatrix((col, row) => roomList.GetOrDefault(row * cols + col));
        }
    }
}