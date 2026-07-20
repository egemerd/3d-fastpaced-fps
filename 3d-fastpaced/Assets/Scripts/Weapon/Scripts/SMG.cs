using UnityEngine;

public class SMG : Gun
{
    [SerializeField] private GameObject magazine;
    [SerializeField] private float magazineForce;
    [SerializeField] private float kickUpwardBias = 0.3f; // yukarý yönlü bias
    private WeaponRecoil recoil;

    private Vector3 magazineInitialPos;
    [SerializeField] private Rigidbody rb;
    protected override void Start()
    {
        base.Start();
        recoil = GetComponent<WeaponRecoil>();
        magazineInitialPos = magazine.transform.localPosition;
        ReloadKickAnimation(); // Baþlangýçta tetikleme animasyonunu tetikle
    }

   
    public override void Update()
    {
        base.Update();
        if (InputManager.Instance.fireAction.IsPressed())
        {
            TryShoot();
        }
        else
        {
            recoil?.ResetPattern(); // tetik býrakýlýnca pattern sýfýrlansýn
        }
        HandleReload();

    }


    public override void Shoot()
    {
        Quaternion recoilRotation = Quaternion.Euler(
            -recoil.CurrentAimPitchOffset,  // pitch: yukarý kayma (ekseni modeline göre kontrol et, ters gelirse iþareti deðiþtir)
            recoil.CurrentAimYawOffset,     // yaw: saða/sola kayma
            0f
        );
        Vector3 shootDirection = recoilRotation * mainCamera.forward;

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, gunData.fireRange, gunData.whatToHit))
        {
            Debug.Log("SMG hit: " + hit.collider.name);

            StartBulletFire(hit.point, hit);
        }
        else
        {
            Vector3 missTarget = mainCamera.position + mainCamera.forward * gunData.fireRange;

            StartBulletFire(missTarget, hit);
        }
        recoil?.ApplyRecoil();

    }

    public override void WeaponShootAnimation()
    {
        
    }

    private void ReloadKickAnimation()
    {
        Debug.Log("Reload kick animation triggered");
        Vector3 kickDirection = (mainCamera.forward + Vector3.up * kickUpwardBias).normalized;
        rb.AddForce(kickDirection * magazineForce, ForceMode.VelocityChange);
    }

    private void onMagazineDrop()
    {
        if (magazine != null)
        {
            magazine.SetActive(true);
            Animator animator = magazine.GetComponent<Animator>();
        }
    }

    private void onMagazineEnd()
    {
        ReloadKickAnimation();
        //magazine.transform.position = magazineInitialPos;
        //magazine.SetActive(false);
    }

    
}
