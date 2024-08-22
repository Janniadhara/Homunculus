using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public enum CellType
    {
        None,
        Room,
        Hallway,
        Stairs,
        EmptySpace,
        Entrance
    }
    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;
    [SerializeField] private GameObject FrontWall;
    [SerializeField] private GameObject BackWall;
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private GameObject FrontDoor;
    [SerializeField] private GameObject BackDoor;
    [SerializeField] private GameObject UnvisitedBlock;
    [SerializeField] private GameObject EntranceBlock;
    private bool isEntrance;

    public bool isVisited { get; private set; }
    public Vector3Int Index;
    public int distanceToSpawn;
    public CellType type;

    public void Visit()
    {
        isVisited = true;
        if (UnvisitedBlock != null)
        {
            UnvisitedBlock.SetActive(false);
        }
        if (EntranceBlock != null)
        {
            EntranceBlock.SetActive(false);
        }
    }
    public void ClearLeftWall()
    {
        if (LeftWall != null)
        {
            LeftWall.SetActive(false);
        }
        if (isEntrance && LeftDoor != null)
        {
            LeftDoor.SetActive(true);
        }
    }
    public void ClearRightWall()
    {
        if (RightWall != null)
        {
            RightWall.SetActive(false);
        }
        if (isEntrance && RightDoor != null)
        {
            RightDoor.SetActive(true);
        }
    }
    public void ClearFrontWall()
    {
        if (FrontWall != null)
        {
            FrontWall.SetActive(false);
        }
        if (isEntrance && FrontDoor != null)
        {
            FrontDoor.SetActive(true);
        }
    }
    public void ClearBackWall()
    {
        if (BackWall != null)
        {
            BackWall.SetActive(false);
        }
        if (isEntrance && BackDoor != null)
        {
            BackDoor.SetActive(true);
        }
    }
    public void SetEntrance()
    {
        isVisited = false;
        isEntrance = true;
        type = CellType.Entrance;
        EntranceBlock.SetActive(true);
    }
    public void SetIndex(int x , int y, int z)
    {
        Index.x = x; 
        Index.y = y;
        Index.z = z;
    }
    public void SetType(MazeCell.CellType type)
    {
        this.type = type;
    }
}
