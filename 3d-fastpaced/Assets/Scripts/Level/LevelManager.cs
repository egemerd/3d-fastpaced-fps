using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int currentLevelIndex;

    private GameObject levelEndCanvas;
    private RectTransform canvasRectTransform;
    [SerializeField] private TextMeshProUGUI levelEndTimeText;
    [SerializeField] private TextMeshProUGUI levelEndkilledEnemiesText;
    [SerializeField] private TextMeshProUGUI levelEndRankText;

    [SerializeField] private List<LevelDataSO> levelDatas;
    

    [SerializeField] Material wallMaterial;
    [SerializeField] Material floorMaterial;
    
    [SerializeField] Volume globalVolume;

    [Header("Level End Animation")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutBack; // Açýlýþ animasyonu
    [SerializeField] private Ease closeEase = Ease.InBack;

    [Header("Game Over Settings")]
    [SerializeField] private float exposureFadeDuration = 1.5f;
    [SerializeField] private float targetExposure = 8f; // Beyaz için pozitif deðer
    [SerializeField] private AnimationCurve exposureCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private GameObject gameOverCanvas;

    private ColorAdjustments colorAdjustments;
    private float originalExposure = 0f;

    LevelStats levelStats;
    private void Awake()
    {
        Instance = this;
        levelStats = FindObjectOfType<LevelStats>();

        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            originalExposure = colorAdjustments.postExposure.value;
        }
        else
        {
            Debug.LogWarning("[LevelManager] ColorAdjustments not found in Volume profile!");
        }

    }

    private void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        FindLevelEndCanvas();
        
        wallMaterial.color = levelDatas[currentLevelIndex].wallColor;
        floorMaterial.color = levelDatas[currentLevelIndex].floorColor;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(GameOverCoroutine());
        }
    }

    

    public float GetSliderSpeed()
    {
        return levelDatas[currentLevelIndex].sliderSpeed;   
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelCoroutine());
    }
    private IEnumerator LoadNextLevelCoroutine()
    {
        EnableAnimation(levelEndCanvas);
        EnableAnimation(levelEndkilledEnemiesText.gameObject);
        EnableAnimation(levelEndTimeText.gameObject);

        yield return new WaitForSecondsRealtime(2f);
        levelEndCanvas.SetActive(false);
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentLevelIndex + 1);
    }

    private void EnableAnimation(GameObject obj)
    {
        obj.transform.DOScale(Vector3.zero, animationDuration)
            .SetEase(closeEase) // InBack - içeri çekilip küçülür
            .SetUpdate(true);
    }
    
    public void ActivateLevelEndCanvas()
    {
        if (levelEndCanvas == null) return;

        levelEndCanvas.transform.parent.gameObject.SetActive(true);
        levelEndCanvas.SetActive(true);
        
        levelEndTimeText.text = levelStats.gameTime.ToString();
        levelEndkilledEnemiesText.text = levelStats.killedEnemies.ToString();
        levelEndRankText.text = levelStats.GetCurrentRankString();
        levelEndRankText.color = levelStats.GetCurrentRankColor();

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //  Scale animasyonu (0'dan 1'e)
        if (canvasRectTransform != null)
        {
            canvasRectTransform.localScale = Vector3.zero;
            canvasRectTransform.DOScale(Vector3.one, animationDuration)
                .SetEase(openEase) // OutBack - pop efekti
                .SetUpdate(true);
        }
    }

    private void FindLevelEndCanvas()
    {
        LevelEndCanvasMarker marker = FindObjectOfType<LevelEndCanvasMarker>(true); // true = inactive objeleri de ara

        if (marker != null)
        {
            levelEndCanvas = marker.gameObject;

            canvasRectTransform = levelEndCanvas.GetComponent<RectTransform>();

            levelEndCanvas.SetActive(false);
            Debug.Log($"[LevelManager] LevelEndCanvas found: {levelEndCanvas.name}");
        }
        else
        {
            Debug.LogError("[LevelManager] LevelEndCanvas with LevelEndCanvasMarker not found!");
        }
    }

    public void GameOverCanvas()
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        AudioManager.Instance.PlaySFX("PlayerDeath" ,0.8f);
        Time.timeScale = 0f;
        float elapsed = 0f;
        float startExposure = colorAdjustments.postExposure.value;

        while (elapsed < exposureFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Time.timeScale'den baðýmsýz
            float t = elapsed / exposureFadeDuration;
            float curveValue = exposureCurve.Evaluate(t);

            // Exposure deðerini güncelle
            colorAdjustments.postExposure.value = Mathf.Lerp(startExposure, targetExposure, curveValue);

            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        
        // Final deðeri garantile
        colorAdjustments.postExposure.value = targetExposure;
        
        gameOverCanvas.SetActive(true);
        RectTransform gameOverRect = gameOverCanvas.GetComponent<RectTransform>();
        gameOverRect.localScale = Vector3.zero;
        gameOverRect.DOScale(Vector3.one, animationDuration)
            .SetEase(openEase) // OutBack - pop efekti
            .SetUpdate(true);

        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    
}
