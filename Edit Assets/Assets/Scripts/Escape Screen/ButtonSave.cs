using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSave : MonoBehaviour
{
    private Button button;
    [SerializeField] private AudioClip clip;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener((UnityEngine.Events.UnityAction)delegate
        {
            EventsManager.Instance.playSoundEvent.PlayOneShot(clip, 1);
            //EventsManager.Instance.saveGameEvent.SaveGame();
            DataPersistenceManager.Instance.SaveGame();
        });
    }
}
