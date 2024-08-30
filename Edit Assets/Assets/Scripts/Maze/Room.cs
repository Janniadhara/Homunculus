using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3Int roomPosition;
    public Vector3Int roomSize;
    public MazeCell[] entranceCells;
    public GameObject[] XRows;
    public MazeCell GetRandomEntrance()
    {
        return entranceCells[Random.Range(0, entranceCells.Length)];
    }
}
