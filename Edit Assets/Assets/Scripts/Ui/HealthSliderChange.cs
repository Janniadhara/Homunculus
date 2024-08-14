using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSliderChange : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.Instance.hmsEvent.onChangeHealth += UpdateValue;
    }
    private void OnDisable()
    {
        EventsManager.Instance.hmsEvent.onChangeHealth -= UpdateValue;
    }
    private void UpdateValue(float curHealth, float maxHealth)
    {
        transform.GetComponent<Slider>().value = curHealth / maxHealth;
    }
}
