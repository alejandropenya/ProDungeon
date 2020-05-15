using System.Collections.Generic;
using DungeonGenerator;
using Extensions;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/Room", order = 1)]
public class RoomScriptable : ScriptableObject, ISerializationCallbackReceiver
{
    public int cols;
    public int rows;

    public Door northDoor;
    public Door southDoor;
    public Door eastDoor;
    public Door westDoor;

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
        if (roomList == null) return;
        roomMatrix.FillMatrix((col, row) => roomList.GetOrDefault(row * cols + col));
    }

    public Room ToRoom()
    {
        OnAfterDeserialize();
        PopulateDoors();
        var roomResult = ScriptableObject.CreateInstance<Room>();
        roomResult.cols = cols;
        roomResult.rows = rows;
        roomResult.eastDoor = eastDoor.Clone();
        roomResult.westDoor = westDoor.Clone();
        roomResult.northDoor = northDoor.Clone();
        roomResult.southDoor = southDoor.Clone();
        roomResult.roomMatrix = roomMatrix.CloneMatrix();
        return roomResult;
    }

    private void PopulateDoors()
    {
        var doorTileType = AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable");
            
        var northDoorIndex = roomMatrix.GetRow(0).IndexOf(doorTileType);
        if (northDoorIndex != -1)
        {
            northDoor = ScriptableObject.CreateInstance<Door>();
            northDoor.orientation = CardinalPoints.North;
            northDoor.localPosition = new Vector2Int(northDoorIndex, 0);
        }

        var westDoorIndex = roomMatrix.GetColumn(0).IndexOf(doorTileType);
        if (westDoorIndex != -1)
        {
            westDoor = ScriptableObject.CreateInstance<Door>();
            westDoor.orientation = CardinalPoints.West;
            westDoor.localPosition = new Vector2Int(0, westDoorIndex);
        }

        var eastDoorIndex = roomMatrix.GetColumn(cols - 1).IndexOf(doorTileType);
        if (eastDoorIndex != -1)
        {
            eastDoor = ScriptableObject.CreateInstance<Door>();
            eastDoor.orientation = CardinalPoints.East;
            eastDoor.localPosition = new Vector2Int(cols - 1, eastDoorIndex);
        }

        var southDoorIndex = roomMatrix.GetRow(rows - 1).IndexOf(doorTileType);
        if (southDoorIndex != -1)
        {
            southDoor = ScriptableObject.CreateInstance<Door>();
            southDoor.orientation = CardinalPoints.South;
            southDoor.localPosition = new Vector2Int(southDoorIndex, rows - 1);
        }
    }
}