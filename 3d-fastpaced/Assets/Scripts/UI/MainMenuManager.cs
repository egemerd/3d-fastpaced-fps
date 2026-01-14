using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject shutterSlider;
    [SerializeField] private GameObject slidersPanel;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject crosshairCanvas;



    bool isGameStarting;

    private void Awake()
    {
        if (GameManager.isGameStarted)
        {
            gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        crosshairCanvas.SetActive(false);
            
    }

    public void StartGame()
    {
        Debug.Log("[MainMenuManager] Start Game button pressed.");        
        StartCoroutine(GameStartCoroutine());
    }

    public void Settings()
    {
        if (isGameStarting) return;
        settingsPanel.SetActive(true);
        buttonsPanel.SetActive(false); 
        
    }

    public void BackToMainMenu()
    {
        if (isGameStarting) return;
        settingsPanel.SetActive(false);
        buttonsPanel.SetActive(true);
    }       

    public void QuitGame()
    {
        if(isGameStarting) return;
        Application.Quit();
    }   

    private IEnumerator GameStartCoroutine()
    {
        isGameStarting = true;
        buttonsPanel.SetActive(false);
        yield return new WaitForSecondsRealtime(0.1f);
        shutterSlider.transform.DOMove(
            shutterSlider.transform.position + Vector3.up * 3000f,
            1f
        ).SetUpdate(true).SetEase(Ease.InOutQuad);
        yield return new WaitForSecondsRealtime(1f);
        slidersPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        isGameStarting = false;
        crosshairCanvas.SetActive(true);

        GameManager.isGameStarted = true;
    }
}
