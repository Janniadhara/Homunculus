using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] private RoomCell CornerPrefab;
    [SerializeField] private RoomCell WallPrefab;
    [SerializeField] private RoomCell MiddlePrefab;
    public int roomWidth; //x
    public int roomDepth; //z
    private RoomCell[,] roomGrid;
    [SerializeField] private int cellSize;
    public void GenerateRoom()
    {
        roomGrid = new RoomCell[roomWidth, roomDepth];
        for (int x = 0; x < roomWidth; x++)
        {
            for (int z = 0; z < roomDepth; z++)
            {
                //corners
                if (x == 0 && z == 0)
                {
                    roomGrid[x, z] = Instantiate(CornerPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 90, 0), transform);
                }
                if (x == 0 && z == roomDepth - 1)
                {
                    roomGrid[x, z] = Instantiate(CornerPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 180, 0), transform);
                }
                if (x == roomWidth - 1 && z == 0)
                {
                    roomGrid[x, z] = Instantiate(CornerPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 0, 0), transform);
                }
                if (x == roomWidth - 1 && z == roomDepth - 1)
                {
                    roomGrid[x, z] = Instantiate(CornerPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 270, 0), transform);
                }
                //wall left side
                if (x == 0 && z > 0 && z < roomDepth - 1)
                {
                    roomGrid[x, z] = Instantiate(WallPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 90, 0), transform);
                }
                //wall right side
                if (x == roomWidth - 1 && z > 0 && z < roomDepth - 1)
                {
                    roomGrid[x, z] = Instantiate(WallPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 270, 0), transform);
                }
                //wall bottom side
                if (x > 0 && x < roomWidth - 1 && z == 0)
                {
                    roomGrid[x, z] = Instantiate(WallPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 0, 0), transform);
                }
                //wall top side
                if (x > 0 && x < roomWidth - 1 && z == roomDepth - 1)
                {
                    roomGrid[x, z] = Instantiate(WallPrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.Euler(0, 180, 0), transform);
                }
                //center pieces
                if (x > 0 && x < roomWidth - 1 && z > 0 && z < roomDepth - 1)
                {
                    roomGrid[x, z] = Instantiate(MiddlePrefab, 
                        new Vector3(transform.position.x + x * cellSize, transform.position.y, transform.position.z + z * cellSize),
                        Quaternion.identity, transform);
                }
            }
        }
    }
    private void Start()
    {
        //GenerateRoom();
    }
}
