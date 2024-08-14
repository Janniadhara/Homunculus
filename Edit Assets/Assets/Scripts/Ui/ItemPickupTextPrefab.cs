using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupTextPrefab : MonoBehaviour
{
    [SerializeField] private float secondsToLive;
    private float secondsAlive;
    private Text ItemText;
    private Color textColor;
    // Start is called before the first frame update
    void Start()
    {
        ItemText = GetComponent<Text>();
        textColor = ItemText.color;
    }

    // Update is called once per frame
    void Update()
    {
        secondsAlive += Time.deltaTime;
        if (secondsAlive >= secondsToLive)
        {
            Destroy(gameObject);
        }
        textColor.a = 1 - (secondsAlive / secondsToLive);
        ItemText.color = textColor;
    }
}
