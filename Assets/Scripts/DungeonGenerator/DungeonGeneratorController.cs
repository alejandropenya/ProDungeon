using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utils;

namespace DungeonGenerator
{
    public class DungeonGeneratorController : MonoBehaviour
    {
        [SerializeField] public int maxNumberOfRooms;
        [SerializeField] private List<GameObject> currentObjects;
        private List<Room> _alreadyCheckedRooms;

        private List<Room> _currentRooms;
        private Matrix<(TileTypeScriptable, Room)> _floorMatrix;

        public void InitializeFloor()
        {
            currentObjects?.ForEach(DestroyImmediate);
            currentObjects = new List<GameObject>();
            _currentRooms = new List<Room>();
            _alreadyCheckedRooms = new List<Room>();
        
            _floorMatrix = new Matrix<(TileTypeScriptable, Room)>(200, 200);
            _floorMatrix.FillMatrix((col, row) => (AssetDatabaseUtils.LoadAssetWithName<TileTypeScriptable>("Empty", "TileTypeScriptable"), null));
        
        }

        private void AddRoom(Room newRoom, Vector2Int newPosition, Door dungeonDoor)
        {
            _floorMatrix.SetSubMatrix(newRoom.roomMatrix.Convert(type => (type, room: newRoom)),
                newPosition.x, newPosition.y);
            newRoom.northDoor.globalPosition = newRoom.northDoor.localPosition + newPosition;
            newRoom.southDoor.globalPosition = newRoom.southDoor.localPosition + newPosition;
            newRoom.westDoor.globalPosition = newRoom.westDoor.localPosition + newPosition;
            newRoom.eastDoor.globalPosition = newRoom.eastDoor.localPosition + newPosition;
            _currentRooms.Add(newRoom);

            if (dungeonDoor == null) return;

            dungeonDoor.neighbourRoom = newRoom;
            switch (dungeonDoor.orientation)
            {
                case CardinalPoints.North:
                    newRoom.southDoor.neighbourRoom = dungeonDoor.parentRoom;
                    break;
                case CardinalPoints.East:
                    newRoom.westDoor.neighbourRoom = dungeonDoor.parentRoom;
                    break;
                case CardinalPoints.West:
                    newRoom.eastDoor.neighbourRoom = dungeonDoor.parentRoom;
                    break;
                case CardinalPoints.South:
                    newRoom.northDoor.neighbourRoom = dungeonDoor.parentRoom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool HasGenerationEnded()
        {
            return maxNumberOfRooms <= _currentRooms.Count;
        }

        private Room GetRandomRoom()
        {
            var room = RoomPool.roomList.Where(room1 => !_alreadyCheckedRooms.Contains(room1)).GetRandom();
            if (room == null) return null;
            _alreadyCheckedRooms.Add(room);
            room = room.Clone();
            return room;
        }

        public void NextStep()
        {
            var newPosition = new Vector2Int(-1, -1);
            Door door = null;
            Room room = null;
            while (newPosition == new Vector2Int(-1, -1))
            {
                room = GetRandomRoom();
                if (room == null)
                {
                    maxNumberOfRooms = _currentRooms.Count;
                    return;
                }
                var availablePosition = GetAvailablePosition(room);
                newPosition = availablePosition.Item1;
                door = availablePosition.Item2;
            }
            AddRoom(room, newPosition, door);
        }

        private (Vector2Int, Door) GetAvailablePosition(Room room)
        {
            if (_currentRooms.Count == 0)
                return (new Vector2Int(_floorMatrix.Cols / 2, _floorMatrix.Rows / 2), null);

            var dungeonRoomDoors = GetAvailableDoors();

            foreach (var dungeonDoor in dungeonRoomDoors)
            {
                var availablePosition = GetInitialPositionFromDoor(room, dungeonDoor);
                if (DoesRoomFit(room, availablePosition)) return (availablePosition, dungeonDoor);
            }

            return (new Vector2Int(-1, -1), null);
        }

        private List<Door> GetAvailableDoors()
        {
            return _currentRooms.Where(x => x.GetAvailableDoors().Any()).SelectMany((room => room.GetAvailableDoors()))
                .ToList();
        }

        private static Vector2Int GetInitialPositionFromDoor(Room newRoom, Door dungeonRoomDoor)
        {
            var result = new Vector2Int(dungeonRoomDoor.globalPosition.x, dungeonRoomDoor.globalPosition.y);
            Door currentRoomDoor;

            switch (dungeonRoomDoor.orientation)
            {
                case CardinalPoints.North:
                    currentRoomDoor = newRoom.southDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x, -currentRoomDoor.localPosition.y + 1);
                    break;
                case CardinalPoints.East:
                    currentRoomDoor = newRoom.westDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x + 1, -currentRoomDoor.localPosition.y);
                    break;
                case CardinalPoints.West:
                    currentRoomDoor = newRoom.eastDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x - 1, -currentRoomDoor.localPosition.y);
                    break;
                case CardinalPoints.South:
                    currentRoomDoor = newRoom.northDoor;
                    result += new Vector2Int(-currentRoomDoor.localPosition.x, -currentRoomDoor.localPosition.y - 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        private bool DoesRoomFit(Room room, Vector2Int initialPosition)
        {
            try
            {
                var spaceToCheck = _floorMatrix.GetSubMatrix(initialPosition.x,
                    initialPosition.y, room.cols, room.rows);
                var result = spaceToCheck.All((item, col, row) => item.Item2 == null);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void PaintTest()
        {
            this._floorMatrix.ForEach((col, row, item) =>
            {
           
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.GetComponent<MeshRenderer>().material.color = item.Item1.color;

                currentObjects.Add(cube);
                cube.transform.SetParent(transform);
                cube.transform.position = new Vector3(col, 0, row);
                cube.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
            });
        }
    }
}