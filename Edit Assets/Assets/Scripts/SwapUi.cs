using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapUi : MonoBehaviour
{
    [SerializeField] GameObject[] ButtonGroups;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject BackButton;
    private int position = 0;

    // Start is called before the first frame update
    void Start()
    {
        NextButton.SetActive(true);
        BackButton.SetActive(false);
        ButtonGroups[0].SetActive(true);
        for (int i = 1; i < ButtonGroups.Length; i++)
        {
            ButtonGroups[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextGroup()
    {
        ButtonGroups[position].SetActive(false);
        if (position < ButtonGroups.Length - 1)
        {
            position++;
        }
        if (position == ButtonGroups.Length - 1)
        {
            NextButton.SetActive(false);
        }
        if (!BackButton.activeSelf)
        {
            BackButton.SetActive(true);
        }
        ButtonGroups[position].SetActive(true);
    }
    public void PrevioustGroup()
    {
        ButtonGroups[position].SetActive(false);
        if (position > 0)
        {
            position--;
        }
        if (position == 0)
        {
            BackButton.SetActive(false);
        }
        if (!NextButton.activeSelf)
        {
            NextButton.SetActive(true);
        }
        ButtonGroups[position].SetActive(true);
    }
}
