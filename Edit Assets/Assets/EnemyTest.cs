using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTest : MonoBehaviour
{
    public Slider HealthBar;
    public float maxhealth;
    public float curhealth;

    public float tilNextHit = 1.6f;
    public bool canBeHit;
    // Start is called before the first frame update
    void Start()
    {
        maxhealth = 10000;
        curhealth = maxhealth;
        HealthBar.value = curhealth / maxhealth;

        canBeHit = true;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.value = curhealth / maxhealth;
        if (!canBeHit)
        {
            tilNextHit -= Time.deltaTime;
        }
        if (tilNextHit < 0 )
        {
            canBeHit = true;
            tilNextHit = 1.6f;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item" && canBeHit)
        {
            //curhealth -= other.GetComponent<ItemData>().item.dmgValue;
            canBeHit = false;
        }
    }
}
