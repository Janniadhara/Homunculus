using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.FilePathAttribute;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell[] MazeCellPrefab;
    public MazeCell FarthestCell;
    [SerializeField] private BossRoom BossRoomPrefab;
    [SerializeField] private TreasureRoom TreasureRoomPrefab;
    [SerializeField] private int mazeWidth; //x
    [SerializeField] private int mazeDepth; //z
    private MazeCell[,] mazeGrid;
    [SerializeField] private int cellSize;
    [SerializeField] private bool withGaps;
    private int distanceToZero;

    [Header("Rooms")]
    [SerializeField] private bool generateWithRooms;
    [SerializeField] private int roomCount;
    [SerializeField] private MazeCell MazeRoomPrefab;
    [SerializeField] private MazeCell CornerPrefabLU;
    [SerializeField] private MazeCell CornerPrefabLO;
    [SerializeField] private MazeCell CornerPrefabRU;
    [SerializeField] private MazeCell CornerPrefabRO;
    [SerializeField] private MazeCell WallPrefab;
    [SerializeField] private MazeCell MiddlePrefab;
    [SerializeField] Vector2Int roomMinSize;
    [SerializeField] Vector2Int roomMaxSize;


    IEnumerator Start()
    {
        if (withGaps)
        {
            cellSize *= 2;
        }
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];
        if (generateWithRooms)
        {
            GenerateRooms();
        }
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                if (mazeGrid[x, z] == null)
                {
                    mazeGrid[x, z] = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                        new Vector3(x * cellSize, 0, z * cellSize), Quaternion.identity, transform);
                }
            }
        }

        yield return GenerateMaze(null, mazeGrid[0, 0]);
        GenerateBossRoom();
        GenerateTreasureRoom();
    }
    private void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
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
    private void GenerateRoomTile(Vector2Int location, Vector2Int size, int xStep, int zStep)
    {
        //corner l-u
        if (xStep == 0 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabLU,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 90, 0), transform);
        }
        //corner l-o
        if (xStep == 0 && zStep == size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabLO,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 180, 0), transform);
        }
        //corner r-u
        if (xStep == size.x - 1 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabRU,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 0, 0), transform);
        }
        //corner r-o
        if (xStep == size.x - 1 && zStep == size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(CornerPrefabRO,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 270, 0), transform);
        }
        //wall left side
        if (xStep == 0 && zStep > 0 && zStep < size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 90, 0), transform);
        }
        //wall right side
        if (xStep == size.x - 1 && zStep > 0 && zStep < size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 270, 0), transform);
        }
        //wall bottom side
        if (xStep > 0 && xStep < size.x - 1 && zStep == 0)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 0, 0), transform);
        }
        //wall top side
        if (xStep > 0 && xStep < size.x - 1 && zStep == size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(WallPrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.Euler(0, 180, 0), transform);
        }
        //center pieces
        if (xStep > 0 && xStep < size.x - 1 && zStep > 0 && zStep < size.y - 1)
        {
            mazeGrid[location.x + xStep, location.y + zStep] = Instantiate(MiddlePrefab,
                new Vector3((location.x + xStep) * cellSize, 0, (location.y + zStep) * cellSize),
                Quaternion.identity, transform);
        }
    }
    private void GenerateBossRoom()
    {
        //find the room furthest from spawn at the max y axis
        int roomWithMostDistance = 0;
        int farthestRoomX = mazeWidth - 1;
        for (int x = 0; x < mazeWidth; x++)
        {
            if (roomWithMostDistance < mazeGrid[x, mazeDepth - 1].distanceToSpawn)
            {
                roomWithMostDistance = mazeGrid[x, mazeDepth - 1].distanceToSpawn;
                farthestRoomX = x;
            }
        }
        //spawn Boss Room
        BossRoom bossRoom = Instantiate(BossRoomPrefab, new Vector3(farthestRoomX * cellSize, 0, mazeDepth * cellSize), 
            Quaternion.identity, transform);
        mazeGrid[farthestRoomX, mazeDepth-1].ClearFrontWall();
        //spawn Gap if needed
        if (withGaps)
        {
            MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                new Vector3(bossRoom.transform.position.x, 0, bossRoom.transform.position.z - cellSize / 2),
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
        int amountPerZSide = mazeDepth / cellSize;
        if (withGaps)
        {
            amountPerZSide *= 2;
        }
        int intervall = mazeDepth / amountPerZSide;
        for (int z = 0; z < amountPerZSide; z++)
        {
            //room on right side
            randZ = Random.Range(intervall * z, intervall * (z + 1));
            treasureRoom = Instantiate(TreasureRoomPrefab,
                new Vector3(mazeWidth * cellSize, 0, randZ * cellSize),
                Quaternion.identity, transform);  
            treasureRoom.ClearLeftWall();
            mazeGrid[mazeWidth - 1, randZ].ClearRightWall();

            //spawn Gap if needed
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(treasureRoom.transform.position.x - cellSize / 2, 0, treasureRoom.transform.position.z),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
            }

            //room on left side
            randZ = Random.Range(intervall * z + 1, intervall * (z + 1));
            treasureRoom = Instantiate(TreasureRoomPrefab,
                new Vector3(-cellSize, 0, randZ * cellSize),
                Quaternion.identity, transform);
            treasureRoom.ClearRightWall();
            mazeGrid[0, randZ].ClearLeftWall();
            
            //spawn Gap if needed
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(treasureRoom.transform.position.x + cellSize / 2, 0, treasureRoom.transform.position.z),
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
        int x = (int)currCell.transform.position.x / cellSize;
        int z = (int)currCell.transform.position.z / cellSize;

        if (x+1 < mazeWidth)
        {
            var cellToRight = mazeGrid[x+1, z];
            if (cellToRight.isVisited == false)
            {
                yield return cellToRight;
            }
        }
        if (x-1 >= 0)
        {
            var cellToLeft = mazeGrid[x-1, z];
            if (cellToLeft.isVisited == false)
            {
                yield return cellToLeft;
            }
        }
        if (z+1 < mazeDepth)
        {
            var cellToFront = mazeGrid[x, z+1];
            if (cellToFront.isVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0)
        {
            var cellToBack = mazeGrid[x, z-1];
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn +1;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x + cellSize/2, 0, prevCell.transform.position.z), 
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x - cellSize/2, 0, prevCell.transform.position.z),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearLeftWall();
                gapCell.ClearRightWall();
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x, 0, prevCell.transform.position.z + cellSize / 2),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearFrontWall();
                gapCell.ClearBackWall();
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
            currCell.distanceToSpawn = prevCell.distanceToSpawn + 1;
            if (withGaps)
            {
                MazeCell gapCell = Instantiate(MazeCellPrefab[Random.Range(0, MazeCellPrefab.Length)],
                    new Vector3(prevCell.transform.position.x, 0, prevCell.transform.position.z - cellSize / 2),
                    Quaternion.identity, transform);
                gapCell.Visit();
                gapCell.ClearFrontWall();
                gapCell.ClearBackWall();
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
        Vector3 size = new Vector3(mazeWidth, 1, mazeDepth);
        Gizmos.DrawWireCube(transform.position + size * 3, size * 6);
    }
}
