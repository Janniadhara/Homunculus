using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpSliderChange : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.Instance.levelUpEvent.onChangeExp += UpdateValue;
    }
    private void OnDisable()
    {
        EventsManager.Instance.levelUpEvent.onChangeExp -= UpdateValue;
    }
    private void UpdateValue(int curExp, int reqExp)
    {
        transform.GetComponent<Slider>().value = (float)curExp / reqExp;
    }
}
