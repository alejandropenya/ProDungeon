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

    public Room(int cols, int rows)
    {
        this.cols = cols;
        this.rows = rows;
        roomMatrix = new Matrix<TileTypeScriptable>(this.cols, this.rows);
        roomMatrix.FillMatrix((col, row) =>
            AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Ground", "TileTypeScriptable"));

        northDoor = ScriptableObject.CreateInstance<Door>();
        northDoor.localPosition = new Vector2Int(this.GetRandomInt(1, cols - 2), rows - 1);
        northDoor.orientation = CardinalPoints.North;
        northDoor.parentRoom = this;
        northDoor.neighbourRoom = null;
        roomMatrix.SetValue(AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            northDoor.localPosition.x, northDoor.localPosition.y);

        southDoor = ScriptableObject.CreateInstance<Door>();
        southDoor.localPosition = new Vector2Int(this.GetRandomInt(1, cols - 2), 0);
        southDoor.orientation = CardinalPoints.South;
        southDoor.parentRoom = this;
        southDoor.neighbourRoom = null;
        roomMatrix.SetValue(AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            southDoor.localPosition.x, southDoor.localPosition.y);

        eastDoor = ScriptableObject.CreateInstance<Door>();
        eastDoor.localPosition = new Vector2Int(cols - 1, this.GetRandomInt(1, rows - 2));
        eastDoor.orientation = CardinalPoints.East;
        eastDoor.parentRoom = this;
        eastDoor.neighbourRoom = null;
        roomMatrix.SetValue(AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            eastDoor.localPosition.x, eastDoor.localPosition.y);

        westDoor = ScriptableObject.CreateInstance<Door>();
        westDoor.localPosition = new Vector2Int(0, this.GetRandomInt(1, rows - 2));
        westDoor.orientation = CardinalPoints.West;
        westDoor.parentRoom = this;
        westDoor.neighbourRoom = null;
        roomMatrix.SetValue(AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            westDoor.localPosition.x, westDoor.localPosition.y);
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

    public static Room CreateRandomRoom()
    {
        var result = ScriptableObject.CreateInstance<Room>();
        result.cols = 3;
        result.rows = 3;
        result.roomMatrix = new Matrix<TileTypeScriptable>(result.cols, result.rows);
        result.roomMatrix.FillMatrix((col, row) =>
            AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Ground", "TileTypeScriptable"));
        result.northDoor = ScriptableObject.CreateInstance<Door>();
        result.northDoor.localPosition = new Vector2Int(result.GetRandomInt(0, result.cols - 1), 0);
        result.northDoor.orientation = CardinalPoints.North;
        result.northDoor.parentRoom = result;
        result.roomMatrix.SetValue(
            AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            result.northDoor.localPosition.x,
            result.northDoor.localPosition.y);

        result.southDoor = ScriptableObject.CreateInstance<Door>();
        result.southDoor.localPosition = new Vector2Int(result.GetRandomInt(0, result.cols - 1), result.rows - 1);
        result.southDoor.orientation = CardinalPoints.South;
        result.southDoor.parentRoom = result;
        result.roomMatrix.SetValue(
            AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            result.southDoor.localPosition.x,
            result.southDoor.localPosition.y);

        result.eastDoor = ScriptableObject.CreateInstance<Door>();
        result.eastDoor.localPosition = new Vector2Int(result.cols - 1, result.GetRandomInt(0, result.rows - 1));
        result.eastDoor.orientation = CardinalPoints.East;
        result.eastDoor.parentRoom = result;
        result.roomMatrix.SetValue(
            AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            result.eastDoor.localPosition.x,
            result.eastDoor.localPosition.y);

        result.westDoor = ScriptableObject.CreateInstance<Door>();
        result.westDoor.localPosition = new Vector2Int(0, result.GetRandomInt(0, result.rows - 1));
        result.westDoor.orientation = CardinalPoints.West;
        result.westDoor.parentRoom = result;
        result.roomMatrix.SetValue(
            AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Door", "TileTypeScriptable"),
            result.westDoor.localPosition.x,
            result.westDoor.localPosition.y);

        return result;
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