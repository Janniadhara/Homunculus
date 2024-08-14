using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HmsCalculator : MonoBehaviour, IDataPersistence
{
    [SerializeField] private bool godMode;

    private MoveAndLook MoveAndLookScript;
    private CharacterController CharacterController;
    public EquipmentObject PlayerEquipment;

    [Header("Exp")]
    [SerializeField] private Slider ExpSlider;
    [SerializeField] private Text LevelText;
    public Leveling leveling;
    [Header("Money")]
    public Money money;
    [SerializeField] private Text BronzeText;
    [SerializeField] private Text SilverText;
    [SerializeField] private Text GoldText;
    [Header("Health")]
    [SerializeField] private Slider HealthSlider;
    public float maxHealth;
    public float curHealth;
    [Header("Mana")]
    [SerializeField] private Slider ManaSlider;
    private float maxMana;
    private float curMana;
    [Header("Stamina")]
    [SerializeField] private Slider StaminaSlider;
    private float maxStamina;
    private float curStamina;

    public bool isAlive;
    public bool canRun;
    public bool canJump;
    public float fallTime;

    private float totalHealthBoost;

    private void OnEnable()
    {
        EventsManager.Instance.levelUpEvent.onGainXp += leveling.GainedExp;
        EventsManager.Instance.levelUpEvent.onGainXp += UpdateLevelDisplay;
        EventsManager.Instance.levelUpEvent.onLevelUp += UpdateLevelDisplay;
        //EventsManager.Instance.levelUpEvent.LevelUp(leveling.curLevel);

        EventsManager.Instance.moneyGainedEvent.onGainMoney += money.GainedCoins;
        EventsManager.Instance.moneyGainedEvent.onGainMoney += UpdateMoneyDisplay;

        EventsManager.Instance.damageEvents.onDamagePlayer += GetDamage;
    }
    private void OnDisable()
    {
        EventsManager.Instance.levelUpEvent.onGainXp -= leveling.GainedExp;
        EventsManager.Instance.levelUpEvent.onGainXp -= UpdateLevelDisplay;
        EventsManager.Instance.levelUpEvent.onLevelUp -= UpdateLevelDisplay;
        //EventsManager.Instance.levelUpEvent.LevelUp(leveling.curLevel);

        EventsManager.Instance.moneyGainedEvent.onGainMoney -= money.GainedCoins;
        EventsManager.Instance.moneyGainedEvent.onGainMoney -= UpdateMoneyDisplay;

        EventsManager.Instance.damageEvents.onDamagePlayer -= GetDamage;
    }
    public void LoadData(GameData data)
    {
        Debug.Log("Loading HmsCalculator.");

        //TODO load maxhealth, current health, maxstamina, maxmana

        //maxMana = data.maxMana;
        //maxStamina = data.maxStamina;
        //curMana = maxMana;
        //curStamina = maxStamina;
        SetMaxHealth();

        leveling.curLevel = data.level;
        leveling.curExp = data.exp;
        leveling.CalculateExp();
        money.curBronzeCoins = data.bronzeCoins;
        money.curSilverCoins = data.silverCoins;
        money.curGoldCoins = data.goldCoins;
        UpdateMoneyDisplay(money.curBronzeCoins, money.curSilverCoins, money.curGoldCoins);
        UpdateLevelDisplay(leveling.curLevel);
        Debug.Log("Loading HmsCalculator completed.");
    }
    public void SaveData(ref GameData data)
    {
        //TODO save maxhealth, current health, maxstamina, maxmana

        data.level = leveling.curLevel;
        data.exp = leveling.curExp;
        data.bronzeCoins = money.curBronzeCoins;
        data.silverCoins = money.curSilverCoins;
        data.goldCoins = money.curGoldCoins;
    }
    void Start()
    {
        MoveAndLookScript = GetComponent<MoveAndLook>();
        CharacterController = GetComponent<CharacterController>();

        maxHealth = 100;
        curHealth = maxHealth;
        maxMana = 100;
        curMana = maxMana;
        maxStamina = 100;
        curStamina = maxStamina;

        isAlive = true;

        EventsManager.Instance.hmsEvent.ChangeStamina(curStamina, maxStamina);

        CalculateHealth(0);
        UpdateLevelDisplay(leveling.curLevel);
    }

    void Update()
    {
        //testing leveling
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventsManager.Instance.levelUpEvent.GainXp(4);
        }

        if (MoveAndLookScript.isFalling)
        {
            fallTime += Time.deltaTime;
        }
        else if (MoveAndLookScript.isSwimming)
        {
            fallTime = 0;
        }
        if (MoveAndLookScript.isGrounded && fallTime >= 1.5)
        {
            CalculateHealth(fallTime * 20);
            fallTime = 0;
        }
        if (MoveAndLookScript.isGrounded && fallTime < 1.5)
        {
            fallTime = 0;
        }
        CalculateStamina();
        //ManaSlider.value = curMana / maxMana;

        //slow health regen
        if (curHealth < maxHealth && isAlive)
        {
            curHealth += 0.5f * Time.deltaTime;
            EventsManager.Instance.hmsEvent.ChangeHealth(curHealth, maxHealth);
        }

    }
    private void UpdateMoneyDisplay(int b, int s, int g)
    {
        BronzeText.text = money.curBronzeCoins.ToString();
        SilverText.text = money.curSilverCoins.ToString();
        GoldText.text = money.curGoldCoins.ToString();
    }
    private void UpdateLevelDisplay(int level)
    {
        LevelText.text = "Lvl. " + leveling.curLevel.ToString();
        ExpSlider.value = (float)leveling.curExp / leveling.neededExp;
        EventsManager.Instance.levelUpEvent.ChangeExp(leveling.curExp, leveling.neededExp);
    }

    public void SetMaxHealth()
    {
        float ringHealthBoost = PlayerEquipment.Ring.healthBoost;
        totalHealthBoost = ringHealthBoost;
        maxHealth = 100 * totalHealthBoost;
    }
    public void CalculateHealth(float damage)
    {
        if (curHealth > 0)
        {
            curHealth -= damage;
        }
        if (curHealth <= 0)
        {
            curHealth = 0;
            //isAlive = false;
        }
        HealthSlider.value = curHealth / maxHealth;
        EventsManager.Instance.hmsEvent.ChangeHealth(curHealth, maxHealth);
    }
    void CalculateStamina()
    {
        if (!godMode)
        {
            //lose stamina if running or sprinting (more when ovesprinting)
            if (MoveAndLookScript.maxSpeed > 2 && curStamina > 0 && MoveAndLookScript.isGrounded)
            {
                curStamina -= MoveAndLookScript.maxSpeed * Time.deltaTime;
            }
            //regain stamina if waltking or standing 10 per sec
            else if (MoveAndLookScript.maxSpeed <= 2 && curStamina <= maxStamina && MoveAndLookScript.isGrounded)
            {
                curStamina += 10 * Time.deltaTime;
            }
            //can't run if out of stamina
            if (curStamina <= 0)
            {
                canRun = false;
                canJump = false;
            }
            //needs to recharge a bit stamina bevore running again
            else if (curStamina > 10)
            {
                canRun = true;
                canJump = true;
            }
            else if (curStamina <= 10)
            {
                canJump = false;
            }
            if (canJump && MoveAndLookScript.isGrounded && MoveAndLookScript.isJumping)
            {
                curStamina -= 10;
            }
            StaminaSlider.value = curStamina / maxStamina;
            EventsManager.Instance.hmsEvent.ChangeStamina(curStamina, maxStamina);
        }
    }
    private void GetDamage(float damage)
    {
        CalculateHealth(damage);
    }
}
