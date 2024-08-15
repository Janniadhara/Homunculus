using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterExitDungeon : MonoBehaviour
{
    [SerializeField] private string SceneNameToTransitionTo;
    
    public void UsePortal()
    {
        if (SceneNameToTransitionTo != null)
        {
            DataPersistenceManager.Instance.TransitionScene();
            SceneManager.LoadSceneAsync(SceneNameToTransitionTo);
        }
    }
}
