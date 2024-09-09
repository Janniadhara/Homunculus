using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour
{
    public Vector3Int staircasePosition;
    public Vector3Int staircaseSize;
    public MazeCell[] entranceCells;
    public GameObject[] XRows;
    public MazeCell GetEntrance()
    {
        return entranceCells[Random.Range(0, entranceCells.Length)];
    }
}
