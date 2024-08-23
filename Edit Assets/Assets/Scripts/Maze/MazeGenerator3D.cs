using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MazeGenerator3D : MonoBehaviour
{
    class Room
    {
        public BoundsInt bounds;
        public Room(Vector3Int location, Vector3Int size)
        {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }
    [SerializeField]
    Vector3Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector3Int roomMinSize;
    [SerializeField]
    Vector3Int roomMaxSize;
    [SerializeField]
    GameObject cubePrefab;

    [Header("my stuff")]
    [SerializeField] private GameObject RoomGeneratorPrefab;
    [SerializeField] private GameObject CorridorPlaceholderPrefab;


    Random random;
    //Grid3D<CellType> grid;
    List<Room> rooms;
    List<Room> roomsOnFloor;
    //Delaunay3D delaunay;
    //HashSet<Prim.Edge> selectedEdges;
    void Start()
    {
        random = new Random(0);
        //grid = new Grid3D<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();
        roomsOnFloor = new List<Room>();

        PlaceRooms();
        //Triangulate();
        //CreateHallways();
        //PathfindHallways();
    }

    void PlaceRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            Vector3Int roomSize = new Vector3Int(
                random.Next(roomMinSize.x, roomMaxSize.x + 1),
                random.Next(roomMinSize.y, roomMaxSize.y + 1),
                random.Next(roomMinSize.z, roomMaxSize.z + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, -1, -1), roomSize + new Vector3Int(2, 1, 2));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z)
            {
                add = false;
            }

            if (add)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    //grid[pos] = CellType.Room;
                }
            }
        }

        //PlaceCorridorOnEachFloor();
    }
    void PlaceRoom(Vector3Int location, Vector3Int size)
    {
        PlaceCube(location, size);
    }
    void PlaceCube(Vector3Int location, Vector3Int size)
    {
        GameObject go = Instantiate(cubePrefab, transform.position + location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = size;
        Vector3 roomLocation = location * 6;
        int offsetX = (int)roomLocation.x + (size.x - 1) * -3;
        int offsetZ = (int)roomLocation.z + (size.z - 1) * -3;
        GameObject Room = Instantiate(RoomGeneratorPrefab,
            transform.position + roomLocation /*+ new Vector3(offsetX, roomLocation.y - 3, offsetZ)*/,
            Quaternion.identity, transform);
        //Room.GetComponent<RoomGenerator>().roomWidth = size.x;
        //Room.GetComponent<RoomGenerator>().roomDepth = size.z;
        //Room.GetComponent<RoomGenerator>().GenerateRoom();
    }
    void PlaceCorridorOnEachFloor()
    {
        roomsOnFloor = new List<Room>();
        for (int i = 0; i < size.y; i++)
        {
            for (int x = 0; x < rooms.Count; x++)
            {
                if (rooms[x].bounds.position.y == 0)
                {
                    roomsOnFloor.Add(rooms[x]);
                }
            }
        }
        MakeCorridor();
    }
    void MakeCorridor()
    {
        int minDistance = 10000;
        Room roomFrom = roomsOnFloor[0];
        Room roomTo = null;
        for (int i = 1; i < roomsOnFloor.Count; i++)
        {
            int distance = (int)Vector3.Distance(roomFrom.bounds.position, roomsOnFloor[i].bounds.position);
            Debug.Log(distance);
            if (distance < minDistance)
            {
                minDistance = (int)Vector3.Distance(roomsOnFloor[0].bounds.position, roomsOnFloor[i].bounds.position);
                roomTo = roomsOnFloor[i];
                Debug.Log(minDistance);
            }
        }
        Instantiate(CorridorPlaceholderPrefab, transform.position + (roomFrom.bounds.position) * 6, Quaternion.identity, transform);
        Instantiate(CorridorPlaceholderPrefab, transform.position + (roomTo.bounds.position) * 6, Quaternion.Euler(0, 10, 0), transform);
        roomsOnFloor.Clear();

        if (roomTo != null)
        {
            if (roomFrom.bounds.x < roomTo.bounds.x)
            {
                Instantiate(CorridorPlaceholderPrefab, transform.position + (roomFrom.bounds.position + Vector3.right) * 6, Quaternion.identity, transform);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
    }
}
