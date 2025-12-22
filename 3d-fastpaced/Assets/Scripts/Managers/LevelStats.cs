using TMPro;
using UnityEngine;

public class LevelStats : MonoBehaviour
{
    private float killedEnemies;
    private float gameTime;

    [SerializeField] private TextMeshProUGUI killedEnemiesText;

    void Start()
    {
        GameEvents.current.onEnemyDeath += CountKilledEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime; 
    }

    void CountKilledEnemies()
    {
        killedEnemies++;
        killedEnemiesText.text = "Killed Enemies: " + killedEnemies;
    }
}
