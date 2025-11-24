using UnityEngine;
using UnityEngine.PlayerLoop;

public class Shotgun : Gun
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
        

        for (int i = 0; i < gunData.bulletsPerShot; i++)
        {
            Vector3 spreadDirection = GetSpreadDirection();

            if (Physics.Raycast(mainCamera.position,spreadDirection,out hit,gunData.fireRange))
            {
                StartBulletFire(hit.point,hit);
            }
            else
            {
                Vector3 endPoint = mainCamera.position + spreadDirection * gunData.fireRange;
                StartBulletFire(endPoint,hit);
            }
        }
        
    } 

    private Vector3 GetSpreadDirection()
    {
        Vector3 forward = mainCamera.forward;

        float spreadInRadians = gunData.spreadAngle * Mathf.Deg2Rad;

        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Mathf.Sqrt(Random.Range(0f, 1f)) * spreadInRadians;

        float x = Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance;
        float y = Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance;

        Vector3 right = mainCamera.right;
        Vector3 up = mainCamera.up;

        Vector3 spreadOffset = right * x + up * y;
        Vector3 finalDirection = (forward + spreadOffset).normalized;

        return finalDirection;
    }
}
