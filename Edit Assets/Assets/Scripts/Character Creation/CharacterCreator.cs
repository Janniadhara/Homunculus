using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private GameObject PlayerModel;

    public Transform PlayerHead;
    public Transform PlayerTorso;
    public Transform PlayerLegs;

    [SerializeField] private GameObject[] PlayerModelPrefabs;
    private GameObject PlayerModelPrefab;

    [SerializeField] private Material[] HairEyesColour;
    private string[] HairColour = { "Brown", "Blue", "Green", "Turquoise", "Pink", "Purple" };
    private string[] EyesColour = { "Brown", "Blue", "Green", "Turquoise", "Pink", "Purple" , "Black", "White"};
    [SerializeField] private Material[] BlousePantsColours;
    private string[] BlousePantsColour = { "Red", "Brown", "Grey", "Green", "Blue", "Purple"};
    [SerializeField] private Material[] SkirtShirtColours;
    private string[] SkirtShirtColour = { "White", "Brown", "Grey", "Green", "Blue", "Purple" };
    [SerializeField] private Material[] BhColours;
    private string[] BhColour = { "Orange", "Brown", "Grey", "Green", "Blue", "Purple" };

    //needs to be loaded and saved
    private int modelCount = 0;
    private int hairColourCount = 0;
    private int eyesColourCount = 0;
    private int torsoColourCount = 0;
    private int legColourCount = 0;

    private bool pants = false;
    private bool shirt = false;
    private bool bh = false;

    [SerializeField] private TextMeshProUGUI ModelText;
    [SerializeField] private TextMeshProUGUI HairColourText;
    [SerializeField] private TextMeshProUGUI EyesColourText;
    [SerializeField] private TextMeshProUGUI TorsoColourText;
    [SerializeField] private TextMeshProUGUI LegColourText;

    public int test;

    void Start()
    {
        RemoveModel();

        modelCount = 0;
        hairColourCount = 0;
        eyesColourCount = 0;
        torsoColourCount = 0;
        legColourCount = 0;

        PlayerModelPrefab = Instantiate(PlayerModelPrefabs[modelCount]);
        PlayerModelPrefab.transform.SetParent(PlayerModel.transform, false);
        ModelUpdated();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        test = PlayerModelPrefabs.Length;
    }

    private void RemoveModel()
    {
        int childCount = PlayerModel.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(PlayerModel.transform.GetChild(i).gameObject);
        }
    }
    public void ModelUpdated()
    {
        PlayerHead = PlayerModelPrefab.transform.GetChild(0);
        PlayerTorso = PlayerModelPrefab.transform.GetChild(2);
        PlayerLegs = PlayerModelPrefab.transform.GetChild(1);

        CheckClothing();
        UpdateColour();
        ModelText.text = "Model " + (modelCount+1).ToString();
    }
    private void CheckClothing()
    {
        //Female model with pants
        if (modelCount == 1)
        {
            pants = true;
            shirt = false;
            bh = false;
        }
        //Male model
        else if (modelCount == 2)
        {
            pants = true;
            shirt = true;
            bh = false;
        }
        //Female bikini model
        else if (modelCount == 3)
        {
            pants = false;
            shirt = false;
            bh = true;
        }
        else
        {
            pants = false;
            shirt = false;
            bh = false;
        }
    }
    private void UpdateColour()
    {
        //Head
        PlayerHead.GetComponent<SkinnedMeshRenderer>().material = HairEyesColour[hairColourCount + (eyesColourCount * 6)];
        HairColourText.text = HairColour[hairColourCount];
        EyesColourText.text = EyesColour[eyesColourCount];

        //Torso
        if (shirt)
        {
            PlayerTorso.GetComponent<SkinnedMeshRenderer>().material = SkirtShirtColours[torsoColourCount];
            TorsoColourText.text = SkirtShirtColour[torsoColourCount];
        }
        else if (!shirt)
        {
            PlayerTorso.GetComponent<SkinnedMeshRenderer>().material = BlousePantsColours[torsoColourCount];
            TorsoColourText.text = BlousePantsColour[torsoColourCount];
        }
        if (bh)
        {
            PlayerTorso.GetComponent<SkinnedMeshRenderer>().material = BhColours[torsoColourCount];
            TorsoColourText.text = BhColour[torsoColourCount];
        }
        //Legs
        if (pants)
        {
            PlayerLegs.GetComponent<SkinnedMeshRenderer>().material = BlousePantsColours[legColourCount];
            LegColourText.text = BlousePantsColour[legColourCount];
        }
        else if (!pants)
        {
            PlayerLegs.GetComponent<SkinnedMeshRenderer>().material = SkirtShirtColours[legColourCount];
            LegColourText.text = SkirtShirtColour[legColourCount];
        }
    }

    #region Change Model
    public void ModelUp()
    {
        if (modelCount < PlayerModelPrefabs.Length - 1)
        {
            modelCount++;
        }
        RemoveModel();

        //load and paste in Playermodel
        PlayerModelPrefab = Instantiate(PlayerModelPrefabs[modelCount]);
        PlayerModelPrefab.transform.SetParent(PlayerModel.transform, false);

        //get the three diferent Bodyparts
        ModelUpdated();
    }
    public void ModelDown()
    {
        if (modelCount > 0)
        {
            modelCount--;
        }
        RemoveModel();

        //load and paste in Playermodel
        PlayerModelPrefab = Instantiate(PlayerModelPrefabs[modelCount]);
        PlayerModelPrefab.transform.SetParent(PlayerModel.transform, false);

        //get the three diferent Bodyparts
        ModelUpdated();
        UpdateColour();
    }
    #endregion

    #region Haircolour
    public void HairColourUp()
    {
        if (hairColourCount < HairColour.Length - 1)
        {
            hairColourCount++;
        }
        UpdateColour();
    }
    public void HairColourDown()
    {
        if (hairColourCount > 0)
        {
            hairColourCount--;
        }
        UpdateColour();
    }
    #endregion

    #region Eyecolour
    public void EyesColourUp()
    {
        if (eyesColourCount < EyesColour.Length - 1)
        {
            eyesColourCount++;
        }
        UpdateColour();
    }
    public void EyesColourDown()
    {
        if (eyesColourCount > 0)
        {
            eyesColourCount--;
        }
        UpdateColour();
    }
    #endregion

    #region Torsocolour
    public void TorsoColourUp()
    {
        if (torsoColourCount < 5)
        {
            torsoColourCount++;
        }
        UpdateColour();
    }
    public void TorsoColourDown()
    {
        if (torsoColourCount > 0)
        {
            torsoColourCount--;
        }
        UpdateColour();
    }
    #endregion

    #region Legcolour
    public void LegsColourUp()
    {
        if(legColourCount < 5)
        {
            legColourCount++;
        }
        UpdateColour();
    }
    public void LegsColourDown()
    {
        if (legColourCount > 0)
        {
            legColourCount--;
        }
        UpdateColour();
    }
    #endregion
}
