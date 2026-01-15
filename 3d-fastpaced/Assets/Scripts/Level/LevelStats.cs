using TMPro;
using UnityEngine;

public enum LevelRank
{
    F,
    D,
    C,
    B,
    A,
    S
}


public class LevelStats : MonoBehaviour
{
    public float killedEnemies;
    public float gameTime;

    [SerializeField] private TextMeshProUGUI killedEnemiesText;
    [SerializeField] private TextMeshProUGUI gameTimeText;

    [Header("Rank Settings - Kill Rate Thresholds (kills per minute)")]
    [SerializeField] private float sRankKillRate = 50f;  // 50 kill/dakika = S Rank
    [SerializeField] private float aRankKillRate = 40f;  // 40 kill/dakika = A Rank
    [SerializeField] private float bRankKillRate = 30f;  // 30 kill/dakika = B Rank
    [SerializeField] private float cRankKillRate = 20f;  // 20 kill/dakika = C Rank
    [SerializeField] private float dRankKillRate = 10f;  // 10 kill/dakika = D Rank
                                                         // 10'dan az = F Rank

    [Header("Rank Colors")]
    [SerializeField] private Color sRankColor = new Color(1f, 0.84f, 0f); // Gold
    [SerializeField] private Color aRankColor = new Color(0.75f, 0.75f, 0.75f); // Silver
    [SerializeField] private Color bRankColor = new Color(0.8f, 0.5f, 0.2f); // Bronze
    [SerializeField] private Color cRankColor = Color.green;
    [SerializeField] private Color dRankColor = Color.yellow;
    [SerializeField] private Color fRankColor = Color.red;

    private LevelRank currentRank;
    private float currentKillRate;

    void Start()
    {
        GameEvents.current.onEnemyDeath += CountKilledEnemies;
        currentRank = LevelRank.F;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.isGameStarted) return;
        gameTime += Time.deltaTime; 
        gameTimeText.text = "Game Time: " + gameTime + "s";   
        
        CalculateRank();
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

    private void CalculateRank()
    {
        // Kill rate hesapla (kills per minute)
        if (gameTime > 0)
        {
            currentKillRate = (killedEnemies / gameTime) * 60f;
            Debug.Log("Current Kill Rate: " + currentKillRate + " kills/min");
        }
        else
        {
            currentKillRate = 0f;
        }

        LevelRank newRank = DetermineRankByKillRate();
        currentRank = newRank;
    }

    private LevelRank DetermineRankByKillRate()
    {
        // Kill rate'e göre rank belirle
        if (currentKillRate >= sRankKillRate)
            return LevelRank.S;
        else if (currentKillRate >= aRankKillRate)
            return LevelRank.A;
        else if (currentKillRate >= bRankKillRate)
            return LevelRank.B;
        else if (currentKillRate >= cRankKillRate)
            return LevelRank.C;
        else if (currentKillRate >= dRankKillRate)
            return LevelRank.D;
        else
            return LevelRank.F;
    }

    public string GetCurrentRankString()
    {
        return currentRank.ToString();
    }   

    public Color GetCurrentRankColor()
    {
        return currentRank switch
        {
            LevelRank.S => sRankColor,
            LevelRank.A => aRankColor,
            LevelRank.B => bRankColor,
            LevelRank.C => cRankColor,
            LevelRank.D => dRankColor,
            _ => fRankColor,
        };
    }
}
