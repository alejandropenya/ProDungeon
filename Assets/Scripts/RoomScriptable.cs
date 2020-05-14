using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace Utils
{
    [CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/Room", order = 1)]
    public class RoomScriptable : ScriptableObject, ISerializationCallbackReceiver
    {
        public int cols;
        public int rows;
        public Matrix<TileTypeScriptable> roomMatrix;
        public List<TileTypeScriptable> roomList;

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
            roomMatrix = new Matrix<TileTypeScriptable>(cols, rows);
            if(roomList == null) return;
            roomMatrix.FillMatrix((col, row) => roomList.GetOrDefault(row * cols + col));
        }
    }
}