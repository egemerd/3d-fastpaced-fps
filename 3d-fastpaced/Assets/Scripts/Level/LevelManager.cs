using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int currentLevelIndex;

    private GameObject levelEndCanvas;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentLevelIndex = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex).buildIndex;
        FindLevelEndCanvas();
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
