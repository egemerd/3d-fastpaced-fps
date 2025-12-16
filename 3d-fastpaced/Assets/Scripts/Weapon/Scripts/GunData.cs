using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Data", menuName = "Weapon/Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;

    public LayerMask whatToHit;

    [Header("Shotgun Config")]
    public float spreadAngle;
    public int bulletsPerShot;

    [Header("Fire Config")]
    public float fireRate;
    public float fireRange;

    [Header("Reload Config")]
    public float reloadTime;
    public int magazineSize;
    public bool reloadAoutomatic;

    [Header("Damage Config")]
    public float gunDamage;

    [Header("VFX")]
    public GameObject gunTrailPrefab;
    public float bulletSpeed;
    [SerializeField] public GameObject bulletMarkPrefab;
    [SerializeField] public GameObject hitParticlePrefab;

    [Header("Weapon Shoot Animation")]
    public float shootAnimDuration;
    public float returnShootAnimDuration;
    public float movementDistance;
}
