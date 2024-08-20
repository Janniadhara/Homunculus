using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : MonoBehaviour
{
    [SerializeField] private GameObject LeftWall;
    [SerializeField] private GameObject RightWall;
    [SerializeField] private GameObject FrontWall;
    [SerializeField] private GameObject BackWall;

    public void ClearLeftWall()
    {
        LeftWall.SetActive(false);
    }
    public void ClearRightWall()
    {
        RightWall.SetActive(false);
    }
    public void ClearFrontWall()
    {
        FrontWall.SetActive(false);
    }
    public void ClearBackWall()
    {
        BackWall.SetActive(false);
    }
}
