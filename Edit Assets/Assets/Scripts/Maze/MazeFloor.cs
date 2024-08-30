using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MazeFloor : MonoBehaviour
{
    [SerializeField] private Transform StaircasesParentTransform;
    [SerializeField] private Transform RoomsParentTransform;
    [SerializeField] private Transform HallwaysParentTransform;
    [SerializeField] private Transform EmptyParentTransform;
    [SerializeField] private GameObject TriggerAreaUp;
    [SerializeField] private GameObject TriggerAreaDown;
    public MazeFloor prevFloor;
    public MazeFloor nextFloor;
    //    location: (transform.position.x + mazeSize.x) * cellSize / 2, 
    //              transform.position.y + cellSize * 1.5, 
    //              (transform.position.z + mazeSize.z) * cellSize / 2), 
    //    scale: mazeSize.x * cellSize,
    //           2,
    //           mazeSize.z * cellSize)
    public void SetTrigger(Vector3Int mazeSize, float cellSize)
    {
        TriggerAreaUp.transform.position = new Vector3(
            (transform.position.x + mazeSize.x) * cellSize / 2 - cellSize / 2,
            transform.position.y + cellSize * 1.9f,
            (transform.position.z + mazeSize.z) * cellSize / 2 - cellSize / 2);
        TriggerAreaUp.transform.localScale = new Vector3(
            mazeSize.x * cellSize / transform.parent.localScale.x,
            0.1f,
            mazeSize.z * cellSize / transform.parent.localScale.z);
        TriggerAreaDown.transform.position = new Vector3(
            (transform.position.x + mazeSize.x) * cellSize / 2 - cellSize / 2,
            transform.position.y + cellSize * 1.3f,
            (transform.position.z + mazeSize.z) * cellSize / 2 - cellSize / 2);
        TriggerAreaDown.transform.localScale = new Vector3(
            mazeSize.x * cellSize / transform.parent.localScale.x,
            0.1f,
            mazeSize.z * cellSize / transform.parent.localScale.z);
    }
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
    public void EnablePrevFloor()
    {
        if (prevFloor != null)
        {
            prevFloor.gameObject.SetActive(true);
        }
    }
    public void DisablePrevFloor()
    {
        if (prevFloor != null)
        {
            prevFloor.gameObject.SetActive(false);
        }
    }
    public void EnableNextFloor()
    {
        if (nextFloor != null)
        {
            nextFloor.gameObject.SetActive(true);
        }
    }
    public void DisableNextFloor()
    {
        if (nextFloor != null)
        {
            nextFloor.gameObject.SetActive(false);
        }
    }
}
