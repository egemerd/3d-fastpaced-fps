using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool isGameStarted = false;

    LevelManager levelManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }   

        SubscribeToEvents();
        levelManager = FindObjectOfType<LevelManager>();

    }

    private void Start()
    {
        SubscribeToEvents();    
    }
    private void OnDisable()
    {
        GameEvents.current.onDoorClosed -= HandleDoorClosed;
        GameEvents.current.onPlayerDeath -= GameOver;
    }

    private void SubscribeToEvents()
    {
        GameEvents.current.onDoorClosed += HandleDoorClosed;
        GameEvents.current.onPlayerDeath += GameOver;
    }
    private void HandleDoorClosed()
    {
        GameOver();
    }

    private void GameOver()
    {
        Debug.Log("[Game Manager] Game Over! The door has closed.");
        levelManager.GameOverCanvas();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    


}
