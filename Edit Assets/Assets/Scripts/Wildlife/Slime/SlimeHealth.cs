using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeHealth : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private int baseHealth;
    [SerializeField] private float healthRegen;
    private float maxHealth;
    private float curHealth;
    private float actualHealthRegen;
    [SerializeField] private Text slimeLevel;
    public int SlimeLevel { private get; set; }
    private bool valuesSet = false;

    [Header("Kill Rewards")]
    [SerializeField] private int baseXpReward;
    private int actualXpreward;
    [SerializeField] private ItemObject[] droppedItems;
    [SerializeField] private ParticleSystem droppedParticles;
    [Range(0, 10)]
    [SerializeField] private int maxDropAmount;
    public void SetValues()
    {
        maxHealth = baseHealth * SlimeLevel;
        actualXpreward = baseXpReward * SlimeLevel;
        actualHealthRegen = healthRegen * SlimeLevel;
        curHealth = maxHealth;
        healthSlider.value = curHealth;
        slimeLevel.text = "Lv. " + SlimeLevel.ToString();
        valuesSet = true;
    }

    void Update()
    {
        if (valuesSet)
        {
            if (curHealth < maxHealth)
            {
                healthSlider.gameObject.SetActive(true);
                RegenerateHealth();
            }
            else if (curHealth >= maxHealth)
            {
                curHealth = maxHealth;
                healthSlider.gameObject.SetActive(false);
            }
            healthSlider.value = curHealth / maxHealth;

            if (curHealth <= 0)
            {
                RewardKiller();
                //drop items at position + 0.5y
                //destroy Gameobject
            }
        }
    }
    private void RegenerateHealth()
    {
        curHealth += actualHealthRegen * Time.deltaTime;
    }
    private void RewardKiller()
    {
        GameObject ItemDrop = Instantiate(droppedItems[0].Prefab, transform.position + Vector3.up, transform.rotation); ;
        ParticleSystem DropParticles = droppedParticles;
        ItemDrop.GetComponent<AttachedItem>().amount = Random.Range(1, maxDropAmount + 1);
        
        Instantiate(DropParticles, ItemDrop.transform, false);

        EventsManager.Instance.levelUpEvent.GainXp(actualXpreward);

        Destroy(gameObject);
    }
}
