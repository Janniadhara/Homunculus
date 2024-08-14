using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AggressiveMobStats : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [Header("Base Stats")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Text wildliveLevelText;
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseDamage;
    private float maxHealth;
    public float actualDamage;
    [SerializeField] private float healthRegen;
    private float actualHealthRegen;
    private float curHealth;
    public int WildliveLevel { private get; set; }
    private bool valuesSet = false;

    [Header("Kill Rewards")]
    [SerializeField] private string animalName;
    [SerializeField] private int baseXpReward;
    private int actualXpreward;
    [Range(1, 10)]
    [SerializeField] private int maxDropAmount;
    [SerializeField] private ItemObject[] droppedItems;
    [SerializeField] private ParticleSystem droppedParticles;

    public void SetValues()
    {
        maxHealth = baseHealth * WildliveLevel;
        actualXpreward = baseXpReward * WildliveLevel;
        actualHealthRegen = healthRegen * WildliveLevel;
        curHealth = maxHealth;
        healthSlider.value = curHealth;
        wildliveLevelText.text = "Lv. " + WildliveLevel.ToString();
        actualDamage = baseDamage * ((WildliveLevel + 1) * 0.5f);
        attackHitbox.GetComponent<AggressiveMobAttackEnemy>().damage = actualDamage;
        attackHitbox.SetActive(false);

        valuesSet = true;
    }
    private void OnEnable()
    {
        EventsManager.Instance.damageEvents.onDamageAnimal += GetDamage;
    }
    private void OnDisable()
    {
        EventsManager.Instance.damageEvents.onDamageAnimal -= GetDamage;
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
        else
        {
            SetValues();
        }
    }
    private void RegenerateHealth()
    {
        curHealth += actualHealthRegen * Time.deltaTime;
    }
    private void RewardKiller()
    {
        int dropAmount = Random.Range(1, maxDropAmount);
        for (int i = 0; i < dropAmount; i++)
        {
            GameObject ItemDrop = Instantiate(droppedItems[0].Prefab, transform.position + Vector3.up, transform.rotation);
            ParticleSystem DropParticles = droppedParticles;
            ItemDrop.GetComponent<AttachedItem>().amount = 1;

            Instantiate(DropParticles, ItemDrop.transform, false);
        }
        EventsManager.Instance.levelUpEvent.GainXp(actualXpreward);
        EventsManager.Instance.killEvent.KillAnimal(animalName);

        Destroy(gameObject);
    }
    private void GetDamage(GameObject animal, int damage)
    {
        if (animal == gameObject)
        {
            curHealth -= damage;
        }
    }
    public void StartAttack()
    {
        attackHitbox.SetActive(true);
    }
    public void StopAttack()
    {
        attackHitbox.SetActive(false);
    }
}
