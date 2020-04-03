using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public class DungeonGenerator : MonoBehaviour
    {
        [SerializeField] public int numberOfRooms;
        [SerializeField] private List<GameObject> currentObjects;

        private List<Room> currentRooms;
        private Matrix<(DungeonTileType, Room)> floorMatrix;

        [ContextMenu("test")]
        public void GenerateFloor()
        {
            InitializeFloor();

            for (var i = 0; i < numberOfRooms; i++)
            {
                var room = GetRandomRoom();
                var newPosition = GetAvailablePosition(room);
                AddRoom(room, newPosition);
            }

            PaintTest();
        }

        public void InitializeFloor()
        {
            currentObjects?.ForEach(DestroyImmediate);
            currentObjects = new List<GameObject>();
            currentRooms = new List<Room>();

            floorMatrix = new Matrix<(DungeonTileType, Room)>(200, 200);
            floorMatrix.FillMatrix((col, row) => (DungeonTileType.Empty, null));
        }

        private bool AddRoom(Room room, Vector2Int newPosition)
        {
            floorMatrix.SetSubMatrix(room.roomMatrix.Convert(type => (type, room)),
                newPosition.x, newPosition.y);
            room.northDoor.globalPosition = room.northDoor.localPosition + newPosition;
            room.southDoor.globalPosition = room.southDoor.localPosition + newPosition;
            room.westDoor.globalPosition = room.westDoor.localPosition + newPosition;
            room.eastDoor.globalPosition = room.eastDoor.localPosition + newPosition;
            currentRooms.Add(room);

            return numberOfRooms > currentRooms.Count;
        }

        private Room GetRandomRoom()
        {
            var room = RoomPool.roomList.GetRandom();
            room = room.Clone();
            return room;
        }

        public bool NextStep()
        {
            var room = GetRandomRoom();
            var result = AddRoom(room, GetAvailablePosition(room));
            PaintTest();
            return result;
        }

        private Vector2Int GetAvailablePosition(Room room)
        {
            if (currentRooms.Count == 0)
                return new Vector2Int(floorMatrix.Cols / 2, floorMatrix.Rows / 2);
            var dungeonRoom = currentRooms.Where(x => x.GetAvailableDoors().Any()).GetRandom();
            var dungeonRoomDoor = dungeonRoom.GetAvailableDoors().GetRandom();
            Door currentRoomDoor;

            var result = new Vector2Int(dungeonRoomDoor.globalPosition.x, dungeonRoomDoor.globalPosition.y);


            switch (dungeonRoomDoor.orientation)
            {
                case CardinalPoints.North:
                    currentRoomDoor = room.southDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x, -currentRoomDoor.localPosition.y + 1);
                    room.southDoor.neighbourRoom = dungeonRoom;
                    break;
                case CardinalPoints.East:
                    currentRoomDoor = room.westDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x + 1, -currentRoomDoor.localPosition.y);
                    room.westDoor.neighbourRoom = dungeonRoom;
                    break;
                case CardinalPoints.West:
                    currentRoomDoor = room.eastDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x - 1, -currentRoomDoor.localPosition.y);
                    room.eastDoor.neighbourRoom = dungeonRoom;
                    break;
                case CardinalPoints.South:
                    currentRoomDoor = room.northDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x, -currentRoomDoor.localPosition.y - 1);
                    room.northDoor.neighbourRoom = dungeonRoom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            dungeonRoomDoor.neighbourRoom = room;

            Debug.Log(dungeonRoomDoor.orientation.ToString() + ", " + dungeonRoomDoor.globalPosition.x + ", " +
                      dungeonRoomDoor.globalPosition.y);

            return result;
        }

        private void PaintTest()
        {
            this.floorMatrix.ForEach((col, row, item) =>
            {
                if (item.Item1 == DungeonTileType.Empty || item.Item1 == DungeonTileType.Hole)
                {
                    return;
                }

                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                if (item.Item1 == DungeonTileType.Door)
                {
                    cube.GetComponent<MeshRenderer>().material.color = Color.green;
                }

                ;

                currentObjects.Add(cube);
                cube.transform.SetParent(transform);
                cube.transform.position = new Vector3(col, 0, row);
                cube.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
            });
        }
    }
}