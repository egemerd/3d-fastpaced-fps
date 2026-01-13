using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopEffect : MonoBehaviour
{
    public static TimeStopEffect Instance;
    private float initialTimeScale;

    [SerializeField] private List<TimeStopData> timeStopDatas = new List<TimeStopData>();
    private Dictionary<string, TimeStopData> timeStopDataDict = new Dictionary<string, TimeStopData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePresets();
    }
    private void Start()
    {
        initialTimeScale = Time.timeScale;
    }

    private void InitializePresets()
    {
        timeStopDataDict.Clear();

        foreach (var data in timeStopDatas)
        {
            // Preset name boþ veya null kontrolü
            if (string.IsNullOrEmpty(data.presetName))
            {
                Debug.LogWarning("[TimeStopEffect] Found preset with empty name, skipping...");
                continue;
            }

            // Duplicate kontrol
            if (timeStopDataDict.ContainsKey(data.presetName))
            {
                Debug.LogWarning($"[TimeStopEffect] Duplicate preset name '{data.presetName}' found! Using first occurrence.");
                continue;
            }

            // Dictionary'ye ekle
            timeStopDataDict.Add(data.presetName, data);
        }

        Debug.Log($"[TimeStopEffect] Initialized {timeStopDataDict.Count} time stop presets");

    }
    public void TimeStop(float delay ,float duration, float timeScale)
    {
        StartCoroutine(TimeStopCoroutine(delay ,duration, timeScale));
    }

    public void TimeStopPreset(string presetName)
    {
        if(timeStopDataDict.TryGetValue(presetName, out TimeStopData data))
        {
            TimeStop(data.delay , data.timeStopDuration, data.timeScaleDuringStop);
        }
        else
        {
            Debug.LogWarning($"[TimeStopEffect] Preset '{presetName}' not found!");
        }   
    }
    

    private IEnumerator TimeStopCoroutine(float delay , float duration, float timeScale)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = initialTimeScale;
    }
}
