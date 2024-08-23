using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell[] MazeCellPrefab;
    public MazeCell FarthestCell;
    [SerializeField] private BossRoom BossRoomPrefab;
    [SerializeField] private TreasureRoom TreasureRoomPrefab;
    [SerializeField] Vector3Int mazeSize; //width, height, depth
    private MazeCell[,,] mazeGrid;
    [SerializeField] private float cellSize;
    private int distanceToZero;

    [Header("Rooms")]
    [SerializeField] private bool generateWithRooms;
    [SerializeField] private int roomAttempts;
    [SerializeField] private Room MazeRoomPrefab;
    [SerializeField] private MazeCell CornerPrefabLU;
    [SerializeField] private MazeCell CornerPrefabLO;
    [SerializeField] private MazeCell CornerPrefabRU;
    [SerializeField] private MazeCell CornerPrefabRO;
    [SerializeField] private MazeCell WallPrefab;
    [SerializeField] private MazeCell MiddlePrefab;
    [SerializeField] Vector2Int roomMinSize;
    [SerializeField] Vector2Int roomMaxSize;
    [SerializeField] private List<Room> Rooms = new List<Room>();

    [Header("Empty Space")]
    [SerializeField] private bool generateWithEmpty;
    [SerializeField] private int emptyAttempts;
    [SerializeField] private MazeCell EmptySpacePrefab;
    [SerializeField] Vector2Int emptyMinSize;
    [SerializeField] Vector2Int emptyMaxSize;

    [Header("Staircase")]
    [SerializeField] private bool generateWithStairs;
    [SerializeField] private int stairsAttempts;
    [SerializeField] private Staircase MazeStaircasePrefab;
    [SerializeField] private MazeCell StairsEntrancePrefab;
    [SerializeField] private MazeCell StairsPrefab;
    [SerializeField] private MazeCell StairsPlaceholder;
    [SerializeField] private int maxStairsAmount;
    private int stairsAmount;
    [SerializeField] private List<Staircase> Staircases = new List<Staircase>();

    public bool isValid;
    [SerializeField] private List<MazeCell> Entrances;
    public Vector3Int testVector = new Vector3Int();
    

    IEnumerator Start()
    {
        isValid = true;
        cellSize *= transform.localScale.x;
        mazeGrid = new MazeCell[
            mazeSize.x, 
            mazeSize.y * 2, 
            mazeSize.z];
        if (generateWithStairs)
        {
            CreateStaircase();
            Debug.Log("Stairs generated");
        }
        if (generateWithRooms)
        {
            GenerateRooms();
            Debug.Log("Rooms generated");
            //yield return GenerateRooms();
        }
        if (generateWithEmpty)
        {
            GenerateEmpty();
            Debug.Log("Empty Spaces generated");
            //yield return GenerateEmpty();
        }
        mazeGrid[0, 0, 0] = Instantiate(
            MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
            new Vector3( 0, 0, 0) + transform.position,
            Quaternion.identity, transform
            );
        mazeGrid[0, 0, 0].SetIndex(0, 0, 0);
        mazeGrid[0, 0, 0].SetEntrance();
        mazeGrid[0, 0, 0].SetType(MazeCell.CellType.Entrance);
        Entrances.Add(mazeGrid[0, 0, 0]);
        BeginToConnectEntrances();
        for (int y = 0; y < mazeSize.y; y++)
        {
            for (int x = 0; x < mazeSize.x; x++)
            {
                for (int z = 0; z < mazeSize.z; z++)
                {
                    if (mazeGrid[x, y, z] == null)
                    {
                        mazeGrid[x, y , z] = Instantiate(
                            MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                            new Vector3(
                                x * cellSize, 
                                y * cellSize * 2, 
                                z * cellSize) + transform.position,
                            Quaternion.identity, transform
                            );
                        mazeGrid[x, y, z].SetIndex(x, y, z);
                        mazeGrid[x, y, z].SetType(MazeCell.CellType.Hallway);
                    }
                }
            }
        }
        for (int y = 0; y < mazeSize.y; y++)
        {
            yield return GenerateMaze(null, mazeGrid[0, y, 0]);
        }

        Debug.Log("Maze generated");
        GenerateBossRoom();
        GenerateTreasureRoom();
        CheckEntrancesForVisits();
        if (!isValid)
        {
            Debug.LogWarning("Maze not valid");
        }
    }
    private void GenerateRooms()
    {
        int roomsCount = 0;
        for (int mazeHight = 0; mazeHight < mazeSize.y; mazeHight++)
        {
            for (int i = 0; i < roomAttempts; i++)
            {
                Vector3Int roomSize = new Vector3Int(
                    Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                    1,
                    Random.Range(roomMinSize.y, roomMaxSize.y) + 1);
                Vector3Int roomLocation = new Vector3Int(
                    Random.Range(0, mazeSize.x - roomSize.x),
                    mazeHight,
                    Random.Range(0, mazeSize.z - roomSize.z));
                if (roomLocation.x < 2 && roomLocation.z < 2)
                {
                    roomLocation.x += 2;
                    roomLocation.z += 2;
                }
                if (!CheckOverlapping(roomLocation + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2), MazeCell.CellType.None))
                {
                    Room room = Instantiate(MazeRoomPrefab,
                        new Vector3(
                            (roomLocation.x + roomSize.x / 2) * cellSize,
                            (roomLocation.y + roomSize.y / 2) * cellSize * 2,
                            (roomLocation.z + roomSize.z / 2) * cellSize)
                        + transform.position, Quaternion.identity, transform);
                    Rooms.Add(room);
                    room.roomPosition = roomLocation;
                    room.roomSize = roomSize;
                    GenerateRoomTiles(roomLocation, roomSize, roomsCount);
                    roomsCount++;
                }
                //yield return new WaitForSeconds(0.1f);
            }
        }
        GenerateRoomEntrances();
    }
    private void GenerateRoomTiles(Vector3Int location, Vector3Int size, int roomCount)
    {
        for (int xStep = 0; xStep < size.x; xStep++)
        {
            for (int zStep = 0; zStep < size.z; zStep++)
            {
                //corner l-u
                if (xStep == 0 && zStep == 0)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabLU,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 90, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //corner l-o
                if (xStep == 0 && zStep == size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabLO,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 180, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //corner r-u
                if (xStep == size.x - 1 && zStep == 0)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabRU,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 0, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //corner r-o
                if (xStep == size.x - 1 && zStep == size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabRO,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 270, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall left side
                if (xStep == 0 && zStep > 0 && zStep < size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 90, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall right side
                if (xStep == size.x - 1 && zStep > 0 && zStep < size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 270, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall bottom side
                if (xStep > 0 && xStep < size.x - 1 && zStep == 0)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 0, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall top side
                if (xStep > 0 && xStep < size.x - 1 && zStep == size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 180, 0), transform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //center pieces
                if (xStep > 0 && xStep < size.x - 1 && zStep > 0 && zStep < size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(MiddlePrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.identity, transform);
                }
                mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
                mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
            }
        }
    }
    private void GenerateRoomEntrances()
    {
        MazeCell entrance = null;
        for (int i = 0; i < Rooms.Count; i++)
        {
            for (int entranceAmount = 0; entranceAmount < 2; entranceAmount++)
            {
                entrance = Rooms[i].GetRandomBorderCell(entrance);
                mazeGrid[entrance.Index.x, entrance.Index.y, entrance.Index.z].SetEntrance();
                Entrances.Add(mazeGrid[entrance.Index.x, entrance.Index.y, entrance.Index.z]);
            }
        }
    }
    private void GenerateEmpty()
    {
        emptyAttempts /= mazeSize.y;
        for (int mazeHight = 0; mazeHight < mazeSize.y; mazeHight++)
        {
            for (int i = 0; i < emptyAttempts; i++)
            {
                Vector3Int emptySize = new Vector3Int(Random.Range(emptyMinSize.x, emptyMaxSize.x + 1),
                    1,
                    Random.Range(emptyMinSize.y, emptyMaxSize.y + 1));
                Vector3Int emptyLocation = new Vector3Int(
                    Random.Range(1, mazeSize.x - emptySize.x),
                    mazeHight,
                    Random.Range(0, mazeSize.z - emptySize.z));
                if (emptyLocation.x < 2 && emptyLocation.z < 2)
                {
                    emptyLocation.x += 2;
                    emptyLocation.z += 2;
                }
                if (!CheckOverlapping(emptyLocation + new Vector3Int(-1, 0, -1), emptySize + new Vector3Int(2, 0, 2), MazeCell.CellType.EmptySpace))
                {
                    for (int x = 0; x < emptySize.x; x++)
                    {
                        for (int z = 0; z < emptySize.z; z++)
                        {
                            if (mazeGrid[emptyLocation.x + x, emptyLocation.y, emptyLocation.z + z] == null)
                            {
                                mazeGrid[emptyLocation.x + x, emptyLocation.y, emptyLocation.z + z] = Instantiate(EmptySpacePrefab,
                                    new Vector3((emptyLocation.x + x) * cellSize, emptyLocation.y * cellSize * 2, (emptyLocation.z + z) * cellSize) + transform.position,
                                    Quaternion.identity, transform);
                                mazeGrid[emptyLocation.x + x, emptyLocation.y, emptyLocation.z + z].SetIndex(emptyLocation.x + x, emptyLocation.y, emptyLocation.z + z);
                                mazeGrid[emptyLocation.x + x, emptyLocation.y, emptyLocation.z + z].SetType(MazeCell.CellType.EmptySpace);
                                mazeGrid[emptyLocation.x + x, emptyLocation.y, emptyLocation.z + z].Visit();
                            }
                        }
                    }
                }
                //yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private void CreateStaircase()
    {
        for (int mazeHight = 0; mazeHight < mazeSize.y - 1; mazeHight++)
        {
            stairsAmount = 0;
            for (int i = 0; i < stairsAttempts; i++)
            {
                Vector3Int stairsLocation = new Vector3Int(
                    Random.Range(1, mazeSize.x - 5),
                    mazeHight,
                    Random.Range(1, mazeSize.z - 5));
                int stairsVariant = Random.Range(0, 5);
                Vector3Int stairsSize = new Vector3Int();
                if (stairsVariant == 0 || stairsVariant == 2)
                {
                    stairsSize = new Vector3Int(1, 2, 5);
                }
                if (stairsVariant == 1 || stairsVariant == 3)
                {
                    stairsSize = new Vector3Int(5, 2, 1);
                }
                if (!CheckOverlapping(stairsLocation, stairsSize, MazeCell.CellType.None))
                {
                    Staircase staircase = Instantiate(MazeStaircasePrefab,
                        new Vector3(
                            (stairsLocation.x) * cellSize,
                            (stairsLocation.y) * cellSize * 2,
                            (stairsLocation.z) * cellSize)
                        + transform.position, Quaternion.identity, transform);
                    Staircases.Add(staircase);
                    staircase.staircasePosition = stairsLocation;
                    staircase.staircaseSize = stairsSize;
                    GenerateStairTiles(stairsLocation, stairsSize, stairsVariant);
                    stairsAmount++;
                }
                if (stairsAmount == maxStairsAmount)
                {
                    break;
                }
            }
        }
    }
    private void GenerateStairTiles(Vector3Int location, Vector3Int size, int variant)
    {
        CreateStairsEntrances(location, variant);
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    if (mazeGrid[location.x + x, location.y + y, location.z + z] == null)
                    {
                        mazeGrid[location.x + x, location.y + y, location.z + z] = Instantiate(StairsPlaceholder,
                            new Vector3((location.x + x) * cellSize, (location.y + y) * cellSize * 2, (location.z + z) * cellSize) + transform.position,
                            Quaternion.identity, transform);
                        mazeGrid[location.x + x, location.y + y, location.z + z].SetIndex(location.x + x, y, location.y + z);
                        mazeGrid[location.x + x, location.y + y, location.z + z].SetType(MazeCell.CellType.Stairs);
                        mazeGrid[location.x + x, location.y + y, location.z + z].Visit();
                        //why is that here?
                        //mazeGrid[location.x + x, location.y + y, location.z + z].GetComponent<MazeCell>().distanceToSpawn = variant;
                    }
                }
            }
        }
    }
    private void CreateStairsEntrances(Vector3Int location, int stairsVariant)
    {
        Vector3Int entrance1Location;
        Vector3Int entrance2Location;
        Vector3Int stairsLocation;
        if (stairsVariant == 0)
        {
            entrance1Location = new Vector3Int(location.x, location.y, location.z + 4);
            entrance2Location = new Vector3Int(location.x, location.y + 1, location.z);
            stairsLocation = new Vector3Int(location.x, location.y, location.z + 3);

            InstantiateStairsEntrance(entrance1Location, 0);
            InstantiateStairsEntrance(entrance2Location, 2);
            InstantiateStairs(stairsLocation, stairsVariant);
        }
        if (stairsVariant == 1)
        {
            entrance1Location = new Vector3Int(location.x + 4, location.y, location.z);
            entrance2Location = new Vector3Int(location.x, location.y + 1, location.z);
            stairsLocation = new Vector3Int(location.x + 3, location.y, location.z);

            InstantiateStairsEntrance(entrance1Location, 1);
            InstantiateStairsEntrance(entrance2Location, 3);
            InstantiateStairs(stairsLocation, stairsVariant);
        }
        if (stairsVariant == 2)
        {
            entrance1Location = new Vector3Int(location.x, location.y, location.z);
            entrance2Location = new Vector3Int(location.x, location.y + 1, location.z + 4);
            stairsLocation = new Vector3Int(location.x, location.y, location.z + 1);

            InstantiateStairsEntrance(entrance1Location, 2);
            InstantiateStairsEntrance(entrance2Location, 0);
            InstantiateStairs(stairsLocation, stairsVariant);
        }
        if (stairsVariant == 3)
        {
            entrance1Location = new Vector3Int(location.x, location.y, location.z);
            entrance2Location = new Vector3Int(location.x + 4, location.y + 1, location.z);
            stairsLocation = new Vector3Int(location.x + 1, location.y, location.z);

            InstantiateStairsEntrance(entrance1Location, 3);
            InstantiateStairsEntrance(entrance2Location, 1);
            InstantiateStairs(stairsLocation, stairsVariant);
        }
    }
    private void InstantiateStairsEntrance(Vector3Int entranceLocation, int clearWall)
    {
        MazeCell mazeCell = Instantiate(StairsEntrancePrefab,
            new Vector3((entranceLocation.x) * cellSize, entranceLocation.y * cellSize * 2, (entranceLocation.z) * cellSize) + transform.position,
            Quaternion.identity, transform);
        mazeGrid[entranceLocation.x, entranceLocation.y, entranceLocation.z] = mazeCell;
        mazeCell.SetEntrance();
        mazeCell.SetIndex(entranceLocation.x, entranceLocation.y, entranceLocation.z);
        Entrances.Add(mazeCell);
        if (clearWall == 0)
        {
            mazeCell.ClearBackWall();
        }
        if (clearWall == 1)
        {
            mazeCell.ClearLeftWall();
        }
        if (clearWall == 2)
        {
            mazeCell.ClearFrontWall();
        }
        if (clearWall == 3)
        {
            mazeCell.ClearRightWall();
        }
    }
    private void InstantiateStairs(Vector3Int location, int variant)
    {
        MazeCell mazeCell = Instantiate(StairsPrefab,
            new Vector3((location.x) * cellSize, location.y * cellSize * 2, (location.z) * cellSize) + transform.position,
            Quaternion.Euler(0, 90 * variant, 0), transform);
        mazeGrid[location.x, location.y, location.z] = mazeCell;
        mazeCell.SetEntrance();
        mazeCell.SetIndex(location.x, location.y, location.z);
    }
    private MazeCell GetNextCell(MazeCell fromCell, MazeCell toCell)
    {
        if (fromCell.Index.x <= toCell.Index.x)
        {
            if (mazeGrid[fromCell.Index.x + 1, fromCell.Index.y, fromCell.Index.z].type == MazeCell.CellType.Room)
            {
                if (mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1].type == MazeCell.CellType.Room)
                {
                    return mazeGrid[fromCell.Index.x - 1, fromCell.Index.y, fromCell.Index.z];
                }
                else
                {
                    return mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1];
                }
            }
            else
            {
                return mazeGrid[fromCell.Index.x + 1, fromCell.Index.y, fromCell.Index.z];
            }
        }
        if (fromCell.Index.z <= toCell.Index.z)
        {
            if (mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1].type == MazeCell.CellType.Room)
            {
                if (mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1].type == MazeCell.CellType.Room)
                {
                    return mazeGrid[fromCell.Index.x - 1, fromCell.Index.y, fromCell.Index.z];
                }
                else
                {
                    return mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1];
                }
            }
            else
            {
                return mazeGrid[fromCell.Index.x + 1, fromCell.Index.y, fromCell.Index.z];
            }
            //z += 1;
            if (mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1].type == MazeCell.CellType.Room)
            {
                return mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1];
            }
            else
            {
                return mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1];
            }
        }
        if (fromCell.Index.x > toCell.Index.x)
        {
            if (mazeGrid[fromCell.Index.x - 1, fromCell.Index.y, fromCell.Index.z].type == MazeCell.CellType.Room)
            {
                return mazeGrid[fromCell.Index.x, fromCell.Index.y, fromCell.Index.z + 1];
            }
            else
            {
                return mazeGrid[fromCell.Index.x - 1, fromCell.Index.y, fromCell.Index.z];
            }
        }
        return null;
    }
    private bool CheckOverlapping(Vector3Int location, Vector3Int size, MazeCell.CellType ignoreType)
    {
        bool isOverlapping = false;
        if (location.x < 0)
        {
            location.x += 1;
            size.x -= 1;
        }
        if (location.z < 0)
        {
            location.z += 1;
            size.z -= 1;
        }
        if (location.x + size.x > mazeSize.x || location.z + size.z > mazeSize.z)
        {
            return true;
        }
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    if (mazeGrid[location.x + x, location.y + y, location.z + z] != null)
                    {
                        if (mazeGrid[location.x + x, location.y + y, location.z + z].type == ignoreType)
                        {
                            isOverlapping = false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return isOverlapping;
    }
    private void GenerateBossRoom()
    {
        //find the room furthest from spawn at the max y axis
        int roomWithMostDistance = 0;
        int farthestRoomX = mazeSize.x - 1;
        for (int x = 0; x < mazeSize.x; x++)
        {
            if (roomWithMostDistance < mazeGrid[x, 0, mazeSize.z - 1].distanceToSpawn)
            {
                roomWithMostDistance = mazeGrid[x, 0, mazeSize.z - 1].distanceToSpawn;
                farthestRoomX = x;
            }
        }
        //spawn Boss Room
        BossRoom bossRoom = Instantiate(
            BossRoomPrefab, 
            new Vector3(farthestRoomX * cellSize, 0, mazeSize.z * cellSize) + transform.position, 
            Quaternion.identity, transform);
        mazeGrid[farthestRoomX, 0, mazeSize.z - 1].ClearFrontWall();
    }
    private void GenerateTreasureRoom()
    {
        TreasureRoom treasureRoom;
        int randZ;
        int intervall;
        int amountPerZSide = (int)(mazeSize.z / cellSize);
        if (amountPerZSide == 0)
        {
            intervall = 1;
        }
        else
        {
            intervall = mazeSize.z / amountPerZSide;
        }
        for (int z = 0; z < amountPerZSide; z++)
        {
            //room on right side
            randZ = Random.Range(intervall * z, intervall * (z + 1));
            treasureRoom = Instantiate(
                TreasureRoomPrefab,
                new Vector3(mazeSize.x * cellSize, 0, randZ * cellSize) + transform.position,
                Quaternion.identity, transform);  
            treasureRoom.ClearLeftWall();
            mazeGrid[mazeSize.x - 1, 0, randZ].ClearRightWall();

            //room on left side
            randZ = Random.Range(intervall * z + 1, intervall * (z + 1));
            treasureRoom = Instantiate(TreasureRoomPrefab,
                new Vector3(-cellSize, 0, randZ * cellSize) + transform.position,
                Quaternion.identity, transform);
            treasureRoom.ClearRightWall();
            mazeGrid[0, 0, randZ].ClearLeftWall();
        }
    }
    private void CheckEntrancesForVisits()
    {
        for (int i = 0; i < Entrances.Count; i++)
        {
            if (!Entrances[i].isVisited)
            {
                StartCoroutine(GenerateMaze(null, mazeGrid[Entrances[i].Index.x, Entrances[i].Index.y, Entrances[i].Index.z]));
                Debug.Log(Entrances[i]);
                //break;
            }
        }
    }
    private void BeginToConnectEntrances()
    {
        List<MazeCell> entrancesOnOneLevel = new List<MazeCell>();
        for (int height = 0; height < mazeSize.y; height++)
        {
            for (int x = 0; x < Entrances.Count; x++)
            {
                if (Entrances[x].Index.y == height)
                {
                    entrancesOnOneLevel.Add(Entrances[x]);
                }
            }
            //here connect them (pathfinding -.-)
            MazeCell currEntrance = null;
            MazeCell nextEntrance = null;
            for (int i = 0; i < entrancesOnOneLevel.Count - 1; i++)
            {
                currEntrance = GetNextEntrance(entrancesOnOneLevel, currEntrance, i, height);
                nextEntrance = GetNextEntrance(entrancesOnOneLevel, currEntrance, i + 1, height);
                if (nextEntrance != null && currEntrance != null)
                {
                    Vector3Int direction = nextEntrance.Index - currEntrance.Index;
                    do
                    {
                        int randX = Random.Range(0, 2);
                        int randY = Random.Range(0, 2);
                        if (randX == 1 && mazeGrid[currEntrance.Index.x + randX, currEntrance.Index.y, currEntrance.Index.z] != null)
                        {
                            mazeGrid[currEntrance.Index.x + randX, currEntrance.Index.y, currEntrance.Index.z] = Instantiate(
                            MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                            new Vector3(
                                currEntrance.Index.x * cellSize + randX,
                                currEntrance.Index.y * cellSize * 2,
                                currEntrance.Index.z * cellSize) + transform.position,
                            Quaternion.identity, transform
                            );
                            mazeGrid[currEntrance.Index.x + randX, currEntrance.Index.y, currEntrance.Index.z].SetIndex(currEntrance.Index.x + randX, currEntrance.Index.y, currEntrance.Index.z);
                            mazeGrid[currEntrance.Index.x + randX, currEntrance.Index.y, currEntrance.Index.z].SetType(MazeCell.CellType.Hallway);
                        }
                    } while (currEntrance != nextEntrance);
                    currEntrance = nextEntrance;
                }
            }
        }
    }
    private MazeCell GetNextEntrance(List<MazeCell> entrancesOnOneLevel, MazeCell currEntrance, int count, int height)
    {
        if (currEntrance == null)
        {
            return entrancesOnOneLevel[count];
        }
        if (entrancesOnOneLevel[count].Index.y != height)
        {
            return null;
        }
        else
        {
            return entrancesOnOneLevel[count];
        }
    }
    private IEnumerator GenerateMaze(MazeCell prevCell, MazeCell currCell)
    {
        currCell.Visit();
        ClearWalls(prevCell, currCell);

        yield return new WaitForSeconds(0.05f);

        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currCell, nextCell);
            }
        }while (nextCell != null);
    }
    private MazeCell GetNextUnvisitedCell(MazeCell currCell)
    {
        var unvisitedCells = GetUnvisitedCells(currCell);
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currCell)
    {
        int x = currCell.Index.x;
        int y = currCell.Index.y;
        int z = currCell.Index.z;
        if (currCell.type == MazeCell.CellType.Room)
        {
            if (x + 1 < mazeSize.x)
            {
                var cellToRight = mazeGrid[x + 1, y, z];
                if (cellToRight.isVisited == false && cellToRight.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToRight;
                }
            }
            if (x - 1 >= 0)
            {
                var cellToLeft = mazeGrid[x - 1, y, z];
                if (cellToLeft.isVisited == false && cellToLeft.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToLeft;
                }
            }
            if (z + 1 < mazeSize.z)
            {
                var cellToFront = mazeGrid[x, y, z + 1];
                if (cellToFront.isVisited == false && cellToFront.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToFront;
                }
            }
            if (z - 1 >= 0)
            {
                var cellToBack = mazeGrid[x, y, z - 1];
                if (cellToBack.isVisited == false && cellToBack.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToBack;
                }
            }
        }
        else if(currCell.type == MazeCell.CellType.Entrance)
        {
            if (x + 1 < mazeSize.x)
            {
                var cellToRight = mazeGrid[x + 1, y, z];
                if (cellToRight.isVisited == false)
                {
                    yield return cellToRight;
                }
            }
            if (x - 1 >= 0)
            {
                var cellToLeft = mazeGrid[x - 1, y, z];
                if (cellToLeft.isVisited == false)
                {
                    yield return cellToLeft;
                }
            }
            if (z + 1 < mazeSize.z)
            {
                var cellToFront = mazeGrid[x, y, z + 1];
                if (cellToFront.isVisited == false)
                {
                    yield return cellToFront;
                }
            }
            if (z - 1 >= 0)
            {
                var cellToBack = mazeGrid[x, y, z - 1];
                if (cellToBack.isVisited == false)
                {
                    yield return cellToBack;
                }
            }
        }
        else
        {
            if (x + 1 < mazeSize.x)
            {
                var cellToRight = mazeGrid[x + 1, y, z];
                if (cellToRight.isVisited == false && cellToRight.type != MazeCell.CellType.Room)
                {
                    yield return cellToRight;
                }
            }
            if (x - 1 >= 0)
            {
                var cellToLeft = mazeGrid[x - 1, y, z];
                if (cellToLeft.isVisited == false && cellToLeft.type != MazeCell.CellType.Room)
                {
                    yield return cellToLeft;
                }
            }
            if (z + 1 < mazeSize.z)
            {
                var cellToFront = mazeGrid[x, y, z + 1];
                if (cellToFront.isVisited == false && cellToFront.type != MazeCell.CellType.Room)
                {
                    yield return cellToFront;
                }
            }
            if (z - 1 >= 0)
            {
                var cellToBack = mazeGrid[x, y, z - 1];
                if (cellToBack.isVisited == false && cellToBack.type != MazeCell.CellType.Room)
                {
                    yield return cellToBack;
                }
            }
        }
    }
    private void ClearWalls(MazeCell prevCell, MazeCell currCell)
    {
        if (prevCell == null)
        {
            if (currCell.Index.y == 0)
            {
                currCell.ClearLeftWall();
                currCell.distanceToSpawn = 0;
                FarthestCell = currCell;
            }
            return;
        }
        if (prevCell.transform.position.x < currCell.transform.position.x)
        {
            if (prevCell.type != MazeCell.CellType.Room && currCell.type != MazeCell.CellType.Room)
            {
                currCell.ClearLeftWall();
                prevCell.ClearRightWall();
            }
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }
            return;
        }
        if (prevCell.transform.position.x > currCell.transform.position.x)
        {
            if (prevCell.type != MazeCell.CellType.Room && currCell.type != MazeCell.CellType.Room)
            {
                currCell.ClearRightWall();
                prevCell.ClearLeftWall();
            }
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }
            return;
        }
        if (prevCell.transform.position.z < currCell.transform.position.z)
        {
            if (prevCell.type != MazeCell.CellType.Room && currCell.type != MazeCell.CellType.Room)
            {
                currCell.ClearBackWall();
                prevCell.ClearFrontWall();
            }
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }
            return;
        }
        if (prevCell.transform.position.z > currCell.transform.position.z)
        {
            if (prevCell.type != MazeCell.CellType.Room && currCell.type != MazeCell.CellType.Room)
            {
                currCell.ClearFrontWall();
                prevCell.ClearBackWall();
            }
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }
            return;
        }
    }
    /*
    private MazeCell GetNextVisitedCell(MazeCell currCell)
    {
        var visitedCells = GetVisitedCells(currCell);
        return visitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }
    private IEnumerable<MazeCell> GetVisitedCells(MazeCell currCell)
    {
        int x = currCell.Index.x;
        int z = currCell.Index.y;
        if (x + distancetoNextCell < mazeSize.x)
        {
            var cellToRight = mazeGrid[x + distancetoNextCell, z];
            if (cellToRight.isVisited == true && (cellToRight.type == MazeCell.CellType.Hallway || cellToRight.type == MazeCell.CellType.Entrance))
            {
                yield return cellToRight;
            }
        }
        if (x - distancetoNextCell >= 0)
        {
            var cellToLeft = mazeGrid[x - distancetoNextCell, z];
            if (cellToLeft.isVisited == true && (cellToLeft.type == MazeCell.CellType.Hallway || cellToLeft.type == MazeCell.CellType.Entrance))
            {
                yield return cellToLeft;
            }
        }
        if (z + distancetoNextCell < mazeSize.z)
        {
            var cellToFront = mazeGrid[x, z + distancetoNextCell];
            if (cellToFront.isVisited == true && (cellToFront.type == MazeCell.CellType.Hallway || cellToFront.type == MazeCell.CellType.Entrance))
            {
                yield return cellToFront;
            }
        }
        if (z - distancetoNextCell >= 0)
        {
            var cellToBack = mazeGrid[x, z - distancetoNextCell];
            if (cellToBack.isVisited == true && (cellToBack.type == MazeCell.CellType.Hallway || cellToBack.type == MazeCell.CellType.Entrance))
            {
                yield return cellToBack;
            }
        }
    }
    */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 size = new Vector3(mazeSize.x, mazeSize.y * 2, mazeSize.z);
        Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
        Gizmos.color = Color.blue;
        for (int i = 0; i < Rooms.Count - 1; i++)
        {
            testVector = Entrances[1].Index - Entrances[0].Index;
            //Gizmos.DrawLine(Rooms[i].transform.position, Rooms[i + 1].transform.position);
        }
    }
}
