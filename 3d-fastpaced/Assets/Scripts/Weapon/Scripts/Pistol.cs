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
            if(hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Pistol hit enemy: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
            }   
            
        }
    }

    public override void WeaponShootAnimation()
    {

    }

}
