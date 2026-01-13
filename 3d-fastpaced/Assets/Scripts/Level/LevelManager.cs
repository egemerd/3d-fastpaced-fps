using NUnit.Framework;
using System.Collections.Generic;
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
    [SerializeField] private Ease animationEase = Ease.OutBack;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLevelIndex = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex).buildIndex;
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
        levelEndCanvas.SetActive(false);
        SceneManager.LoadScene(currentLevelIndex + 1);
    }

    public void ActivateLevelEndCanvas()
    {
        if (levelEndCanvas == null) return;

        levelEndCanvas.transform.parent.gameObject.SetActive(true);
        levelEndCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //  Scale animasyonu (0'dan 1'e)
        if (canvasRectTransform != null)
        {
            canvasRectTransform.localScale = Vector3.zero; // Baþlangýçta küçük
            canvasRectTransform.DOScale(Vector3.one, animationDuration)
                .SetEase(animationEase)
                .SetUpdate(true); // Time.timeScale = 0 olsa bile çalýþýr
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
