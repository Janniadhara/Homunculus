using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModularArmorsCharacterCreator : MonoBehaviour, IDataPersistence
{
    [SerializeField] GameObject[] Model;
    //female armors
    [SerializeField] GameObject[] FeKnightArmor;
    [SerializeField] GameObject[] FeAssassinArmor;
    [SerializeField] GameObject[] FeVillagerArmor;
    [SerializeField] GameObject[] FeNakedArmor;
    [SerializeField] GameObject FeHair;
    //male armors
    [SerializeField] GameObject[] MaKnightArmor;
    [SerializeField] GameObject[] MaAssassinArmor;
    [SerializeField] GameObject[] MaVillagerArmor;
    [SerializeField] GameObject[] MaNakedArmor;
    [SerializeField] GameObject[] MaHair;

    //loaded and saved data
    private bool isFemale;
    private int classCount;
    private bool isSaved;


    private bool isNaked;
    private bool isVillager;
    private bool isKnight;
    private bool isAssassin;
    [SerializeField] GameObject[] ColourUis;

    private string[] classText = { "Naked", "Villager", "Knight", "Assassin" };
    [SerializeField] TextMeshProUGUI BodytypeText;
    [SerializeField] TextMeshProUGUI ClasstypeText;

    public void LoadData(GameData data)
    {
        isFemale = data.isFemale;
        classCount = data.classCount;
        isSaved = data.isSaved;

        CheckIfCreateCharacter();
    }
    public void SaveData(ref GameData data)
    {
        data.isFemale = isFemale;
        data.classCount = classCount;
        data.isSaved = isSaved;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //SwapGender();
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            //NextClass();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            //PreviousClass();
        }
        UpdateUI();
    }

    void CheckIfCreateCharacter()
    {
        if (!isSaved)
        {
            if (isFemale)
            {
                Model[0].SetActive(true);
                Model[1].SetActive(false);
            }
            else
            {
                Model[0].SetActive(false);
                Model[1].SetActive(true);
            }

            UpdateClass();
            UpdateModelMesh();
        }
        else
        {
            UpdateClass();
            UpdateModelMesh();
            CreatePlayerModel();
            transform.GetComponent<ModularArmorsCharacterCreator>().enabled = false;
        }
    }

    public void SwapGender()
    {
        if(isFemale)
        {
            isFemale = false;
            Model[0].SetActive(false);
            Model[1].SetActive(true);
        }
        else
        {
            isFemale = true;
            Model[0].SetActive(true);
            Model[1].SetActive(false);
        }
        UpdateModelMesh();
    }
    public void NextClass()
    {
        if (classCount < 3)
        {
            classCount++;
        }
        else
        {
            classCount = 0;
        }
        UpdateClass();
        UpdateModelMesh();
    }
    public void PreviousClass()
    {
        if (classCount > 0)
        {
            classCount--;
        }
        else
        {
            classCount = 3;
        }
        UpdateClass();
        UpdateModelMesh();
    }
    void UpdateClass()
    {
        if (classCount == 0)
        {
            isNaked = true;
            isVillager = false;
            isKnight = false;
            isAssassin = false;
            ColourUis[0].SetActive(true);
            ColourUis[1].SetActive(false);
            ColourUis[2].SetActive(false);
            ColourUis[3].SetActive(false);
        }
        else if (classCount == 1)
        {
            isNaked = false;
            isVillager = true;
            isKnight = false;
            isAssassin = false;
            ColourUis[0].SetActive(false);
            ColourUis[1].SetActive(true);
            ColourUis[2].SetActive(false);
            ColourUis[3].SetActive(false);
        }
        else if (classCount == 2)
        {
            isNaked = false;
            isVillager = false;
            isKnight = true;
            isAssassin = false;
            ColourUis[0].SetActive(false);
            ColourUis[1].SetActive(false);
            ColourUis[2].SetActive(true);
            ColourUis[3].SetActive(false);
        }
        else if (classCount == 3)
        {
            isNaked = false;
            isVillager = false;
            isKnight = false;
            isAssassin = true;
            ColourUis[0].SetActive(false);
            ColourUis[1].SetActive(false);
            ColourUis[2].SetActive(false);
            ColourUis[3].SetActive(true);
        }
    }
    void UpdateModelMesh()
    {
        if (isFemale)
        {
            foreach (GameObject femaleKnight in FeKnightArmor)
            {
                femaleKnight.SetActive(isKnight);
            }
            foreach (GameObject femaleAssassin in FeAssassinArmor)
            {
                femaleAssassin.SetActive(isAssassin);
            }
            foreach (GameObject femaleVillager in FeVillagerArmor)
            {
                femaleVillager.SetActive(isVillager);
            }
            foreach (GameObject femaleNaked in FeNakedArmor)
            {
                femaleNaked.SetActive(isNaked);
            }
            if (isNaked || isVillager)
            {
                FeHair.SetActive(true);
            }
            else if (!isNaked && !isVillager)
            {
                FeHair.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject maleKnight in MaKnightArmor)
            {
                maleKnight.SetActive(isKnight);
            }
            foreach (GameObject maleAssassin in MaAssassinArmor)
            {
                maleAssassin.SetActive(isAssassin);
            }
            foreach (GameObject maleVillager in MaVillagerArmor)
            {
                maleVillager.SetActive(isVillager);
            }
            foreach (GameObject maleNaked in MaNakedArmor)
            {
                maleNaked.SetActive(isNaked);
            }
            if (isNaked || isVillager)
            {
                foreach (GameObject maleHair in MaHair)
                {
                    maleHair.SetActive(true);
                }
            }
            else if (!isNaked && !isVillager)
            {
                foreach (GameObject maleHair in MaHair)
                {
                    maleHair.SetActive(false);
                }
            }
        }
    }
    void UpdateUI()
    {
        if (isFemale)
        {
            BodytypeText.text = "Body 1";
        }
        else
        {
            BodytypeText.text = "Body 2";
        }
        ClasstypeText.text = classText[classCount];
    }
    void CreatePlayerModel()
    {
        if (isFemale)
        {
            Model[0].SetActive(true);
            Model[1].SetActive(false);
            for (int i = 0; i < Model[0].transform.childCount; i++)
            {
                if (!Model[0].transform.GetChild(i).gameObject.activeSelf)
                {
                    GameObject.Destroy(Model[0].transform.GetChild(i).gameObject);
                }
            }
            GameObject.Destroy(Model[1]);
        }
        else
        {
            Model[0].SetActive(false);
            Model[1].SetActive(true);
            for (int i = 0; i < Model[1].transform.childCount; i++)
            {
                if (!Model[1].transform.GetChild(i).gameObject.activeSelf)
                {
                    GameObject.Destroy(Model[1].transform.GetChild(i).gameObject);
                }
            }
            GameObject.Destroy(Model[0]);
        }
    }
}
