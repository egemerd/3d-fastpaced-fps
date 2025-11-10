using UnityEngine;

public class Pistol : Gun
{
    public override void Update()
    {
        base.Update();
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleReload();
        }
    }
    public override void Shoot()
    {
        RaycastHit hit;
    
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, gunData.fireRange))
        {
            Debug.Log("Pistol hit: " + hit.collider.name);
            BulletMark(hit);
        }
    }
    public override void BulletMark(RaycastHit hit)
    {
        GameObject bulletMark = Instantiate(base.bulletMarkPrefab, hit.point, Quaternion.LookRotation(hit.normal));
    }
}
