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
    [Header("Walls")]
    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;
    [SerializeField] private GameObject FrontWall;
    [SerializeField] private GameObject BackWall;
    [Header("Doors")]
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private GameObject FrontDoor;
    [SerializeField] private GameObject BackDoor;
    [Header("Pillars")]
    [SerializeField] private GameObject LeftPillar;
    [SerializeField] private GameObject RightPillar;
    [SerializeField] private GameObject FrontPillar;
    [SerializeField] private GameObject BackPillar;
    [Header("Rest")]
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
        if (LeftPillar != null)
        {
            LeftPillar.SetActive(true);
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
        if (RightPillar != null)
        {
            RightPillar.SetActive(true);
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
        if (FrontPillar != null)
        {
            FrontPillar.SetActive(true);
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
        if (BackPillar != null)
        {
            BackPillar.SetActive(true);
        }
    }
    public void SetEntrance()
    {
        isVisited = false;
        isEntrance = true;
        type = CellType.Entrance;
        if (EntranceBlock != null)
        {
            EntranceBlock.SetActive(true);
        }
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
