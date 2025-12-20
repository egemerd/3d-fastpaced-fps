using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Enemy Attributes")]
    public string enemyName;
    public float health;
    public float enemyDamage;
    public float moveSpeed; 

}
