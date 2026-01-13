using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject shutterSlider;
    [SerializeField] private GameObject slidersPanel;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject buttonsPanel;


    bool isGameStarting;    

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }



    public void StartGame()
    {
        Debug.Log("[MainMenuManager] Start Game button pressed.");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(GameStartCoroutine());
    }

    public void Settings()
    {
        if (isGameStarting) return;
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
        yield return new WaitForSeconds(0.2f);
        shutterSlider.transform.DOMove(shutterSlider.transform.position + Vector3.up * 2000f , 2).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(2f);
        slidersPanel.SetActive(false);
        isGameStarting = false;
    }
}
