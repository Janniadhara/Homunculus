using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFloor : MonoBehaviour
{
    [SerializeField] private Transform StaircasesParentTransform;
    [SerializeField] private Transform RoomsParentTransform;
    [SerializeField] private Transform HallwaysParentTransform;
    [SerializeField] private Transform EmptyParentTransform;
    public MazeFloor prevFloor;
    public MazeFloor nextFloor;

    public void DeleteUnusedSpace()
    {
        for (int i = 0; i < EmptyParentTransform.childCount; i++)
        {
            Destroy(EmptyParentTransform.GetChild(i).gameObject);
        }
        for (int j = 0; j < HallwaysParentTransform.childCount; j++)
        {
            if (!HallwaysParentTransform.GetChild(j).GetComponent<MazeCell>().isVisited)
            {
                Destroy(HallwaysParentTransform.GetChild(j).gameObject);
            }
        }
    }
    public void ConnectPrev(MazeFloor prevFloor)
    {
        this.prevFloor = prevFloor; 
    }
    public void ConnectNext(MazeFloor nextFloor)
    {
        this.nextFloor = nextFloor;
    }
    public void DisaplePrevFloor()
    {
        if (prevFloor.gameObject.activeSelf)
        {
            prevFloor.gameObject.SetActive(false);
        }
        else
        {
            prevFloor.gameObject.SetActive(true);
        }
    }
    public void EnableNextFloor()
    {
        if (nextFloor.gameObject.activeSelf)
        {
            nextFloor.gameObject.SetActive(false);
        }
        else
        {
            nextFloor.gameObject.SetActive(true);
        }
    }
}
