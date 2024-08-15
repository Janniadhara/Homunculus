using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour, IDataPersistence
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource MusicSource, MusicSource2, EffectSource;
    [Header("Ambient Music Clips")]
    [SerializeField] private List<AudioClip> DayAmbientMusic;
    [SerializeField] private List<AudioClip> NightAmbientMusic;
    [Header("Tavern")]
    [SerializeField] private AudioClip TavernBackgroundSoundclip;
    [SerializeField] private List<AudioClip> TavernIndoorMusic;
    [Header("Fairy Den")]
    [SerializeField] private AudioClip FairyBackgroundSoundClip;
    [Header("Cementary")]
    [SerializeField] private List<AudioClip> CementaryAmbientMusic;
    [Header("LevelUp Clip")]
    [SerializeField] private AudioClip LevelUpClip;
    private int curLevel;
    private List<AudioClip> AmbientMusic;
    private int outsideTrack;
    private bool isDay;
    private bool daynightSwap;
    private bool inoutSwap;
    private bool outside;
    private bool inTavern;
    private bool inFairy;
    private bool inCementary;

    public float masterVolume;
    public float musicVolume;
    public float effectVolume;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Sound Manager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //EventsManager.Instance.playSoundEvent.onPlayOneShot += PlayEffect;
    }
    private void OnEnable()
    {
        EventsManager.Instance.dayEvent.onDayTime += SetDay;
        EventsManager.Instance.dayEvent.onNightTime += SetNight;
        EventsManager.Instance.levelUpEvent.onLevelUp += LevelUp;
        EventsManager.Instance.pauseGameEvent.onPauseGame += PauseSounds;
    }
    private void OnDisable()
    {
        EventsManager.Instance.dayEvent.onDayTime -= SetDay;
        EventsManager.Instance.dayEvent.onNightTime -= SetNight;
        EventsManager.Instance.levelUpEvent.onLevelUp -= LevelUp;
        EventsManager.Instance.pauseGameEvent.onPauseGame -= PauseSounds;
    }
    public void LoadData(GameData data)
    {
        masterVolume = data.masterVolume;
        musicVolume = data.musicVolume;
        effectVolume = data.effectVolume;
        curLevel = data.level;

        AudioListener.volume = masterVolume;
        MusicSource.volume = musicVolume;
        MusicSource2.volume = musicVolume;
        EffectSource.volume = effectVolume;

        isDay = true;
        daynightSwap = false;
        inoutSwap = false;
        outside = true;
        outsideTrack = 0;
        MusicSource.clip = DayAmbientMusic[Random.Range(0, DayAmbientMusic.Count)];
        MusicSource.Play();
        AmbientMusic = DayAmbientMusic;
    }

    public void SaveData(ref GameData data)
    {
        data.musicVolume = musicVolume;
        data.effectVolume = effectVolume;
        data.masterVolume = masterVolume;
    }
    private void Update()
    {
        if (outside)
        {
            //switch from night to day music
            if (isDay && AmbientMusic == NightAmbientMusic)
            {
                AmbientMusic = DayAmbientMusic;
                daynightSwap = true;
            }
            //switch from day to night music
            else if (!isDay && AmbientMusic == DayAmbientMusic)
            {
                AmbientMusic = NightAmbientMusic;
                daynightSwap = true;
            }
            //make the volume go smoothly to 0 bevore stopping the clip
            if (daynightSwap || inoutSwap)
            {
                MusicSource.volume -= Time.deltaTime * 0.4f;
            }
            if (MusicSource.volume <= 0)
            {
                MusicSource.Stop();
                daynightSwap = false;
                inoutSwap = false;
            }
            //make the background go gradually to 0 bevor stopping
            if (MusicSource2.isPlaying)
            {
                MusicSource2.volume -= Time.deltaTime * 0.4f;
            }
            if (MusicSource2.volume <= 0)
            {
                MusicSource2.Stop();
            }
            //if outside circle through the ambient soundclips
            if (!daynightSwap && !inoutSwap)
            {
                //if volume is 0 gradually go to desired volume
                if (MusicSource.volume <= musicVolume)
                {
                    MusicSource.volume += Time.deltaTime * 0.4f;
                }
                //circle through ambient clips
                if (!MusicSource.isPlaying && outsideTrack < AmbientMusic.Count)
                {
                    MusicSource.clip = AmbientMusic[Random.Range(0, AmbientMusic.Count)];
                    MusicSource.Play();
                    outsideTrack++;
                }
                else if (outsideTrack >= AmbientMusic.Count)
                {
                    outsideTrack = 0;
                }
            }
        }
        else if (inTavern)
        {
            //make the volume go smoothly to 0 bevore stopping the clip
            if (MusicSource.isPlaying && inoutSwap)
            {
                MusicSource.volume -= Time.deltaTime * 0.4f;
            }
            if (MusicSource.volume <= 0)
            {
                MusicSource.Stop();
                MusicSource2.clip = TavernBackgroundSoundclip;
                MusicSource2.volume = 0;
                MusicSource2.Play();
                inoutSwap = false;
            }
            //if volume is 0 gradually go to desired volume
            if (MusicSource.volume <= musicVolume / 3 && !inoutSwap)
            {
                MusicSource.volume += Time.deltaTime * 0.4f;
            }
            if (MusicSource2.volume <= musicVolume && !inoutSwap)
            {
                MusicSource2.volume += Time.deltaTime * 0.4f;
            }
            if (!inoutSwap)
            {
                //circle through ambient clips
                if (!MusicSource.isPlaying)
                {
                    MusicSource.clip = AmbientMusic[Random.Range(0, AmbientMusic.Count)];
                    MusicSource.Play();
                }
            }
        }
        else if (inFairy)
        {
            //make the volume go smoothly to 0 bevore stopping the clip
            if (MusicSource.isPlaying && inoutSwap)
            {
                MusicSource.volume -= Time.deltaTime * 0.4f;
            }
            if (MusicSource.volume <= 0 && inoutSwap)
            {
                MusicSource.Stop();
                MusicSource2.clip = FairyBackgroundSoundClip;
                MusicSource2.volume = 0;
                MusicSource2.Play();
                inoutSwap = false;
            }
            if (MusicSource2.volume <= musicVolume && !inoutSwap)
            {
                MusicSource2.volume += Time.deltaTime * 0.4f;
            }
        }
        else if (inCementary)
        {
            //make the volume go smoothly to 0 bevore stopping the clip
            if (MusicSource.isPlaying && inoutSwap)
            {
                MusicSource.volume -= Time.deltaTime * 0.4f;
            }
            if (MusicSource.volume <= 0)
            {
                MusicSource.Stop();
                inoutSwap = false;
            }
            //if volume is 0 gradually go to desired volume
            if (MusicSource.volume <= musicVolume && !inoutSwap)
            {
                MusicSource.volume += Time.deltaTime * 0.4f;
            }
            if (!inoutSwap)
            {
                //circle through ambient clips
                if (!MusicSource.isPlaying)
                {
                    MusicSource.clip = AmbientMusic[Random.Range(0, AmbientMusic.Count)];
                    MusicSource.Play();
                }
            }
        }
    }

    public void EnterTavern()
    {
        if (outside)
        {
            AmbientMusic = TavernIndoorMusic;
            inTavern = true;
        }
        else
        {
            inTavern = false;
            if (isDay)
            {
                AmbientMusic = DayAmbientMusic;
            }
            else
            {
                AmbientMusic = NightAmbientMusic;
            }
        }
        outside = !outside;
        inoutSwap = true;
    }
    public void EnterFairy()
    {
        if (outside)
        {
            inFairy = true;
        }
        else
        {
            inFairy = false;
            if (isDay)
            {
                AmbientMusic = DayAmbientMusic;
            }
            else
            {
                AmbientMusic = NightAmbientMusic;
            }
        }
        outside = !outside;
        inoutSwap = true;
    }
    public void EnterCementary()
    {
        if (outside)
        {
            AmbientMusic = CementaryAmbientMusic;
            inCementary = true;
        }
        else
        {
            inCementary = false;
            if (isDay)
            {
                AmbientMusic = DayAmbientMusic;
            }
            else
            {
                AmbientMusic = NightAmbientMusic;
            }
        }
        outside = !outside;
        inoutSwap = true;
    }
    public void PlayMusic(AudioClip music)
    {
        MusicSource.clip = music;
        MusicSource.Play();
    }
    public void PlayEffect(AudioClip clip, float volume)
    {
        EffectSource.PlayOneShot(clip, effectVolume);
    }
    #region get/set volume
    public void GetMasterVolume(Slider slider)
    {
        slider.value = masterVolume * 100;
    }
    public void GetMusicVolume(Slider slider)
    {
        slider.value = musicVolume * 100;
    }
    public void GetEffectVolume(Slider slider)
    {
        slider.value = effectVolume * 100;
    }
    public void ChangeMasterVolume(Slider slider)
    {
        masterVolume = slider.value / 100;
        AudioListener.volume = masterVolume;
    }
    public void ChangeMusicVolume(Slider slider)
    {
        musicVolume = slider.value / 100;
        MusicSource.volume = musicVolume;
        MusicSource2.volume = musicVolume;
    }
    public void ChangeEffectVolume(Slider slider)
    {
        effectVolume = slider.value / 100;
        EffectSource.volume = effectVolume;
    }
    #endregion

    private void SetDay()
    {
        isDay = true;
    }
    private void SetNight()
    {
        isDay = false;
    }
    private void LevelUp(int level)
    {
        if (curLevel != level)
        {
            EffectSource.PlayOneShot(LevelUpClip);
            curLevel = level;
        }
    }
    private void PauseSounds(bool ispaused)
    {
        if (ispaused)
        {
            MusicSource.Pause();
            MusicSource2.Pause();
            EffectSource.Pause();
        }
        else
        {
            MusicSource.UnPause();
            MusicSource2.UnPause();
            EffectSource.UnPause();
        }
    }
}
