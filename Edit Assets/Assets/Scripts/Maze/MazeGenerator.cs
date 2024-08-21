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
    [SerializeField] private int mazeWidth; //x
    [SerializeField] private int mazeDepth; //z
    private MazeCell[,] mazeGrid;
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

    public bool isValid;


    IEnumerator Start()
    {
        isValid = true;
        distancetoNextCell = 1;
        cellSize *= transform.localScale.x;
        if (withGaps)
        {
            distancetoNextCell = 2;
        }
        mazeGrid = new MazeCell[mazeWidth * distancetoNextCell, mazeDepth * distancetoNextCell];
        if (generateWithEmpty)
        {
            yield return GenerateEmpty();
        }
        if (generateWithRooms)
        {
            yield return GenerateRooms();
        }
        if (generateWithStairs)
        {
            yield return GenerateStairs();
        }
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                if (mazeGrid[x * distancetoNextCell, z * distancetoNextCell] == null)
                {
                    mazeGrid[x * distancetoNextCell, z * distancetoNextCell] = Instantiate(
                        MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                        new Vector3(x * cellSize * distancetoNextCell, 0, z * cellSize * distancetoNextCell) + transform.position,
                        Quaternion.identity, transform
                        );
                    mazeGrid[x * distancetoNextCell, z * distancetoNextCell].GetComponent<MazeCell>().Index = 
                        new Vector2Int(x * distancetoNextCell, z * distancetoNextCell);
                    mazeGrid[x * distancetoNextCell, z * distancetoNextCell].GetComponent<MazeCell>().type = 
                        MazeCell.CellType.Hallway;
                }
            }
        }

        yield return GenerateMaze(null, mazeGrid[0, 0]);
        GenerateBossRoom();
        GenerateTreasureRoom();
        if (!isValid)
        {
            Debug.LogWarning("Maze not valid");
        }
    }
    private IEnumerator GenerateRooms()
    {
        for (int i = 0; i < roomAttempts; i++)
        {
            Vector2Int roomSize = new Vector2Int(Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                Random.Range(roomMinSize.y, roomMaxSize.y + 1));
            Vector2Int roomLocation = new Vector2Int(Random.Range(0, mazeWidth - roomMaxSize.x),
                Random.Range(0, mazeDepth - roomMaxSize.y));
            if (!CheckOverlapping(roomLocation, roomSize))
            {
                for (int x = 0; x < roomSize.x; x++)
                {
                    for (int z = 0; z < roomSize.y; z++)
                    {
                        GenerateRoomTile(roomLocation, roomSize, x, z);
                        //mazeGrid[roomLocation.x + x, roomLocation.y + z] = Instantiate(MazeRoomPrefab,
                        //        new Vector3((roomLocation.x + x) * cellSize, 0, (roomLocation.y + z) * cellSize),
                        //        Quaternion.identity, transform);
                        mazeGrid[roomLocation.x + x, roomLocation.y + z].Visit();
                    }
                }
                mazeGrid[roomLocation.x, roomLocation.y].SetEntrance();
                mazeGrid[roomLocation.x + roomSize.x - 1, roomLocation.y + roomSize.y - 1].SetEntrance();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void GenerateRoomTile(Vector2Int location, Vector2Int size, int xStep, int zStep)
    {
        //corner l-u
        if (xStep == 0 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabLU,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 90, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //corner l-o
        if (xStep == 0 && zStep == size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabLO,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 180, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //corner r-u
        if (xStep == size.x - 1 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabRU,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 0, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //corner r-o
        if (xStep == size.x - 1 && zStep == size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabRO,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 270, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //wall left side
        if (xStep == 0 && zStep > 0 && zStep < size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 90, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //wall right side
        if (xStep == size.x - 1 && zStep > 0 && zStep < size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 270, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //wall bottom side
        if (xStep > 0 && xStep < size.x - 1 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 0, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //wall top side
        if (xStep > 0 && xStep < size.x - 1 && zStep == size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 180, 0), transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
        //center pieces
        if (xStep > 0 && xStep < size.x - 1 && zStep > 0 && zStep < size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(MiddlePrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.identity, transform);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().Index =
                new Vector2Int(location.x + xStep, location.y + zStep);
            mazeGrid[location.x + xStep, location.y + zStep].GetComponent<MazeCell>().type = MazeCell.CellType.Room;
        }
    }
    private IEnumerator GenerateEmpty()
    {
        for (int i = 0; i < emptyAttempts; i++)
        {
            Vector2Int emptySize = new Vector2Int(Random.Range(emptyMinSize.x, emptyMaxSize.x + 1),
                Random.Range(emptyMinSize.y, emptyMaxSize.y + 1));
            Vector2Int emptyLocation = new Vector2Int(Random.Range(4, mazeWidth - emptyMaxSize.x),
                Random.Range(4, mazeDepth - emptyMaxSize.y));
            if (!CheckOverlapping(emptyLocation, emptySize))
            {
                for (int x = 0; x < emptySize.x; x++)
                {
                    for (int z = 0; z < emptySize.y; z++)
                    {
                        mazeGrid[emptyLocation.x + x, emptyLocation.y + z] = Instantiate(EmptySpacePrefab,
                            new Vector3((emptyLocation.x + x) * cellSize, 0, (emptyLocation.y + z) * cellSize),
                            Quaternion.identity, transform);
                        mazeGrid[emptyLocation.x + x, emptyLocation.y + z].GetComponent<MazeCell>().Index =
                            new Vector2Int(emptyLocation.x + x, emptyLocation.y + z);
                        mazeGrid[emptyLocation.x + x, emptyLocation.y + z].GetComponent<MazeCell>().type
                        = MazeCell.CellType.EmptySpace;
                        mazeGrid[emptyLocation.x + x, emptyLocation.y + z].Visit();
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator GenerateStairs()
    {
        for (int i = 0; i < stairsAttempts; i++)
        {
            Vector2Int stairsLocation = new Vector2Int(Random.Range(5, mazeWidth - 5),
            Random.Range(5, mazeDepth - 5));
            int stairsVariant = Random.Range(0, 5);
            Vector2Int stairsSize = new Vector2Int();
            if (stairsVariant == 0 || stairsVariant == 2)
            {
                stairsSize = new Vector2Int(1, 5);
            }
            if (stairsVariant == 1 || stairsVariant == 3)
            {
                stairsSize = new Vector2Int(5, 1);
            }
            if (!CheckOverlapping(stairsLocation, stairsSize))
            {
                for (int x = 0; x < stairsSize.x; x++)
                {
                    for (int z = 0; z < stairsSize.y; z++)
                    {
                        mazeGrid[stairsLocation.x + x, stairsLocation.y + z] = Instantiate(StairsPlaceholder,
                            new Vector3((stairsLocation.x + x) * cellSize, 0, (stairsLocation.y + z) * cellSize),
                            Quaternion.identity, transform);
                        mazeGrid[stairsLocation.x + x, stairsLocation.y + z].GetComponent<MazeCell>().Index =
                            new Vector2Int(stairsLocation.x + x, stairsLocation.y + z);
                        mazeGrid[stairsLocation.x + x, stairsLocation.y + z].GetComponent<MazeCell>().type
                        = MazeCell.CellType.Stairs;
                        mazeGrid[stairsLocation.x + x, stairsLocation.y + z].GetComponent<MazeCell>().distanceToSpawn = stairsVariant;
                        mazeGrid[stairsLocation.x + x, stairsLocation.y + z].Visit();
                    }
                }
                CreateStairsEntrance(stairsLocation, stairsVariant);
                CreateStairs(stairsLocation, stairsVariant);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void CreateStairsEntrance(Vector2Int stairsLocation, int stairsVariant)
    {
        if (stairsVariant == 0)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y + 4].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y + 4] = Instantiate(StairsEntrancePrefab,
            new Vector3((stairsLocation.x) * cellSize, 0, (stairsLocation.y + 4) * cellSize),
            Quaternion.identity, transform);
            mazeGrid[stairsLocation.x, stairsLocation.y + 4].ClearBackWall();
        }
        if (stairsVariant == 1)
        {
            Destroy(mazeGrid[stairsLocation.x + 4, stairsLocation.y].gameObject);
            mazeGrid[stairsLocation.x + 4, stairsLocation.y] = Instantiate(StairsEntrancePrefab,
            new Vector3((stairsLocation.x + 4) * cellSize, 0, (stairsLocation.y) * cellSize),
            Quaternion.identity, transform);
            mazeGrid[stairsLocation.x + 4, stairsLocation.y].ClearLeftWall();
        }
        if (stairsVariant == 2)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y] = Instantiate(StairsEntrancePrefab,
            new Vector3((stairsLocation.x) * cellSize, 0, (stairsLocation.y) * cellSize),
            Quaternion.identity, transform);
            mazeGrid[stairsLocation.x, stairsLocation.y].ClearFrontWall();
        }
        if (stairsVariant == 3)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y] = Instantiate(StairsEntrancePrefab,
            new Vector3((stairsLocation.x) * cellSize, 0, (stairsLocation.y) * cellSize),
            Quaternion.identity, transform);
            mazeGrid[stairsLocation.x, stairsLocation.y].ClearRightWall();
        }
    }
    private void CreateStairs(Vector2Int stairsLocation, int stairsVariant)
    {
        if (stairsVariant == 0)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y + 3].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y + 3] = Instantiate(StairsPrefab,
            new Vector3((stairsLocation.x) * cellSize, 0, (stairsLocation.y + 3) * cellSize),
            Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x, stairsLocation.y + 3].GetComponent<MazeCell>().type
            = MazeCell.CellType.Stairs;
            mazeGrid[stairsLocation.x, stairsLocation.y + 3].Visit();
        }
        if (stairsVariant == 1)
        {
            Destroy(mazeGrid[stairsLocation.x + 3, stairsLocation.y].gameObject);
            mazeGrid[stairsLocation.x + 3, stairsLocation.y] = Instantiate(StairsPrefab,
            new Vector3((stairsLocation.x + 3) * cellSize, 0, (stairsLocation.y) * cellSize),
            Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x + 3, stairsLocation.y].GetComponent<MazeCell>().type
            = MazeCell.CellType.Stairs;
            mazeGrid[stairsLocation.x + 3, stairsLocation.y].Visit();
        }
        if (stairsVariant == 2)
        {
            Destroy(mazeGrid[stairsLocation.x, stairsLocation.y + 1].gameObject);
            mazeGrid[stairsLocation.x, stairsLocation.y + 1] = Instantiate(StairsPrefab,
            new Vector3((stairsLocation.x) * cellSize, 0, (stairsLocation.y + 1) * cellSize),
            Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x, stairsLocation.y + 1].GetComponent<MazeCell>().type
            = MazeCell.CellType.Stairs;
            mazeGrid[stairsLocation.x, stairsLocation.y + 1].Visit();
        }
        if (stairsVariant == 3)
        {
            Destroy(mazeGrid[stairsLocation.x + 1, stairsLocation.y].gameObject);
            mazeGrid[stairsLocation.x + 1, stairsLocation.y] = Instantiate(StairsPrefab,
            new Vector3((stairsLocation.x + 1) * cellSize, 0, (stairsLocation.y) * cellSize),
            Quaternion.Euler(0, 90 * stairsVariant, 0), transform);
            mazeGrid[stairsLocation.x + 1, stairsLocation.y].GetComponent<MazeCell>().type
            = MazeCell.CellType.Stairs;
            mazeGrid[stairsLocation.x + 1, stairsLocation.y].Visit();
        }
    }
    private bool CheckOverlapping(Vector2Int location, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                if (mazeGrid[location.x + x, location.y + z] != null)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void GenerateBossRoom()
    {
        //find the room furthest from spawn at the max y axis
        int roomWithMostDistance = 0;
        int farthestRoomX = mazeWidth - 1;
        for (int x = 0; x < mazeWidth; x++)
        {
            if (roomWithMostDistance < mazeGrid[x * distancetoNextCell, mazeDepth * distancetoNextCell - distancetoNextCell].distanceToSpawn)
            {
                roomWithMostDistance = mazeGrid[x * distancetoNextCell, mazeDepth * distancetoNextCell - distancetoNextCell].distanceToSpawn;
                farthestRoomX = x;
            }
        }
        //spawn Boss Room
        BossRoom bossRoom = Instantiate(
            BossRoomPrefab, 
            new Vector3(farthestRoomX * cellSize * distancetoNextCell, 0, mazeDepth * cellSize * distancetoNextCell) + transform.position, 
            Quaternion.identity, transform);
        mazeGrid[farthestRoomX * distancetoNextCell, mazeDepth * distancetoNextCell - distancetoNextCell].ClearFrontWall();
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
        int amountPerZSide = (int)(mazeDepth / cellSize);
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
            intervall = mazeDepth / amountPerZSide;
        }
        for (int z = 0; z < amountPerZSide; z++)
        {
            //room on right side
            randZ = Random.Range(intervall * z, intervall * (z + 1));
            treasureRoom = Instantiate(
                TreasureRoomPrefab,
                new Vector3(mazeWidth * cellSize * distancetoNextCell, 0, randZ * cellSize * distancetoNextCell) + transform.position,
                Quaternion.identity, transform);  
            treasureRoom.ClearLeftWall();
            mazeGrid[mazeWidth * distancetoNextCell - distancetoNextCell, randZ * distancetoNextCell].ClearRightWall();

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
            mazeGrid[0, randZ * distancetoNextCell].ClearLeftWall();
            
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
        int z = currCell.Index.y;

        if (x + distancetoNextCell < mazeWidth * distancetoNextCell)
        {
            var cellToRight = mazeGrid[x + distancetoNextCell, z];
            if (cellToRight.isVisited == false)
            {
                yield return cellToRight;
            }
        }
        if (x - distancetoNextCell >= 0)
        {
            var cellToLeft = mazeGrid[x - distancetoNextCell, z];
            if (cellToLeft.isVisited == false)
            {
                yield return cellToLeft;
            }
        }
        if (z + distancetoNextCell < mazeDepth * distancetoNextCell)
        {
            var cellToFront = mazeGrid[x, z + distancetoNextCell];
            if (cellToFront.isVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - distancetoNextCell >= 0)
        {
            var cellToBack = mazeGrid[x, z - distancetoNextCell];
            if (cellToBack.isVisited == false)
            {
                yield return cellToBack;
            }
        }
    }
    private void ClearWalls(MazeCell prevCell, MazeCell currCell)
    {
        if (prevCell == null)
        {
            currCell.ClearLeftWall();
            currCell.distanceToSpawn = 0;
            FarthestCell = currCell;
            return;
        }
        if (prevCell.transform.position.x < currCell.transform.position.x)
        {
            prevCell.ClearRightWall();
            currCell.ClearLeftWall();
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
                gapCell.Index = new Vector2Int(prevCell.Index.x + 1, prevCell.Index.y);
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
            prevCell.ClearLeftWall();
            currCell.ClearRightWall();
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
                gapCell.Index = new Vector2Int(prevCell.Index.x - 1, prevCell.Index.y);
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
            prevCell.ClearFrontWall();
            currCell.ClearBackWall();
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
                gapCell.Index = new Vector2Int(prevCell.Index.x, prevCell.Index.y + 1);
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
            prevCell.ClearBackWall();
            currCell.ClearFrontWall();
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
                gapCell.Index = new Vector2Int(prevCell.Index.x, prevCell.Index.y - 1);
                gapCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            }
            if (FarthestCell.distanceToSpawn < currCell.distanceToSpawn)
            {
                FarthestCell = currCell;
            }
            return;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (withGaps )
        {
            Vector3 size = new Vector3(mazeWidth * distancetoNextCell, 1, mazeDepth * distancetoNextCell);
            Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
        }
        else
        {
            Vector3 size = new Vector3(mazeWidth, 1, mazeDepth);
            Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
        }
    }
}
