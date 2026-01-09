using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int currentLevelIndex;

    private GameObject levelEndCanvas;

    [SerializeField] private List<LevelDataSO> levelDatas;

    [SerializeField] Material wallMaterial;
    [SerializeField] Material floorMaterial;

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
        levelEndCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void FindLevelEndCanvas()
    {
        LevelEndCanvasMarker marker = FindObjectOfType<LevelEndCanvasMarker>(true); // true = inactive objeleri de ara

        if (marker != null)
        {
            levelEndCanvas = marker.gameObject;
            levelEndCanvas.SetActive(false);
            Debug.Log($"[LevelManager] LevelEndCanvas found: {levelEndCanvas.name}");
        }
        else
        {
            Debug.LogError("[LevelManager] LevelEndCanvas with LevelEndCanvasMarker not found!");
        }
    }
}
