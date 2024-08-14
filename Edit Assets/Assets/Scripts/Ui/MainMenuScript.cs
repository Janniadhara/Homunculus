using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button quitGameButton;
    private void Start()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueGameButton.interactable = false;
        }
    }
    public void CreateNewGame()
    {
        DisableButtons();
        DataPersistenceManager.Instance.NewGame();
        SceneManager.LoadSceneAsync("SampleScene");
    }
    public void ContinueGame()
    {
        DisableButtons();
        //DataPersistenceManager.Instance.LoadGame();
        // -> don't need that cause loading a scene loads the game data
        SceneManager.LoadSceneAsync("SampleScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void DisableButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
