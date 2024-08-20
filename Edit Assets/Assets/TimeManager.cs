using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    [Header("Config")]
    [SerializeField] private float startHour;
    [SerializeField] private float timeMultiplier;
    [SerializeField] AnimationCurve lightChangeCurve;
    [SerializeField] private Text timeText;

    [Header("Sun and Daytime")]
    [SerializeField] private Light Sun;
    [SerializeField] private float sunRiseHour;
    private TimeSpan sunriseTime;
    [SerializeField] Color dayColor;
    [SerializeField] private float maxSunIntensity;
    public bool sayIsDay;
    private DateTime curTime;

    [Header("Moon and Nighttime")]
    [SerializeField] private Light Moon;
    [SerializeField] private float sunSetHour;
    private TimeSpan sunsetTime;
    [SerializeField] Color nightColor;
    [SerializeField] private float maxMoonIntensity;
    public bool sayIsNight;

    [Header("Pausing Game")]
    [SerializeField] private GameObject PauseScreen;

    public bool inCementary;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Time Manager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        inCementary = false;
    }
    void Start()
    {
        curTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunRiseHour);
        sunsetTime = TimeSpan.FromHours(sunSetHour);

        PauseScreen.SetActive(false);
        if (curTime.Hour > sunRiseHour && curTime.Hour < sunSetHour)
        {
            sayIsDay = false;
            sayIsNight = true;
        }
        else
        {
            sayIsDay = true;
            sayIsNight = false;
        }
    }

    void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
    }
    public void PauseGame()
    {
        if (!PauseScreen.activeSelf)
        {
            PauseScreen.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            EventsManager.Instance.pauseGameEvent.PauseGame(true);
        }
        else
        {
            PauseScreen.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            EventsManager.Instance.pauseGameEvent.PauseGame(false);
        }
    }
    private void UpdateTimeOfDay()
    {
        curTime = curTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null )
        {
            timeText.text = curTime.ToString("HH:mm");
        }
        if (curTime.Hour == sunRiseHour && sayIsDay)
        {
            sayIsDay = false;
            sayIsNight = true;
            EventsManager.Instance.dayEvent.Sunrise();
        }
        else if (curTime.Hour == sunSetHour && sayIsNight)
        {
            sayIsNight = false;
            sayIsDay = true;
            EventsManager.Instance.dayEvent.Sunset();
        }
    }
    private void RotateSun()
    {
        float sunRotation;
        float moonRotation;
        if (curTime.TimeOfDay > sunriseTime && curTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, curTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            sunRotation = Mathf.Lerp(0, 180, (float)percentage);
            moonRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        else
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, curTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            sunRotation = Mathf.Lerp(180, 360, (float)percentage);
            moonRotation = Mathf.Lerp(0, 180, (float)percentage);
        }

        Sun.transform.rotation = Quaternion.AngleAxis(sunRotation, Vector3.right);
        Moon.transform.rotation = Quaternion.AngleAxis(moonRotation, Vector3.right);
    }
    private void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(Sun.transform.forward, Vector3.down);
        Sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightChangeCurve.Evaluate(dotProduct));
        Moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor, lightChangeCurve.Evaluate(dotProduct));
    }
    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromTime;
        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }
        return difference;
    }
    public void EnterExtiCementary()
    {
        inCementary = !inCementary;
        if (inCementary)
        {
            maxSunIntensity = 0.2f;
        }
        else
        {
            maxSunIntensity = 1;
        }
    }
}
