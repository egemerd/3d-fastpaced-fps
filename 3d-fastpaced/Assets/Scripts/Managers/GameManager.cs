using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool isGameStarted = false;

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
        //GameOver();
    }

    private void GameOver()
    {
        Debug.Log("[Game Manager] Game Over! The door has closed.");
        SceneManager.LoadScene(0);
    
    }


}
