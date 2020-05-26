using System.Collections.Generic;
using DungeonGenerator;
using Extensions;
using UnityEngine;
using Utils;

public class Room : ScriptableObject, ISerializationCallbackReceiver
{
    public int cols;
    public int rows;

    public Door northDoor;
    public Door southDoor;
    public Door eastDoor;
    public Door westDoor;

    public Matrix<TileTypeScriptable> roomMatrix;

    public Room()
    {
    }

    public List<Door> GetAvailableDoors()
    {
        return this.ListOf(northDoor, southDoor, eastDoor, westDoor).Where(x => x.neighbourRoom == null).ToList();
    }

    public Room Clone()
    {
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

    public void OnBeforeSerialize()
    {
        this.ListOf(eastDoor, southDoor, westDoor, northDoor)
            .Where(door => door != null)
            .ForEach(door => door.parentRoom = null);
    }

    public void OnAfterDeserialize()
    {
        this.ListOf(eastDoor, southDoor, westDoor, northDoor)
            .Where(door => door != null)
            .ForEach(door => door.parentRoom = this);
    }
}