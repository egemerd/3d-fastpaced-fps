using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

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

    [Header("Level End Animation")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease openEase = Ease.OutBack; // Açýlýþ animasyonu
    [SerializeField] private Ease closeEase = Ease.InBack;

    LevelStats levelStats;
    private void Awake()
    {
        Instance = this;
        levelStats = FindObjectOfType<LevelStats>();
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
}
