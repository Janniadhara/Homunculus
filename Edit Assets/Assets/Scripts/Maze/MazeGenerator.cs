using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell[] MazeCellPrefab;
    public MazeCell FarthestCell;
    [SerializeField] private BossRoom BossRoomPrefab;
    [SerializeField] private TreasureRoom TreasureRoomPrefab;
    //[SerializeField] private int mazeWidth; //x
    //[SerializeField] private int mazeDepth; //z
    [SerializeField] Vector3Int mazeSize; //width, height, depth
    private MazeCell[,,] mazeGrid;
    [SerializeField] private float cellSize;
    [SerializeField] private bool withGaps;
    private int distanceToZero;
    private int distancetoNextCell;

    [Header("Rooms")]
    [SerializeField] private bool generateWithRooms;
    [SerializeField] private int roomAttempts;
    [SerializeField] private MazeCell MazeRoomPrefab;
    [SerializeField] private MazeCell CornerPrefabLU;
    [SerializeField] private MazeCell CornerPrefabLO;
    [SerializeField] private MazeCell CornerPrefabRU;
    [SerializeField] private MazeCell CornerPrefabRO;
    [SerializeField] private MazeCell WallPrefab;
    [SerializeField] private MazeCell MiddlePrefab;
    [SerializeField] Vector2Int roomMinSize;
    [SerializeField] Vector2Int roomMaxSize;

    [Header("Empty Space")]
    [SerializeField] private bool generateWithEmpty;
    [SerializeField] private int emptyAttempts;
    [SerializeField] private MazeCell EmptySpacePrefab;
    [SerializeField] Vector2Int emptyMinSize;
    [SerializeField] Vector2Int emptyMaxSize;

    [Header("Staircase")]
    [SerializeField] private bool generateWithStairs;
    [SerializeField] private int stairsAttempts;
    [SerializeField] private MazeCell StairsEntrancePrefab;
    [SerializeField] private MazeCell StairsPrefab;
    [SerializeField] private MazeCell StairsPlaceholder;
    [SerializeField] private int maxStairsAmount;
    private int stairsAmount;

    public bool isValid;
    [SerializeField] private List<MazeCell> Entrances;


    IEnumerator Start()
    {
        isValid = true;
        distancetoNextCell = 1;
        cellSize *= transform.localScale.x;
        if (withGaps)
        {
            distancetoNextCell = 2;
        }
        mazeGrid = new MazeCell[
            mazeSize.x * distancetoNextCell, 
            mazeSize.y * distancetoNextCell * 2, 
            mazeSize.z * distancetoNextCell];
        if (generateWithStairs)
        {
            GenerateStairs();
        }
        if (generateWithEmpty)
        {
            GenerateEmpty();
            //yield return GenerateEmpty();
        }
        if (generateWithRooms)
        {
            GenerateRooms();
            //yield return GenerateRooms();
        }
        for (int y = 0; y < mazeSize.y; y++)
        {
            for (int x = 0; x < mazeSize.x; x++)
            {
                for (int z = 0; z < mazeSize.z; z++)
                {
                    if (mazeGrid[x * distancetoNextCell, y * distancetoNextCell, z * distancetoNextCell] == null)
                    {
                        mazeGrid[x * distancetoNextCell, y  * distancetoNextCell, z * distancetoNextCell] = Instantiate(
                            MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                            new Vector3(
                                x * cellSize * distancetoNextCell, 
                                y * cellSize * distancetoNextCell * 2, 
                                z * cellSize * distancetoNextCell) + transform.position,
                            Quaternion.identity, transform
                            );
                        mazeGrid[x * distancetoNextCell, y * distancetoNextCell, z * distancetoNextCell].SetIndex(x * distancetoNextCell, y * distancetoNextCell, z * distancetoNextCell);
                        mazeGrid[x * distancetoNextCell, y * distancetoNextCell, z * distancetoNextCell].SetType(MazeCell.CellType.Hallway);
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
                if (!CheckOverlapping(roomLocation, roomSize))
                {
                    for (int x = 0; x < roomSize.x; x++)
                    {
                        for (int z = 0; z < roomSize.z; z++)
                        {
                            GenerateRoomTile(roomLocation, roomSize, x, z);
                            //mazeGrid[roomLocation.x + x, roomLocation.y + z] = Instantiate(MazeRoomPrefab,
                            //        new Vector3((roomLocation.x + x) * cellSize, 0, (roomLocation.y + z) * cellSize),
                            //        Quaternion.identity, transform);
                            //mazeGrid[roomLocation.x + x, roomLocation.y + z].Visit();
                        }
                    }
                    mazeGrid[roomLocation.x, roomLocation.y, roomLocation.z].SetEntrance();
                    mazeGrid[roomLocation.x + roomSize.x - 1, roomLocation.y, roomLocation.z + roomSize.z - 1].SetEntrance();
                    Entrances.Add(mazeGrid[roomLocation.x, roomLocation.y, roomLocation.z]);
                    Entrances.Add(mazeGrid[roomLocation.x + roomSize.x - 1, roomLocation.y, roomLocation.z + roomSize.z - 1]);
                }
                //yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private void GenerateRoomTile(Vector3Int location, Vector3Int size, int xStep, int zStep)
    {
        //corner l-u
        if (xStep == 0 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabLU,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 90, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //corner l-o
        if (xStep == 0 && zStep == size.z - 1)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabLO,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 180, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //corner r-u
        if (xStep == size.x - 1 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabRU,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 0, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //corner r-o
        if (xStep == size.x - 1 && zStep == size.z - 1)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(CornerPrefabRO,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 270, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //wall left side
        if (xStep == 0 && zStep > 0 && zStep < size.z - 1)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 90, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //wall right side
        if (xStep == size.x - 1 && zStep > 0 && zStep < size.z - 1)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 270, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //wall bottom side
        if (xStep > 0 && xStep < size.x - 1 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 0, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //wall top side
        if (xStep > 0 && xStep < size.x - 1 && zStep == size.z - 1)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.Euler(0, 180, 0), transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
        //center pieces
        if (xStep > 0 && xStep < size.x - 1 && zStep > 0 && zStep < size.z - 1)
        {
            mazeGrid[location.x + xStep, location.y, location.z + zStep] = Instantiate(MiddlePrefab,
                new Vector3((location.x + xStep) * cellSize, location.y * cellSize * 2, (location.z + zStep) * cellSize) + transform.position,
                Quaternion.identity, transform);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetIndex(location.x + xStep, location.y, location.z + zStep);
            mazeGrid[location.x + xStep, location.y, location.z + zStep].SetType(MazeCell.CellType.Room);
        }
    }
    private void GenerateEmpty()
    {
        for (int mazeHight = 0; mazeHight < mazeSize.y; mazeHight++)
        {
            for (int i = 0; i < emptyAttempts; i++)
            {
                Vector3Int emptySize = new Vector3Int(Random.Range(emptyMinSize.x, emptyMaxSize.x + 1),
                    1,
                    Random.Range(emptyMinSize.y, emptyMaxSize.y + 1));
                Vector3Int emptyLocation = new Vector3Int(
                    Random.Range(0, mazeSize.x - emptySize.x),
                    mazeHight,
                    Random.Range(0, mazeSize.z - emptySize.z));
                if (emptyLocation.x < 2 && emptyLocation.z < 2)
                {
                    emptyLocation.x += 2;
                    emptyLocation.z += 2;
                }
                if (!CheckOverlapping(emptyLocation, emptySize))
                {
                    for (int x = 0; x < emptySize.x; x++)
                    {
                        for (int z = 0; z < emptySize.z; z++)
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
                //yield return new WaitForSeconds(0.1f);
            }
        }
    }
    private void GenerateStairs()
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
                if (!CheckOverlapping(stairsLocation, stairsSize))
                {
                    for (int y = 0; y < 2; y++)
                    {
                        for (int x = 0; x < stairsSize.x; x++)
                        {
                            for (int z = 0; z < stairsSize.z; z++)
                            {
                                mazeGrid[stairsLocation.x + x, stairsLocation.y + y, stairsLocation.z + z] = Instantiate(StairsPlaceholder,
                                    new Vector3((stairsLocation.x + x) * cellSize, (stairsLocation.y + y) * cellSize * 2, (stairsLocation.z + z) * cellSize) + transform.position,
                                    Quaternion.identity, transform);
                                mazeGrid[stairsLocation.x + x, stairsLocation.y + y, stairsLocation.z + z].SetIndex(stairsLocation.x + x, y, stairsLocation.y + z);
                                mazeGrid[stairsLocation.x + x, stairsLocation.y + y, stairsLocation.z + z].SetType(MazeCell.CellType.Stairs);
                                mazeGrid[stairsLocation.x + x, stairsLocation.y + y, stairsLocation.z + z].GetComponent<MazeCell>().distanceToSpawn = stairsVariant;
                                mazeGrid[stairsLocation.x + x, stairsLocation.y + y, stairsLocation.z + z].Visit();
                            }
                        }
                    }
                    CreateStairsEntrance(stairsLocation, stairsVariant);
                    CreateStairs(stairsLocation, stairsVariant);
                    //maxStairsAmount -= 1;
                }
                if (stairsAmount == maxStairsAmount)
                {
                    break;
                }
            }
        }
    }
    private void CreateStairsEntrance(Vector3Int stairsLocation, int stairsVariant)
    {
        Vector3Int entrance1Location;
        Vector3Int entrance2Location;
        if (stairsVariant == 0)
        {
            entrance1Location = new Vector3Int(stairsLocation.x, stairsLocation.y, stairsLocation.z + 4);
            entrance2Location = new Vector3Int(stairsLocation.x, stairsLocation.y + 1, stairsLocation.z);

            InstantiateEntrance(entrance1Location, StairsEntrancePrefab);
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].ClearBackWall();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetEntrance();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetIndex(entrance1Location.x, entrance1Location.y, entrance1Location.z);
            Entrances.Add(mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z]);

            InstantiateEntrance(entrance2Location, StairsEntrancePrefab);
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].ClearFrontWall();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetEntrance();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetIndex(entrance2Location.x, entrance2Location.y, entrance2Location.z);
            Entrances.Add(mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z]);
        }
        if (stairsVariant == 1)
        {
            entrance1Location = new Vector3Int(stairsLocation.x + 4, stairsLocation.y, stairsLocation.z);
            entrance2Location = new Vector3Int(stairsLocation.x, stairsLocation.y + 1, stairsLocation.z);

            InstantiateEntrance(entrance1Location, StairsEntrancePrefab);
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].ClearLeftWall();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetEntrance();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetIndex(entrance1Location.x, entrance1Location.y, entrance1Location.z);
            Entrances.Add(mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z]);

            InstantiateEntrance(entrance2Location, StairsEntrancePrefab);
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].ClearRightWall();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetEntrance();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetIndex(entrance2Location.x, entrance2Location.y, entrance2Location.z);
            Entrances.Add(mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z]);
        }
        if (stairsVariant == 2)
        {
            entrance1Location = new Vector3Int(stairsLocation.x, stairsLocation.y, stairsLocation.z);
            entrance2Location = new Vector3Int(stairsLocation.x, stairsLocation.y + 1, stairsLocation.z + 4);

            InstantiateEntrance(entrance1Location, StairsEntrancePrefab);
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].ClearFrontWall();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetEntrance();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetIndex(entrance1Location.x, entrance1Location.y, entrance1Location.z);
            Entrances.Add(mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z]);

            InstantiateEntrance(entrance2Location, StairsEntrancePrefab);
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].ClearBackWall();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetEntrance();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetIndex(entrance2Location.x, entrance2Location.y, entrance2Location.z);
            Entrances.Add(mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z]);
        }
        if (stairsVariant == 3)
        {
            entrance1Location = new Vector3Int(stairsLocation.x, stairsLocation.y, stairsLocation.z);
            entrance2Location = new Vector3Int(stairsLocation.x + 4, stairsLocation.y + 1, stairsLocation.z);

            InstantiateEntrance(entrance1Location, StairsEntrancePrefab);
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].ClearRightWall();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetEntrance();
            mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z].SetIndex(entrance1Location.x, entrance1Location.y, entrance1Location.z);
            Entrances.Add(mazeGrid[entrance1Location.x, entrance1Location.y, entrance1Location.z]);

            InstantiateEntrance(entrance2Location, StairsEntrancePrefab);
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].ClearLeftWall();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetEntrance();
            mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z].SetIndex(entrance2Location.x, entrance2Location.y, entrance2Location.z);
            Entrances.Add(mazeGrid[entrance2Location.x, entrance2Location.y, entrance2Location.z]);
        }
    }
    private void CreateStairs(Vector3Int stairsLocation, int stairsVariant)
    {
        stairsAmount++;
        if (stairsVariant == 0)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 3].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 3] = Instantiate(StairsPrefab,
                new Vector3(
                    (stairsLocation.x) * cellSize,
                    stairsLocation.y * cellSize * 2, 
                    (stairsLocation.z + 3) * cellSize) + transform.position,
                Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 3].SetType(MazeCell.CellType.Stairs);
            mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 3].Visit();
        }
        if (stairsVariant == 1)
        {
            Destroy(mazeGrid[stairsLocation.x + 3, stairsLocation.y, stairsLocation.z].gameObject);
            mazeGrid[stairsLocation.x + 3, stairsLocation.y, stairsLocation.z] = Instantiate(StairsPrefab,
                new Vector3(
                    (stairsLocation.x + 3) * cellSize,
                    stairsLocation.y * cellSize * 2, 
                    (stairsLocation.z) * cellSize) + transform.position,
                Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x + 3, stairsLocation.y, stairsLocation.z].SetType(MazeCell.CellType.Stairs);
            mazeGrid[stairsLocation.x + 3, stairsLocation.y, stairsLocation.z].Visit();
        }
        if (stairsVariant == 2)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 1].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 1] = Instantiate(StairsPrefab,
                new Vector3(
                    (stairsLocation.x) * cellSize,
                    stairsLocation.y * cellSize * 2, 
                    (stairsLocation.z + 1) * cellSize) + transform.position,
                Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 1].SetType(MazeCell.CellType.Stairs);
            mazeGrid[stairsLocation.x, stairsLocation.y, stairsLocation.z + 1].Visit();
        }
        if (stairsVariant == 3)
        {
            Destroy(mazeGrid[stairsLocation.x + 1, stairsLocation.y, stairsLocation.z].gameObject);
            mazeGrid[stairsLocation.x + 1, stairsLocation.y, stairsLocation.z] = Instantiate(StairsPrefab,
                new Vector3(
                    (stairsLocation.x + 1) * cellSize,
                    stairsLocation.y * cellSize * 2, 
                    (stairsLocation.z) * cellSize) + transform.position,
                Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x + 1, stairsLocation.y, stairsLocation.z].SetType(MazeCell.CellType.Stairs);
            mazeGrid[stairsLocation.x + 1, stairsLocation.y, stairsLocation.z].Visit();
        }
    }
    private void InstantiateEntrance(Vector3Int gridLocation, MazeCell prefab)
    {
        Destroy(mazeGrid[gridLocation.x, gridLocation.y, gridLocation.z].gameObject);
        mazeGrid[gridLocation.x, gridLocation.y, gridLocation.z] = null;
        mazeGrid[gridLocation.x, gridLocation.y, gridLocation.z] = Instantiate(StairsEntrancePrefab,
            new Vector3((gridLocation.x) * cellSize, gridLocation.y * cellSize * 2, (gridLocation.z) * cellSize) + transform.position,
            Quaternion.identity, transform);
    }
    private bool CheckOverlapping(Vector3Int location, Vector3Int size)
    {
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
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void GenerateBossRoom()
    {
        //find the room furthest from spawn at the max y axis
        int roomWithMostDistance = 0;
        int farthestRoomX = mazeSize.x - 1;
        for (int x = 0; x < mazeSize.x; x++)
        {
            if (roomWithMostDistance < mazeGrid[x * distancetoNextCell, 0, mazeSize.z * distancetoNextCell - distancetoNextCell].distanceToSpawn)
            {
                roomWithMostDistance = mazeGrid[x * distancetoNextCell, 0, mazeSize.z * distancetoNextCell - distancetoNextCell].distanceToSpawn;
                farthestRoomX = x;
            }
        }
        //spawn Boss Room
        BossRoom bossRoom = Instantiate(
            BossRoomPrefab, 
            new Vector3(farthestRoomX * cellSize * distancetoNextCell, 0, mazeSize.z * cellSize * distancetoNextCell) + transform.position, 
            Quaternion.identity, transform);
        mazeGrid[farthestRoomX * distancetoNextCell, 0, mazeSize.z * distancetoNextCell - distancetoNextCell].ClearFrontWall();
        //spawn Gap if needed
        if (withGaps)
        {
            MazeCell gapCell = Instantiate(
                MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                new Vector3(bossRoom.transform.position.x, 0, bossRoom.transform.position.z - cellSize) + transform.position,
                Quaternion.identity, transform);
            gapCell.Visit();
            gapCell.ClearFrontWall();
            gapCell.ClearBackWall();
        }
    }
    private void GenerateTreasureRoom()
    {
        TreasureRoom treasureRoom;
        int randZ;
        int intervall;
        int amountPerZSide = (int)(mazeSize.z / cellSize);
        if (withGaps)
        {
            //amountPerZSide *= 2;
        }
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
                new Vector3(mazeSize.x * cellSize * distancetoNextCell, 0, randZ * cellSize * distancetoNextCell) + transform.position,
                Quaternion.identity, transform);  
            treasureRoom.ClearLeftWall();
            mazeGrid[mazeSize.x * distancetoNextCell - distancetoNextCell, 0, randZ * distancetoNextCell].ClearRightWall();

            //spawn Gap if needed
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(
                    MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(treasureRoom.transform.position.x - cellSize, 0, treasureRoom.transform.position.z) + transform.position,
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
            }

            //room on left side
            randZ = Random.Range(intervall * z + 1, intervall * (z + 1));
            treasureRoom = Instantiate(TreasureRoomPrefab,
                new Vector3(-cellSize * distancetoNextCell, 0, randZ * cellSize * distancetoNextCell) + transform.position,
                Quaternion.identity, transform);
            treasureRoom.ClearRightWall();
            mazeGrid[0, 0, randZ * distancetoNextCell].ClearLeftWall();
            
            //spawn Gap if needed
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(treasureRoom.transform.position.x + cellSize, 0, treasureRoom.transform.position.z) + transform.position,
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
            }
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
            if (x + distancetoNextCell < mazeSize.x * distancetoNextCell)
            {
                var cellToRight = mazeGrid[x + distancetoNextCell, y, z];
                if (cellToRight.isVisited == false && cellToRight.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToRight;
                }
            }
            if (x - distancetoNextCell >= 0)
            {
                var cellToLeft = mazeGrid[x - distancetoNextCell, y, z];
                if (cellToLeft.isVisited == false && cellToLeft.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToLeft;
                }
            }
            if (z + distancetoNextCell < mazeSize.z * distancetoNextCell)
            {
                var cellToFront = mazeGrid[x, y, z + distancetoNextCell];
                if (cellToFront.isVisited == false && cellToFront.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToFront;
                }
            }
            if (z - distancetoNextCell >= 0)
            {
                var cellToBack = mazeGrid[x, y, z - distancetoNextCell];
                if (cellToBack.isVisited == false && cellToBack.type != MazeCell.CellType.Hallway)
                {
                    yield return cellToBack;
                }
            }
        }
        else if(currCell.type == MazeCell.CellType.Entrance)
        {
            if (x + distancetoNextCell < mazeSize.x * distancetoNextCell)
            {
                var cellToRight = mazeGrid[x + distancetoNextCell, y, z];
                if (cellToRight.isVisited == false)
                {
                    yield return cellToRight;
                }
            }
            if (x - distancetoNextCell >= 0)
            {
                var cellToLeft = mazeGrid[x - distancetoNextCell, y, z];
                if (cellToLeft.isVisited == false)
                {
                    yield return cellToLeft;
                }
            }
            if (z + distancetoNextCell < mazeSize.z * distancetoNextCell)
            {
                var cellToFront = mazeGrid[x, y, z + distancetoNextCell];
                if (cellToFront.isVisited == false)
                {
                    yield return cellToFront;
                }
            }
            if (z - distancetoNextCell >= 0)
            {
                var cellToBack = mazeGrid[x, y, z - distancetoNextCell];
                if (cellToBack.isVisited == false)
                {
                    yield return cellToBack;
                }
            }
        }
        else
        {
            if (x + distancetoNextCell < mazeSize.x * distancetoNextCell)
            {
                var cellToRight = mazeGrid[x + distancetoNextCell, y, z];
                if (cellToRight.isVisited == false && cellToRight.type != MazeCell.CellType.Room)
                {
                    yield return cellToRight;
                }
            }
            if (x - distancetoNextCell >= 0)
            {
                var cellToLeft = mazeGrid[x - distancetoNextCell, y, z];
                if (cellToLeft.isVisited == false && cellToLeft.type != MazeCell.CellType.Room)
                {
                    yield return cellToLeft;
                }
            }
            if (z + distancetoNextCell < mazeSize.z * distancetoNextCell)
            {
                var cellToFront = mazeGrid[x, y, z + distancetoNextCell];
                if (cellToFront.isVisited == false && cellToFront.type != MazeCell.CellType.Room)
                {
                    yield return cellToFront;
                }
            }
            if (z - distancetoNextCell >= 0)
            {
                var cellToBack = mazeGrid[x, y, z - distancetoNextCell];
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
            if (currCell.type != MazeCell.CellType.Entrance && currCell.Index.y == 0)
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + distancetoNextCell;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(
                    MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x + cellSize, 0, prevCell.transform.position.z), 
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
                gapCell.Index = new Vector3Int(prevCell.Index.x + 1, prevCell.Index.y, prevCell.Index.z);
                gapCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            }
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + distancetoNextCell;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(
                    MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x - cellSize, 0, prevCell.transform.position.z),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
                gapCell.Index = new Vector3Int(prevCell.Index.x - 1, prevCell.Index.y, prevCell.Index.z);
                gapCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            }
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + distancetoNextCell;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(
                    MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x, 0, prevCell.transform.position.z + cellSize),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearFrontWall();
                gapCell.ClearBackWall();
                gapCell.Index = new Vector3Int(prevCell.Index.x, prevCell.Index.y, prevCell.Index.z + 1);
                gapCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            }
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + distancetoNextCell;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(
                    MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x, 0, prevCell.transform.position.z - cellSize),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearFrontWall();
                gapCell.ClearBackWall();
                gapCell.Index = new Vector3Int(prevCell.Index.x, prevCell.Index.y, prevCell.Index.z - 1);
                gapCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            }
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
        if (x + distancetoNextCell < mazeSize.x * distancetoNextCell)
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
        if (z + distancetoNextCell < mazeSize.z * distancetoNextCell)
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
        if (withGaps )
        {
            Vector3 size = new Vector3(mazeSize.x * distancetoNextCell, mazeSize.y * distancetoNextCell * 2, mazeSize.z * distancetoNextCell);
            Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
        }
        else
        {
            Vector3 size = new Vector3(mazeSize.x, mazeSize.y * 2, mazeSize.z);
            Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
        }
    }
}
