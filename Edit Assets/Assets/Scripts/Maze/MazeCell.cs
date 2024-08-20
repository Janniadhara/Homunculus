using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;
    [SerializeField] private GameObject FrontWall;
    [SerializeField] private GameObject BackWall;
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private GameObject FrontDoor;
    [SerializeField] private GameObject BackDoor;
    [SerializeField] private GameObject UnvisitedBlock;
    private bool isEntrance;

    public bool isVisited { get; private set; }
    public int distanceToSpawn;

    public void Visit()
    {
        isVisited = true;
        UnvisitedBlock.SetActive(false);
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
        UnvisitedBlock.SetActive(true);
    }
}
