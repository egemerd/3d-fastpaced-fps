using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [SerializeField] public Transform mainCamera;
    [SerializeField] public GameObject bulletMarkPrefab;
    public PlayerController playerController;

    private int currentAmmo = 0;    
    private float nextTimeToFire = 0f;   
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = gunData.magazineSize;
            
    }

    public virtual void Update()
    {

    }

    public void HandleReload()
    {
        if (!isReloading && currentAmmo <= gunData.magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(gunData.reloadTime);
        currentAmmo = gunData.magazineSize;
        Debug.Log("Reloaded...");

        isReloading = false;
    }


    public void TryShoot()
    {
        if (isReloading || currentAmmo <= 0)
            return;
 
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1 / gunData.fireRate;
            HandleShoot();
        }
    }


    private void HandleShoot()
    {
        currentAmmo--;
        Shoot();
    }
    public abstract void Shoot();
    public abstract void BulletMark(RaycastHit hit);
}
