using UnityEngine;

public class AR : Gun
{

    public override void Update()
    {
        base.Update();
        if (InputManager.Instance.fireAction.IsPressed())
        {
            TryShoot();
        }
        HandleReload();
    }

    public override void Shoot()
    {
        RaycastHit hit;
        Vector3 target = Vector3.zero;

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, gunData.fireRange))
        {
            Debug.Log("AR hit: " + hit.collider.name);
            target = hit.point;
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("AR hit enemy: " + hit.collider.name);
                GameEvents.current.TriggerEnemyHit(base.gunData.gunDamage);
            }
            
        }
        else
        {
            target = mainCamera.position + mainCamera.forward * gunData.fireRange;
        }
        StartBulletFire(target,hit);
        
    }

    
}
