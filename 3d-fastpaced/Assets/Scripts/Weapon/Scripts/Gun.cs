using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [SerializeField] public Transform mainCamera;
    [SerializeField] public Transform gunMuzzle;
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
        if (isReloading) return;
        if (currentAmmo >= gunData.magazineSize) return;

        
            if (gunData.reloadAoutomatic && currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }
            if(InputManager.Instance.reloadAction.IsPressed() && !gunData.reloadAoutomatic)
            {
                StartCoroutine(Reload());
            }
        
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");
        yield return new WaitForSeconds(gunData.reloadTime);
        currentAmmo = gunData.magazineSize;
        Debug.Log("Reloaded");

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

    public void StartBulletFire(Vector3 target,RaycastHit hit)
    {
        StartCoroutine(BulletFire(target,hit));
    }

    private IEnumerator BulletFire(Vector3 target , RaycastHit hit)
    {
        GameObject bulletTrail = Instantiate(gunData.gunTrailPrefab, gunMuzzle.position, Quaternion.identity);
        while (bulletTrail != null && Vector3.Distance(bulletTrail.transform.position , target) > 0.1f)
        {
            Debug.Log("Moving bullet trail");
            bulletTrail.transform.position = Vector3.MoveTowards(bulletTrail.transform.position, target,
                Time.deltaTime * gunData.bulletSpeed);
            yield return null;
        }

        Destroy(bulletTrail);

        if(hit.collider != null)
        {
            BulletHitFX(hit);
        }
    }

    private void BulletHitFX(RaycastHit hit)
    {
        
        Vector3 hitPoint = hit.point + hit.normal * 0.01f;
        //GameObject hitParticle = Instantiate(gunData.hitParticlePrefab, hitPoint, Quaternion.LookRotation(hit.normal));
        GameObject bulletMark = Instantiate(gunData.bulletMarkPrefab, hitPoint, Quaternion.LookRotation(hit.normal));
    
        //hitParticle.transform.parent = hit.collider.transform;
        bulletMark.transform.parent = hit.collider.transform;

        //Destroy(hitParticle, 3f);
        Destroy(bulletMark, 3f);
    }

    public abstract void Shoot();
    
}
