using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Utils;

namespace DungeonGenerator
{
    public class DungeonGeneratorController : MonoBehaviour
    {
        [SerializeField] private int minNumberOfRooms;
        [SerializeField] private int maxNumberOfRooms;

        [SerializeField] private bool canRoomsBeRepeated;
        [SerializeField] private bool hasGenerationEnded;

        [SerializeField] private List<GameObject> currentObjects;
        [SerializeField] private TileTypeScriptable emptyTile;

        //Size of the Rooms
        [SerializeField] private DungeonModificator roomSizeModificator;
        //Room Setup
        [SerializeField] private DungeonModificator verticalShapeModificator;
        //Number of Rooms
        [SerializeField] private DungeonModificator roomCountModificator;

        private int variance;
        private List<Room> _alreadyCheckedRooms;
        private List<Room> _availableRooms;
        private List<Room> _currentRooms;
        private Matrix<(TileTypeScriptable, Room)> _floorMatrix;
        public bool HasGenerationEnded => hasGenerationEnded;

        public void InitializeFloor(DungeonRoomSet roomSet, int minRooms, int maxRooms, int variance)
        {
            hasGenerationEnded = false;
            this.minNumberOfRooms = minRooms;
            this.maxNumberOfRooms = maxRooms;
            this.variance = variance;
            currentObjects?.ForEach(DestroyImmediate);
            currentObjects = new List<GameObject>();
            _currentRooms = new List<Room>();
            _alreadyCheckedRooms = new List<Room>();
            _availableRooms = roomSet.RoomSet.Select(roomS => roomS.ToRoom()).ToList();

            _floorMatrix = new Matrix<(TileTypeScriptable, Room)>(200, 200);
            _floorMatrix.FillMatrix((col, row) => (emptyTile, null));
        }

        public IEnumerator GenerateDungeon()
        {
            var countWeight = roomSizeModificator.GetModificatorValue() / roomSizeModificator.MaxExpectedValue;
            countWeight = Mathf.Clamp(countWeight, 0, 1) * 2 - 1;
            // -1 --> 1
            countWeight *= variance;
            // -variance --> + variance
            var desiredNumberOfRooms = Mathf.RoundToInt(this.GetRandomInt(minNumberOfRooms, maxNumberOfRooms) + countWeight);
            
            for (var i = 0; i < desiredNumberOfRooms; i++)
            {
                yield return null;
                if (!NextStep()) break;
            }
            hasGenerationEnded = true;
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

        private Room GetRandomRoom()
        {
            var sizeWeight = roomSizeModificator.GetModificatorValue() / roomSizeModificator.MaxExpectedValue;
            sizeWeight = Mathf.Clamp(sizeWeight, 0, 1);


            var availableRooms = _availableRooms.OrderByAscending(x => x.cols * x.rows);

            if (!canRoomsBeRepeated)
            {
                availableRooms = availableRooms
                    .Where(room1 => !_alreadyCheckedRooms.Contains(room1))
                    .ToList();
            }

            var midRoomIndex = (int) ((availableRooms.Count - 1) * sizeWeight);

            var maxDifference = Mathf.Max(midRoomIndex - 1, availableRooms.Count - midRoomIndex);

            var room = availableRooms.GetRandomBiased(projection =>
            {
                var currentDistance = Mathf.Abs(midRoomIndex - availableRooms.IndexOf(projection));
                return 1 - (currentDistance * 0.95f / maxDifference);
            });

            if (!room) return null;
            _alreadyCheckedRooms.Add(room);
            room = room.Clone();
            return room;
        }

        public bool NextStep()
        {
            var newPosition = new Vector2Int(-1, -1);
            Door door = null;
            Room room = null;
            while (newPosition == new Vector2Int(-1, -1))
            {
                room = GetRandomRoom();
                if (room == null)
                {
                    hasGenerationEnded = true;
                    return false;
                }

                var availablePosition = GetAvailablePosition(room);
                newPosition = availablePosition.Item1;
                door = availablePosition.Item2;
            }

            AddRoom(room, newPosition, door);
            return true;
        }

        private (Vector2Int, Door) GetAvailablePosition(Room room)
        {
            if (_currentRooms.Count == 0)
                return (new Vector2Int(_floorMatrix.Cols / 2, _floorMatrix.Rows / 2), null);

            var verticalWeight = verticalShapeModificator.GetModificatorValue() /
                                 verticalShapeModificator.MaxExpectedValue;
            verticalWeight = Mathf.Clamp(verticalWeight, 0, 1);

            var disorderedDoors = GetAvailableDoors();
            var orderedDoors = new List<Door>();

            while (disorderedDoors.Any())
            {
                var selectedDoor = disorderedDoors.GetRandomBiased(projection =>
                {
                    if (projection.orientation == CardinalPoints.North ||
                        projection.orientation == CardinalPoints.South)
                    {
                        // This is wrong as fuck
                        return verticalWeight > 0.5f ? 2 : 1;
                    }
                    else
                    {
                        // Wrong mathematics go brrrrrr
                        return verticalWeight > 0.5f ? 1 : 2;
                    }
                });
                orderedDoors.Add(selectedDoor);
                disorderedDoors.Remove(selectedDoor);
            }

            foreach (var dungeonDoor in orderedDoors)
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