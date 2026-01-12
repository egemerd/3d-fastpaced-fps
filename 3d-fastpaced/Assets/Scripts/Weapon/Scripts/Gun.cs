using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunData gunData;
    [SerializeField] public Transform mainCamera;
    [SerializeField] public Transform gunMuzzle;
    [SerializeField] public ParticleSystem gunShootParticle;
    [SerializeField] Animator myAnim;
    public PlayerController playerController;
    

    private int currentAmmo = 0;    
    private float nextTimeToFire = 0f;   
    private bool isReloading = false;
    private Coroutine reloadRoutine;
    

    
    protected virtual void Start()
    {
        currentAmmo = gunData.magazineSize;
        myAnim = GetComponent<Animator>();
    }

    public virtual void Update()
    {

    }
    private void OnEnable()
    {
        // Aktifleþirken cooldown sýfýrla, reload durumunu kapat
        nextTimeToFire = 0f;
        isReloading = false;
    }

    private void OnDisable()
    {
        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
        }
        isReloading = false;
    }

    public void HandleReload()
    {
        if (isReloading) return;
        if (currentAmmo >= gunData.magazineSize) return;

        // Otomatik reload sadece mermi bitince
        if (gunData.reloadAoutomatic && currentAmmo <= 0)
        {
            reloadRoutine = StartCoroutine(Reload());
            return;
        }

        // Manuel reload - R tuþuna basýlý tutmaya gerek yok, bir kez bastý mý yeterli
        if (InputManager.Instance.reloadAction.WasPressedThisFrame())
        {
            reloadRoutine = StartCoroutine(Reload());
        }

    }

    private IEnumerator Reload()
    {
        isReloading = true;
        myAnim.SetTrigger("Reload");
        Debug.Log("Reloading");
        yield return new WaitForSeconds(gunData.reloadTime);    
        currentAmmo = gunData.magazineSize;
        Debug.Log("Reloaded");

        isReloading = false;
        reloadRoutine = null;
        nextTimeToFire = 0f;
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
        gunShootParticle.Play();
        while (bulletTrail != null && Vector3.Distance(bulletTrail.transform.position , target) > 0.1f)
        {
            Debug.Log("Moving bullet trail");
            bulletTrail.transform.position = Vector3.MoveTowards(bulletTrail.transform.position, target,
                Time.deltaTime * gunData.bulletSpeed);
            yield return null;
        }

        Destroy(bulletTrail, 1f);
        
        if (hit.collider != null)
        {
            BulletHitFX(hit);
        }

        if(hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Body") || hit.collider.CompareTag("Head"))
        {
            ApplyDamage(hit);
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

    protected void ApplyDamage(RaycastHit hit)
    {
        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
        if (damageable != null)
        {
            if (hit.collider.CompareTag("Body"))
            {
                Debug.Log("[Gun] Applying body damage");
                damageable.TakeDamage(gunData.gunDamage);
            }
            else if (hit.collider.CompareTag("Head"))
            {
                damageable.TakeDamage(gunData.gunDamage * gunData.headshotMultiplier);
            }
            Debug.Log("Hit " + hit.collider.name + " for " + gunData.gunDamage + " damage.");
        }
        else
        {
            Debug.Log("Hit " + hit.collider.name + " but it is not damageable.");
        }
    }

    public abstract void Shoot();
    public abstract void WeaponShootAnimation();    

}
