using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [SerializeField] private Transform mainCamera;
    public PlayerController playerController;

    private int currentAmmo = 0;    
    private float netTimeToFire = 0f;   
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = gunData.magazineSize;
            
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


    private void TryShoot()
    {
        if (isReloading || currentAmmo <= 0)
            return;
 
        if (Time.time >= netTimeToFire)
        {
            netTimeToFire = Time.time + 1 / gunData.fireRate;
            HandleShoot();
        }
    }
    public void HandleShoot()
    {
        currentAmmo--;
        Shoot();
    }
    public abstract void Shoot();
}
