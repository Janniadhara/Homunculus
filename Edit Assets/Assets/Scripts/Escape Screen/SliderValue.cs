using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private Text valueDisplay;
    public Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        if (transform.name == "Slider Master")
        {
            SoundManager.Instance.GetMasterVolume(slider);
        }
        if (transform.name == "Slider Music")
        {
            SoundManager.Instance.GetMusicVolume(slider);
        }
        if (transform.name == "Slider Effect")
        {
            SoundManager.Instance.GetEffectVolume(slider);
        }
        valueDisplay.text = slider.value.ToString();
    }
    void Update()
    {
        valueDisplay.text = slider.value.ToString();
    }
}
