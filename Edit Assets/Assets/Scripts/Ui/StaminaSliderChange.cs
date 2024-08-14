using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSliderChange : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.Instance.hmsEvent.onChangeStamina += UpdateValue;
    }
    private void OnDisable()
    {
        EventsManager.Instance.hmsEvent.onChangeStamina -= UpdateValue;
    }
    private void UpdateValue(float curStamina, float maxStamina)
    {
        transform.GetComponent<Slider>().value = curStamina / maxStamina;
    }
}
