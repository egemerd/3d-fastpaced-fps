using UnityEngine;

public class AR : Gun
{

    public override void Update()
    {
        base.Update();
        if (Input.GetButton("Fire1"))
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
            Debug.Log("AR hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("AR hit enemy: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
            }
            BulletMark(hit);
        }
    }
    public override void BulletMark(RaycastHit hit)
    {
        GameObject bulletMark = Instantiate(base.bulletMarkPrefab, hit.point, Quaternion.LookRotation(hit.normal));

    }
}
