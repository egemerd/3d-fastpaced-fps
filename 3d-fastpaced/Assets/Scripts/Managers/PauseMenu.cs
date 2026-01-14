using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused;
    public static bool isPauseManualy;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject buttons;


    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.Instance.gamePauseAction.WasPressedThisFrame())
        {
            if (isPaused)
            {
                ResumeGameMenu();
            }
            else
            {
                PauseGameMenu();
            }
        }
        
    }

    public void PauseGameMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGameMenu()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void SettingsMenu()
    {
        buttons.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void GoBackPauseMenu()
    {
        settingsPanel.SetActive(false);
        buttons.SetActive(true);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
