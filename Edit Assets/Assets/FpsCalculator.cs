using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCalculator : MonoBehaviour
{
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;
    private int fps;
    [SerializeField] private TextMeshProUGUI FpsText;

    private bool isPaused;

    void Awake()
    {
        frameDeltaTimeArray = new float[50];
        EventsManager.Instance.pauseGameEvent.onPauseGame += PauseGame;
    }

    void Update()
    {
        if (!isPaused)
        {
            frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
            lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;
            FpsText.text = Mathf.RoundToInt(CalculateFps()).ToString();
        }
    }

    private float CalculateFps()
    {
        float total = 0f;
        foreach (float deltaTime in frameDeltaTimeArray)
        {
            total += deltaTime;
        }
        return frameDeltaTimeArray.Length / total;
    }
    private void PauseGame(bool isPaused)
    {
        this.isPaused = isPaused;
    }
}
