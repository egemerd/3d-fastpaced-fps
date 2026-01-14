using TMPro;
using UnityEngine;

public class LevelStats : MonoBehaviour
{
    public float killedEnemies;
    public float gameTime;

    [SerializeField] private TextMeshProUGUI killedEnemiesText;
    [SerializeField] private TextMeshProUGUI gameTimeText;

    void Start()
    {
        GameEvents.current.onEnemyDeath += CountKilledEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime; 
        gameTimeText.text = "Game Time: " + gameTime + "s";   
    }
    public void ResetGameTimer()
    {
        gameTime = 0f;
    }

    void CountKilledEnemies()
    {
        killedEnemies++;
        killedEnemiesText.text = "Killed Enemies: " + killedEnemies;
    }
}
