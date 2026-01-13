using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    private void OnEnable()
    {
        GameEvents.current.onDoorClosed += HandleDoorClosed;
    }
    private void OnDisable()
    {
        GameEvents.current.onDoorClosed -= HandleDoorClosed;
    }
    private void HandleDoorClosed()
    {
        GameOver();
    }

    private void GameOver()
    {
        Debug.Log("[Game Manager] Game Over! The door has closed.");
    }
}
