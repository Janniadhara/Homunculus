using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3Int roomPosition;
    public Vector3Int roomSize;
    public List<MazeCell> borderCells;
    public List<MazeCell> entranceCells;
    public MazeCell GetRandomBorderCell(MazeCell entrance)
    {
        int randCell = Random.Range(0, borderCells.Count);
        entrance = borderCells[randCell];
        entranceCells.Add(entrance);
        return entrance;
    }
    public MazeCell GetRandomEntrance()
    {
        int randEntrance = Random.Range(0, entranceCells.Count);
        return entranceCells[randEntrance];
    }
}
