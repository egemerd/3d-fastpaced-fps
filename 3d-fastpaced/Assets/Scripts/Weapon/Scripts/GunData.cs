using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Data", menuName = "Weapon/Gun Data")]
public class GunData : ScriptableObject
{
    public string gunName;

    public LayerMask whatToHit;

    [Header("Fire Config")]
    public float fireRate;
    public float fireRange;

    [Header("Reload Config")]
    public float reloadTime;
    public int magazineSize;
}
