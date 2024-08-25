using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour
{
    public Vector3Int staircasePosition;
    public Vector3Int staircaseSize;
    public List<MazeCell> entranceCells;
    public MazeCell GetEntrance(int y)
    {
        for (int i = 0; i < entranceCells.Count; i++)
        {
            if (entranceCells[i].Index.y == y)
            {
                return entranceCells[i];
            }
        }
        return null;
    }
}
