using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaSliderChange : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.Instance.hmsEvent.onChangeMana += UpdateValue;
    }
    private void OnDisable()
    {
        EventsManager.Instance.hmsEvent.onChangeMana -= UpdateValue;
    }
    private void UpdateValue(float curMana, float maxMana)
    {
        transform.GetComponent<Slider>().value = curMana / maxMana;
    }
}
