using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int currentLevelIndex;

    private GameObject levelEndCanvas;
    private RectTransform canvasRectTransform;

    [SerializeField] private List<LevelDataSO> levelDatas;

    [SerializeField] Material wallMaterial;
    [SerializeField] Material floorMaterial;

    [Header("Level End Animation")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutBack; // Açýlýþ animasyonu
    [SerializeField] private Ease closeEase = Ease.InBack;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        FindLevelEndCanvas();

        wallMaterial.color = levelDatas[currentLevelIndex].wallColor;
        floorMaterial.color = levelDatas[currentLevelIndex].floorColor;
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
        canvasRectTransform.DOScale(Vector3.zero, animationDuration)
            .SetEase(closeEase) // InBack - içeri çekilip küçülür
            .SetUpdate(true);
        yield return new WaitForSecondsRealtime(2f);
        levelEndCanvas.SetActive(false);
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentLevelIndex + 1);
    }
    
    public void ActivateLevelEndCanvas()
    {
        if (levelEndCanvas == null) return;

        levelEndCanvas.transform.parent.gameObject.SetActive(true);
        levelEndCanvas.SetActive(true);
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
}
