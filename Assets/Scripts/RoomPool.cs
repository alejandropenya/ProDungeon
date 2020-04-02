using System.Collections.Generic;

namespace Utils
{
    public static class RoomPool
    {
        public static List<Room> roomList = new List<Room>()
        {
            new Room(3, 5), new Room(3, 3), new Room(5, 5), new Room(6, 3), new Room(4, 7), new Room(9, 9),
            new Room(25, 20), new Room(10, 14), new Room(3,4), new Room(3,6), new Room(3,10), new Room(4,9), 
            new Room(4,15), new Room(7,18), new Room(7,8), new Room(7,9), new Room(9,19), new Room(10, 15),
            new Room(18,18), new Room(12,12), new Room(20,20), new Room(6,6), new Room(7,7), new Room(8,8),
            new Room(9,9), new Room(10,10), new Room(11,11), new Room(13,13), new Room(14,14), new Room(15,15),
            new Room(16,16), new Room(17,17), new Room(18,18), new Room(19,19), new Room(21,21)
        };
    }
}