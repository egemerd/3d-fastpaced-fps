using UnityEngine;

public class SMG : Gun
{
    private WeaponRecoil recoil;

    protected override void Start()
    {
        base.Start();
        recoil = GetComponent<WeaponRecoil>();
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

    }
}
