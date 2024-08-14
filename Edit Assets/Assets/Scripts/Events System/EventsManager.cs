using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance { get; private set; }

    public DayTimeEvent dayEvent;
    public LevelUpEvent levelUpEvent;
    public SaveGameEvent saveGameEvent;
    public PickUpItemEvent pickUpItemEvent;
    public QuestEvents questEvents;
    public MoneyGainedEvent moneyGainedEvent;
    public PlaySoundEvent playSoundEvent;
    public DialogueEvent dialogueEvent;
    public PauseGameEvent pauseGameEvent;
    public HmsEvent hmsEvent;
    public DetectPlayerEvent detectPlayerEvent;
    public PlayerStateEvent playerStateEvent;
    public DamageEvents damageEvents;
    public KillEvent killEvent;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Events Manager.");
            //Destroy(gameObject);
        }
        Instance = this;

        dayEvent = new DayTimeEvent();
        levelUpEvent = new LevelUpEvent();
        saveGameEvent = new SaveGameEvent();
        pickUpItemEvent = new PickUpItemEvent();
        questEvents = new QuestEvents();
        moneyGainedEvent = new MoneyGainedEvent();
        playSoundEvent = new PlaySoundEvent();
        dialogueEvent = new DialogueEvent();
        pauseGameEvent = new PauseGameEvent();
        hmsEvent = new HmsEvent();
        detectPlayerEvent = new DetectPlayerEvent();
        playerStateEvent = new PlayerStateEvent();
        damageEvents = new DamageEvents();
        killEvent = new KillEvent();
    }
}
