using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;

public class MazeGenerator : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool generateWithStairs;
    [SerializeField] private bool generateWithRooms;
    [Tooltip("Will only be taken into account if fillRestWithHallways = true")]
    [SerializeField] private bool generateWithEmpty;
    [Tooltip("If no rest of the grid will be filled with EmptySpace blocks")]
    [SerializeField] private bool fillRestWithHallways;
    [SerializeField] private bool mainEntranceOnTop;
    [SerializeField] Vector3Int mazeSize; //width, height, depth
    [Tooltip("How big the cells are in x,z so that they are placed in the right amount of distance")]
    [SerializeField] private float cellSize;
    //[SerializeField] private float cellHight;
    [SerializeField] private MazeFloor FloorPrefab;
    [SerializeField] private MazeCell MainEntrancePrefab;
    private List<MazeFloor> FloorList = new List<MazeFloor>();

    [Header("Save Rooms")]
    [SerializeField] private MazeCell SaveRoomPrefab;
    [SerializeField] private int saveEveryYFloor;

    [Header("Hallways")]
    [SerializeField] private MazeCell[] HallwayCells;
    [SerializeField] private MazeCell[] SpecialHallwayCells;
    //public MazeCell FarthestCell;
    [SerializeField] private BossRoom BossRoomPrefab;
    [SerializeField] private TreasureRoom TreasureRoomPrefab;
    private MazeCell[,,] mazeGrid;

    [Header("Staircase")]
    [SerializeField] private int stairsAttempts;
    [SerializeField] private int maxStairsAmount;
    private int stairsAmount;
    [Tooltip("The size of the Stairs Prefab in x and z direction (Note: don't need y cause stairs will always be 2 blocks high")]
    [SerializeField] private Vector2Int stairsSize;
    [SerializeField] private Staircase MazeStaircasePrefab;
    [SerializeField] private MazeCell StairsEntrancePrefab;
    [SerializeField] private MazeCell StairsPrefab;
    [SerializeField] private MazeCell StairsPlaceholder;
    private List<Staircase> Staircases = new List<Staircase>();

    [Header("Rooms")]
    [SerializeField] private int normalRoomAttempts;
    [SerializeField] private int maxNormalRoomAmount;
    [SerializeField] private Room[] NormalRooms;

    [Header("Monster Rooms")]
    [SerializeField] private int monsterRoomAttempts;
    [SerializeField] private int maxMonsterRoomAmount;
    [SerializeField] private Room[] MonsterRooms;

    [Header("Empty Space")]
    [SerializeField] private int emptyAttempts;
    [SerializeField] private MazeCell EmptySpacePrefab;
    [SerializeField] Vector2Int emptyMinSize;
    [SerializeField] Vector2Int emptyMaxSize;

    public bool isValid;
    [Header("Entrance Stuff")]
    [SerializeField] private List<MazeCell> Entrances;
    List<MazeCell> entrancesOnOneLevel;
    private int lastdirection;

    [Header("AiStuff")]
    [SerializeField] private NavMeshSurface NavMesh;

    [Header("Camera")]
    [SerializeField] private GameObject Camera;

    IEnumerator Start()
    {
        Random.InitState(15);
        isValid = true;
        cellSize *= transform.localScale.x;
        mazeGrid = new MazeCell[mazeSize.x, mazeSize.y * 2, mazeSize.z];
        entrancesOnOneLevel = new List<MazeCell>();
        if (mainEntranceOnTop)
        {
            transform.position = new Vector3(0, (-mazeSize.y + 1) * cellSize * 2, 0);
        }
        else
        {
            transform.position = new Vector3(0, 0, 0);
        }
        yield return new WaitForSeconds(0.1f);
        for (int fCount = 0; fCount < mazeSize.y; fCount++)
        {
            MazeFloor floor = Instantiate(
                FloorPrefab, transform.position + (Vector3.up * cellSize * 2 * fCount),
                Quaternion.identity, 
                transform);
            floor.SetTrigger(mazeSize, cellSize);
            if (fCount > 1)
            {
                floor.ConnectPrev(FloorList[fCount - 2]);
                FloorList[fCount - 2].ConnectNext(floor);
            }
            FloorList.Add(floor);
        }
        #region save rooms
        for (int srCount = 1; srCount < mazeSize.y / saveEveryYFloor + 1; srCount++)
        {
            if (srCount * saveEveryYFloor - 1 <= mazeSize.y)
            {
                mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1] = Instantiate(
                    SaveRoomPrefab,
                    new Vector3((mazeSize.x - 1) * cellSize, (srCount * saveEveryYFloor - 1) * cellSize * 2, (mazeSize.z - 1) * cellSize) + transform.position,
                    Quaternion.identity, FloorList[srCount * saveEveryYFloor - 1].transform.GetChild(2).transform
                    );
                mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1].SetIndex(mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1);
                mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1].SetEntrance();
                mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1].SetType(MazeCell.CellType.Entrance);
                mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1].ClearFrontWall();
                mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1].distanceToSpawn = 0;
                Entrances.Add(mazeGrid[mazeSize.x - 1, srCount * saveEveryYFloor - 1, mazeSize.z - 1]);
            }
        }
        yield return new WaitForSeconds(0.1f);
        #endregion

        #region main entrance 
        if (mainEntranceOnTop)
        {
            mazeGrid[0, mazeSize.y - 1, 0] = Instantiate(
                MainEntrancePrefab,
                new Vector3(0, (mazeSize.y - 1) * cellSize * 2, 0) + transform.position,
                Quaternion.identity, FloorList[mazeSize.y - 1].transform.GetChild(2).transform
                );
            mazeGrid[0, mazeSize.y - 1, 0].SetIndex(0, mazeSize.y - 1, 0);
            mazeGrid[0, mazeSize.y - 1, 0].SetEntrance();
            mazeGrid[0, mazeSize.y - 1, 0].SetType(MazeCell.CellType.Entrance);
            mazeGrid[0, mazeSize.y - 1, 0].ClearLeftWall();
            mazeGrid[0, mazeSize.y - 1, 0].distanceToSpawn = 0;
            Entrances.Add(mazeGrid[0, mazeSize.y - 1, 0]);
        }
        else
        {
            mazeGrid[0, 0, 0] = Instantiate(
                MainEntrancePrefab,
                new Vector3(0, 0, 0) + transform.position,
                Quaternion.identity, FloorList[0].transform.GetChild(2).transform
                );
            mazeGrid[0, 0, 0].SetIndex(0, 0, 0);
            mazeGrid[0, 0, 0].SetEntrance();
            mazeGrid[0, 0, 0].SetType(MazeCell.CellType.Entrance);
            mazeGrid[0, 0, 0].ClearLeftWall();
            mazeGrid[0, 0, 0].distanceToSpawn = 0;
            Entrances.Add(mazeGrid[0, 0, 0]);
        }
        yield return new WaitForSeconds(0.1f);
        #endregion

        if (generateWithStairs)
        {
            CreateStaircases();
            yield return new WaitForSeconds(0.1f);
        }
        if (generateWithRooms)
        {
            GenerateRooms(MonsterRooms, monsterRoomAttempts, maxMonsterRoomAmount);
            yield return new WaitForSeconds(0.1f);
            GenerateRooms(NormalRooms, normalRoomAttempts, maxNormalRoomAmount);
            yield return new WaitForSeconds(0.1f);
            //yield return GenerateRooms();
        }

        ConnectEntrances();
        yield return new WaitForSeconds(0.1f);
        //yield return ConnectEntrances();
        if (generateWithEmpty && fillRestWithHallways)
        {
            GenerateEmpty();
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Empty Spaces generated");
            //yield return GenerateEmpty();
        }
        FillRestOfGrid();
        yield return new WaitForSeconds(0.1f);

        for (int y = 0; y < mazeSize.y; y++)
        {
            for (int x = 0; x < Entrances.Count; x++)
            {
                if (Entrances[x].Index.y == y)
                {
                    yield return GenerateMaze(null, mazeGrid[Entrances[x].Index.x, Entrances[x].Index.y, Entrances[x].Index.z]);
                }
            }
        }
        Debug.Log("Maze generated");

        //GenerateBossRoom();
        //GenerateTreasureRoom();
        //CheckEntrancesForVisits();
        if (!isValid)
        {
            Debug.LogWarning("Maze not valid");
        }
        DeleteUnusedSpaces();
        yield return new WaitForSeconds(0.1f);
        //yield return new WaitForSeconds(1);
        NavMesh.BuildNavMesh();
        yield return new WaitForSeconds(0.1f);
        DisableUpperFloors();
    }
    private void FillRestOfGrid()
    {
        for (int y = 0; y < mazeSize.y; y++)
        {
            for (int x = 0; x < mazeSize.x; x++)
            {
                for (int z = 0; z < mazeSize.z; z++)
                {
                    if (mazeGrid[x, y, z] == null)
                    {
                        if (fillRestWithHallways)
                        {
                            mazeGrid[x, y, z] = Instantiate(HallwayCells[Random.Range(0, HallwayCells.Length)],
                                new Vector3(
                                    x * cellSize,
                                    y * cellSize * 2,
                                    z * cellSize) + transform.position,
                                Quaternion.identity, FloorList[y].transform.GetChild(2).transform
                                );
                            mazeGrid[x, y, z].SetIndex(x, y, z);
                            mazeGrid[x, y, z].SetType(MazeCell.CellType.Hallway);
                        }
                        else
                        {
                            mazeGrid[x, y, z] = Instantiate(EmptySpacePrefab,
                                new Vector3(
                                    x * cellSize,
                                    y * cellSize * 2,
                                    z * cellSize) + transform.position,
                                Quaternion.identity, FloorList[y].transform.GetChild(3).transform
                                );
                            mazeGrid[x, y, z].SetIndex(x, y, z);
                            mazeGrid[x, y, z].SetType(MazeCell.CellType.EmptySpace);
                            mazeGrid[x, y, z].Visit();
                        }
                    }
                }
            }
        }
    }

    #region Generating rooms
    private void GenerateRooms(Room[] RoomArray, int attempts, int maxAmount)
    {
        for (int mazeHight = 0; mazeHight < mazeSize.y; mazeHight++)
        {
            int roomsAmount = 0;
            for (int i = 0; i < attempts; i++)
            {
                Room room = RoomArray[Random.Range(0, RoomArray.Length)];
                Vector3Int size = room.roomSize;
                Vector3Int location = new Vector3Int(
                    Random.Range(0, mazeSize.x - size.x),
                    mazeHight,
                    Random.Range(0, mazeSize.z - size.z));
                if (location.x < 2 && location.z < 2)
                {
                    location.x += 2;
                    location.z += 2;
                }
                if (!CheckOverlapping(location + new Vector3Int(-1, 0, -1), size + new Vector3Int(2, 0, 2), MazeCell.CellType.None))
                {
                    room = Instantiate(room,
                        new Vector3(
                            location.x * cellSize,
                            location.y * cellSize * 2,
                            location.z * cellSize)
                        + transform.position, Quaternion.identity, FloorList[location.y].transform.GetChild(1).transform);
                    room.roomPosition = location;
                    GenerateRoomTiles(location, size, room);
                    roomsAmount++;
                }
                if (roomsAmount == maxAmount)
                {
                    break;
                }
                //yield return new WaitForSeconds(0.1f);
            }
        }
        Debug.Log("Rooms generated");
    }
    private void GenerateRoomTiles(Vector3Int location, Vector3Int size, Room room)
    {
        for (int xStep = 0; xStep < size.x; xStep++)
        {
            for (int zStep = 0; zStep < size.z; zStep++)
            {
                mazeGrid[location.x + xStep, location.y, location.z + zStep] = room.XRows[xStep].transform.GetChild(zStep).GetComponent<MazeCell>();
                mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
                mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(room.XRows[xStep].transform.GetChild(zStep).GetComponent<MazeCell>().type);
                if(mazeGrid[location.x + xStep, location.y, location.z + zStep].type == MazeCell.CellType.Entrance)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep].SetEntrance();
                    Entrances.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                /*
                //corner l-u
                if (xStep == 0 && zStep == 0)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabLU,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 90, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //corner l-o
                if (xStep == 0 && zStep == size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabLO,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 180, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //corner r-u
                if (xStep == size.x - 1 && zStep == 0)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabRU,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 0, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //corner r-o
                if (xStep == size.x - 1 && zStep == size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabRO,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 270, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall left side
                if (xStep == 0 && zStep > 0 && zStep < size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 90, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall right side
                if (xStep == size.x - 1 && zStep > 0 && zStep < size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 270, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall bottom side
                if (xStep > 0 && xStep < size.x - 1 && zStep == 0)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 0, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //wall top side
                if (xStep > 0 && xStep < size.x - 1 && zStep == size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.Euler(0, 180, 0), roomTransform);
                    Rooms[roomCount].borderCells.Add(mazeGrid[location.x + xStep, location.y, location.z + zStep]);
                }
                //center pieces
                if (xStep > 0 && xStep < size.x - 1 && zStep > 0 && zStep < size.z - 1)
                {
                    mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(MiddlePrefab,
                        new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                        Quaternion.identity, roomTransform);
                }
                */
            }
        }
    }
    #endregion

    private void GenerateEmpty()
    {
        //emptyAttempts /= mazeSize.y;
        for (int mazeHight = 0; mazeHight < mazeSize.y; mazeHight++)
        {
            for (int i = 0; i < emptyAttempts; i++)
            {
                Vector3Int size = new Vector3Int(Random.Range(emptyMinSize.x, emptyMaxSize.x + 1),
                    1,
                    Random.Range(emptyMinSize.y, emptyMaxSize.y + 1));
                Vector3Int location = new Vector3Int(
                    Random.Range(0, mazeSize.x - size.x + 1),
                    mazeHight,
                    Random.Range(0, mazeSize.z - size.z + 1));
                if (!CheckOverlapping(location, size, MazeCell.CellType.EmptySpace))
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int z = 0; z < size.z; z++)
                        {
                            if (mazeGrid[location.x + x, location.y, location.z + z] == null)
                            {
                                mazeGrid[location.x + x, location.y, location.z + z] = Instantiate(EmptySpacePrefab,
                                    new Vector3((location.x + x) * cellSize, location.y * cellSize * 2, (location.z + z) * cellSize) + transform.position,
                                    Quaternion.identity, FloorList[location.y].transform.GetChild(3).transform);
                                mazeGrid[location.x + x, location.y, location.z + z].SetIndex(location.x + x, location.y, location.z + z);
                                mazeGrid[location.x + x, location.y, location.z + z].SetType(MazeCell.CellType.EmptySpace);
                                mazeGrid[location.x + x, location.y, location.z + z].Visit();
                            }
                        }
                    }
                }
                //yield return new WaitForSeconds(0.1f);
            }
        }
    }

    #region Generate stairs
    private void CreateStaircases()
    {
        Vector3Int size = new Vector3Int();
        for (int mazeHight = 0; mazeHight < mazeSize.y - 1; mazeHight++)
        {
            stairsAmount = 0;
            for (int i = 0; i < stairsAttempts; i++)
            {
                Vector3Int location = new Vector3Int(
                    Random.Range(1, mazeSize.x - 5),
                    mazeHight,
                    Random.Range(1, mazeSize.z - 5));
                int stairsVariant = Random.Range(0, 4);
                if (stairsVariant == 0 || stairsVariant == 2)
                {
                    size = new Vector3Int(stairsSize.x, 2, stairsSize.y + 2);
                }
                if (stairsVariant == 1 || stairsVariant == 3)
                {
                    size = new Vector3Int(stairsSize.y + 2, 2, stairsSize.x);
                }
                if (!CheckOverlapping(location + new Vector3Int(-1, 0, -1), size + new Vector3Int(2, 0, 2), MazeCell.CellType.None))
                {
                    Staircase staircase = Instantiate(MazeStaircasePrefab,
                        new Vector3(
                            (location.x) * cellSize,
                            (location.y) * cellSize * 2,
                            (location.z) * cellSize)
                        + transform.position, Quaternion.identity, FloorList[location.y].transform.GetChild(0).transform);
                    Staircases.Add(staircase);
                    staircase.staircasePosition = location;
                    staircase.staircaseSize = size;
                    GenerateStairTiles(location, size, stairsVariant, staircase.transform);
                    stairsAmount++;
                }
                if (stairsAmount == maxStairsAmount)
                {
                    break;
                }
            }
        }
        Debug.Log("Stairs generated");
    }
    private void GenerateStairTiles(Vector3Int location, Vector3Int size, int variant, Transform stairTransform)
    {
        CreateStairsEntrances(location, variant, stairTransform);
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
                            Quaternion.identity, stairTransform);
                        mazeGrid[location.x + x, location.y + y, location.z + z].SetIndex(location.x + x, y, location.z + z);
                        mazeGrid[location.x + x, location.y + y, location.z + z].SetType(MazeCell.CellType.Stairs);
                        mazeGrid[location.x + x, location.y + y, location.z + z].Visit();
                        //why is that here?
                        //mazeGrid[location.x + x, location.y + y, location.z + z].GetComponent<MazeCell>().distanceToSpawn = variant;
                    }
                }
            }
        }
    }
    private void CreateStairsEntrances(Vector3Int location, int stairsVariant, Transform stairTransform)
    {
        Vector3Int entrance1Location;
        Vector3Int entrance2Location;
        Vector3Int stairsLocation;
        if (stairsVariant == 0)
        {
            entrance1Location = new Vector3Int(location.x, location.y, location.z + 4);
            entrance2Location = new Vector3Int(location.x, location.y + 1, location.z);
            stairsLocation = new Vector3Int(location.x, location.y, location.z + 3);

            InstantiateStairsEntrance(entrance1Location, 0, stairTransform);
            InstantiateStairsEntrance(entrance2Location, 2, stairTransform);
            InstantiateStairs(stairsLocation, stairsVariant, stairTransform);
        }
        if (stairsVariant == 1)
        {
            entrance1Location = new Vector3Int(location.x + 4, location.y, location.z);
            entrance2Location = new Vector3Int(location.x, location.y + 1, location.z);
            stairsLocation = new Vector3Int(location.x + 3, location.y, location.z);

            InstantiateStairsEntrance(entrance1Location, 1, stairTransform);
            InstantiateStairsEntrance(entrance2Location, 3, stairTransform);
            InstantiateStairs(stairsLocation, stairsVariant, stairTransform);
        }
        if (stairsVariant == 2)
        {
            entrance1Location = new Vector3Int(location.x, location.y, location.z);
            entrance2Location = new Vector3Int(location.x, location.y + 1, location.z + 4);
            stairsLocation = new Vector3Int(location.x, location.y, location.z + 1);

            InstantiateStairsEntrance(entrance1Location, 2, stairTransform);
            InstantiateStairsEntrance(entrance2Location, 0, stairTransform);
            InstantiateStairs(stairsLocation, stairsVariant, stairTransform);
        }
        if (stairsVariant == 3)
        {
            entrance1Location = new Vector3Int(location.x, location.y, location.z);
            entrance2Location = new Vector3Int(location.x + 4, location.y + 1, location.z);
            stairsLocation = new Vector3Int(location.x + 1, location.y, location.z);

            InstantiateStairsEntrance(entrance1Location, 3, stairTransform);
            InstantiateStairsEntrance(entrance2Location, 1, stairTransform);
            InstantiateStairs(stairsLocation, stairsVariant, stairTransform);
        }
    }
    private void InstantiateStairsEntrance(Vector3Int entranceLocation, int clearWall, Transform stairTransform)
    {
        MazeCell mazeCell = Instantiate(StairsEntrancePrefab,
            new Vector3((entranceLocation.x) * cellSize, entranceLocation.y * cellSize * 2, (entranceLocation.z) * cellSize) + transform.position,
            Quaternion.identity, stairTransform);
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
    private void InstantiateStairs(Vector3Int location, int variant, Transform stairTransform)
    {
        MazeCell mazeCell = Instantiate(StairsPrefab,
            new Vector3((location.x) * cellSize, location.y * cellSize * 2, (location.z) * cellSize) + transform.position,
            Quaternion.Euler(0, 90 * variant, 0), stairTransform);
        mazeGrid[location.x, location.y, location.z] = mazeCell;
        mazeCell.Visit();
        mazeCell.SetIndex(location.x, location.y, location.z);
        mazeCell.SetType(MazeCell.CellType.Stairs);
    }
    #endregion

    private bool CheckOverlapping(Vector3Int location, Vector3Int size, MazeCell.CellType ignoreType)
    {
        bool isOverlapping = false;
        if (location.x < 0 || location.z < 0)
        {
            return true;
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

    #region Connecting entrances
    private void ConnectEntrances()
    {
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
            if (entrancesOnOneLevel.Count > 2)
            {
                for (int i = 0; i < entrancesOnOneLevel.Count - 1; i++)
                {
                    CreatePathBetweenTwoEntrances(entrancesOnOneLevel[i].Index, entrancesOnOneLevel[i + 1].Index);
                }
                entrancesOnOneLevel.Clear();
                lastdirection = 4;
            }
        }
        Debug.Log("All entrances connected");
    }
    private void CreatePathBetweenTwoEntrances(Vector3Int currCellIndex, Vector3Int nextEntranceIndex)
    {
        currCellIndex = GetNextClosestCell(currCellIndex, nextEntranceIndex);

        if (mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z] == null)
        {
            mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z] = Instantiate(
                            HallwayCells[Random.Range(0, HallwayCells.Length)],
                            new Vector3(
                                currCellIndex.x * cellSize,
                                currCellIndex.y * cellSize * 2,
                                currCellIndex.z * cellSize) + transform.position,
                            Quaternion.identity, FloorList[currCellIndex.y].transform.GetChild(2).transform
                            );
            mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z].SetIndex(currCellIndex.x, currCellIndex.y, currCellIndex.z);
            mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z].SetType(MazeCell.CellType.Hallway);
        }
        //only one level for now

        //yield return new WaitForSeconds(0.1f);

        //nextEntrance = Rooms[1].GetRandomEntrance();
        if (currCellIndex != nextEntranceIndex)
        {
            CreatePathBetweenTwoEntrances(currCellIndex, nextEntranceIndex);
        }
        //get random entrance from room 1, then from room 2
        //after that connect them
        //get adjacent cells from currCell that can be moved to (only null)
        //return the one with the least distance to nextEntrance
        //spawn a hallway at the index of the returned cell

        //then random from room 2 to random from room 3, etc
    }
    private Vector3Int GetNextClosestCell(Vector3Int currCellIndex, Vector3Int nextEntranceIndex)
    {
        List<Vector3Int> freeCellIndex = new List<Vector3Int>();
        Vector3Int nextCellIndex = new Vector3Int(0, 0, 0);
        if (lastdirection != 0) //only check front if not moved back
        {
            if (currCellIndex.z + 1 < mazeSize.z && mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z + 1] == null)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x, currCellIndex.y, currCellIndex.z + 1));
            }
            else if (currCellIndex.z + 1 < mazeSize.z && mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z + 1].type == MazeCell.CellType.Hallway)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x, currCellIndex.y, currCellIndex.z + 1));
            }
            if (currCellIndex + new Vector3Int(0, 0, 1) == nextEntranceIndex)
            {
                nextCellIndex = nextEntranceIndex;
                lastdirection = 4;
                return nextCellIndex;
            }
        }
        if (lastdirection != 1) //only check right if not moved left
        {
            if (currCellIndex.x + 1 < mazeSize.x && mazeGrid[currCellIndex.x + 1, currCellIndex.y, currCellIndex.z] == null)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x + 1, currCellIndex.y, currCellIndex.z));
            }
            else if (currCellIndex.x + 1 < mazeSize.x && mazeGrid[currCellIndex.x + 1, currCellIndex.y, currCellIndex.z].type == MazeCell.CellType.Hallway)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x + 1, currCellIndex.y, currCellIndex.z));
            }
            if (currCellIndex + new Vector3Int(1, 0, 0) == nextEntranceIndex)
            {
                nextCellIndex = nextEntranceIndex;
                lastdirection = 4;
                return nextCellIndex;
            }
        }
        if (lastdirection != 2) //only check back if not moved front
        {
            if (currCellIndex.z - 1 >= 0 && mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z - 1] == null)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x, currCellIndex.y, currCellIndex.z - 1));
            }
            else if (currCellIndex.z - 1 >= 0 && mazeGrid[currCellIndex.x, currCellIndex.y, currCellIndex.z - 1].type == MazeCell.CellType.Hallway)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x, currCellIndex.y, currCellIndex.z - 1));
            }
            if (currCellIndex + new Vector3Int(0, 0, -1) == nextEntranceIndex)
            {
                nextCellIndex = nextEntranceIndex;
                lastdirection = 4;
                return nextCellIndex;
            }
        }
        if (lastdirection != 3) //only check left if not moved right
        {
            if (currCellIndex.x - 1 >= 0 && mazeGrid[currCellIndex.x - 1, currCellIndex.y, currCellIndex.z] == null)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x - 1, currCellIndex.y, currCellIndex.z));
            }
            else if (currCellIndex.x - 1 >= 0 && mazeGrid[currCellIndex.x - 1, currCellIndex.y, currCellIndex.z].type == MazeCell.CellType.Hallway)
            {
                freeCellIndex.Add(new Vector3Int(currCellIndex.x - 1, currCellIndex.y, currCellIndex.z));
            }
            if (currCellIndex + new Vector3Int(-1, 0, 0) == nextEntranceIndex)
            {
                nextCellIndex = nextEntranceIndex;
                lastdirection = 4;
                return nextCellIndex;
            }
        }
        //get the cell with smallest distance
        if (freeCellIndex.Count > 0)
        {
            if (freeCellIndex[0].z < currCellIndex.z)
            {
                //move back
                lastdirection = 0;
            }
            if (freeCellIndex[0].x < currCellIndex.x)
            {
                //move left
                lastdirection = 1;
            }
            if (freeCellIndex[0].z > currCellIndex.z)
            {
                //move front
                lastdirection = 2;
            }
            if (freeCellIndex[0].x > currCellIndex.x)
            {
                //move right
                lastdirection = 3;
            }
            float distance = Vector3.Distance(freeCellIndex[0], nextEntranceIndex);
            nextCellIndex = new Vector3Int(freeCellIndex[0].x, freeCellIndex[0].y, freeCellIndex[0].z);
            for (int i = 0; i < freeCellIndex.Count; i++)
            {
                if (Vector3.Distance(freeCellIndex[i], nextEntranceIndex) < distance)
                {
                    if (freeCellIndex[i].z < currCellIndex.z)
                    {
                        //move back
                        lastdirection = 0;
                    }
                    if (freeCellIndex[i].x < currCellIndex.x)
                    {
                        //move left
                        lastdirection = 1;
                    }
                    if (freeCellIndex[i].z > currCellIndex.z)
                    {
                        //move front
                        lastdirection = 2;
                    }
                    if (freeCellIndex[i].x > currCellIndex.x)
                    {
                        //move right
                        lastdirection = 3;
                    }
                    distance = Vector3.Distance(freeCellIndex[i], nextEntranceIndex);
                    nextCellIndex = new Vector3Int(freeCellIndex[i].x, freeCellIndex[i].y, freeCellIndex[i].z);
                }
            }
        }
        return nextCellIndex;
    }
    #endregion

    #region Generating maze
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
        } while (nextCell != null);
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
        else if (currCell.type == MazeCell.CellType.Entrance)
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
            if (currCell.Index == new Vector3Int(0, 0, 0))
            {
               // currCell.ClearLeftWall();
               // currCell.distanceToSpawn = 0;
                //FarthestCell = currCell;
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
            /*if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }*/
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
            /*if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }*/
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
            /*if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }*/
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
            /*if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }*/
            return;
        }
    }
    #endregion
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
    private void DeleteUnusedSpaces()
    {
        for (int i = 0; i < FloorList.Count; i++)
        {
            FloorList[i].DeleteUnusedSpace();
        }
    }
    private void DisableUpperFloors()
    {
        if (mainEntranceOnTop)
        {
            for (int i = 0; i < FloorList.Count - 2; i++)
            {
                FloorList[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 2; i < FloorList.Count; i++)
            {
                FloorList[i].gameObject.SetActive(false);
            }
        }
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3((transform.position.x + mazeSize.x) * cellSize / 2 - cellSize / 2, (Mathf.Abs(transform.position.y) + mazeSize.y) * cellSize, (transform.position.z + mazeSize.z) * cellSize / 2 - cellSize / 2), new Vector3((transform.position.x + mazeSize.x) * cellSize, (Mathf.Abs(transform.position.y) + mazeSize.y) * cellSize * 2, (transform.position.z + mazeSize.z) * cellSize));
    }
}
