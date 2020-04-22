using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[Serializable]
public class Room
{
    public int cols;
    public int rows;

    public Door northDoor;
    public Door southDoor;
    public Door eastDoor;
    public Door westDoor;

    public Matrix<DungeonTileType> roomMatrix;

    public Room()
    {
    }

    public Room(int cols, int rows)
    {
        this.cols = cols;
        this.rows = rows;
        roomMatrix = new Matrix<DungeonTileType>(this.cols, this.rows);
        roomMatrix.FillMatrix((col, row) => DungeonTileType.Ground);
        northDoor = new Door
        {
            localPosition = new Vector2Int(this.GetRandomInt(1, cols - 2), rows - 1),
            orientation = CardinalPoints.North,
            parentRoom = this,
            neighbourRoom = null
        };
        roomMatrix.SetValue(DungeonTileType.Door, (int) northDoor.localPosition.x, (int) northDoor.localPosition.y);
        southDoor = new Door
        {
            localPosition = new Vector2Int(this.GetRandomInt(1, cols - 2), 0),
            orientation = CardinalPoints.South,
            parentRoom = this,
            neighbourRoom = null
        };
        roomMatrix.SetValue(DungeonTileType.Door, (int) southDoor.localPosition.x, (int) southDoor.localPosition.y);
        eastDoor = new Door
        {
            localPosition = new Vector2Int(cols - 1, this.GetRandomInt(1, rows - 2)),
            orientation = CardinalPoints.East,
            parentRoom = this,
            neighbourRoom = null
        };
        roomMatrix.SetValue(DungeonTileType.Door, (int) eastDoor.localPosition.x, (int) eastDoor.localPosition.y);
        westDoor = new Door
        {
            localPosition = new Vector2Int(0, this.GetRandomInt(1, rows - 2)),
            orientation = CardinalPoints.West,
            parentRoom = this,
            neighbourRoom = null
        };
        roomMatrix.SetValue(DungeonTileType.Door, (int) westDoor.localPosition.x, (int) westDoor.localPosition.y);
    }

    public List<Door> GetAvailableDoors()
    {
        return this.ListOf(northDoor, southDoor, eastDoor, westDoor).Where(x => x.neighbourRoom == null).ToList();
    }

    public Room Clone()
    {
        return new Room(this.cols, this.rows);
    }

    public static Room CreateRandomRoom()
    {
        var result = new Room {cols = 3, rows = 3};
        result.roomMatrix = new Matrix<DungeonTileType>(result.cols, result.rows);
        result.roomMatrix.FillMatrix((col, row) => DungeonTileType.Ground);
        result.northDoor = new Door
        {
            localPosition = new Vector2Int(result.GetRandomInt(0, result.cols - 1), 0),
            orientation = CardinalPoints.North,
            parentRoom = result
        };
        result.roomMatrix.SetValue(DungeonTileType.Door, (int) result.northDoor.localPosition.x,
            (int) result.northDoor.localPosition.y);
        result.southDoor = new Door
        {
            localPosition = new Vector2Int(result.GetRandomInt(0, result.cols - 1), result.rows - 1),
            orientation = CardinalPoints.South,
            parentRoom = result
        };
        result.roomMatrix.SetValue(DungeonTileType.Door, (int) result.southDoor.localPosition.x,
            (int) result.southDoor.localPosition.y);
        result.eastDoor = new Door
        {
            localPosition = new Vector2Int(result.cols - 1, result.GetRandomInt(0, result.rows - 1)),
            orientation = CardinalPoints.East,
            parentRoom = result
        };
        result.roomMatrix.SetValue(DungeonTileType.Door, (int) result.eastDoor.localPosition.x,
            (int) result.eastDoor.localPosition.y);
        result.westDoor = new Door
        {
            localPosition = new Vector2Int(0, result.GetRandomInt(0, result.rows - 1)),
            orientation = CardinalPoints.West,
            parentRoom = result
        };
        result.roomMatrix.SetValue(DungeonTileType.Door, (int) result.westDoor.localPosition.x,
            (int) result.westDoor.localPosition.y);

        return result;
    }
}