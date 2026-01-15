using UnityEngine;
using UnityEngine.PlayerLoop;

public class Shotgun : Gun
{

    private Vector3 initialLocalPos;
    private Coroutine recoilRoutine;
    private CameraShootEffect cameraShootEffect;

    private void Awake()
    {
        cameraShootEffect = mainCamera.GetComponent<CameraShootEffect>();
        initialLocalPos = transform.localPosition;
    }
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
        AudioManager.Instance.PlaySFX("Shotgun", 0.7f);
        if (cameraShootEffect != null)
        {
            cameraShootEffect.ApplyShootImpact();
        }
        WeaponShootAnimation();
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

    public override void WeaponShootAnimation()
    {
        if (recoilRoutine != null)
        {
            StopCoroutine(recoilRoutine);
        }
        recoilRoutine = StartCoroutine(RecoilAnim());
    }

    private System.Collections.IEnumerator RecoilAnim()
    {
        Debug.Log("Shotgun Recoil Animation");

        float moveAmount = gunData.movementDistance;
        Vector3 recoilOffset = Vector3.forward * moveAmount;
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = initialLocalPos - recoilOffset;
        float duration = gunData.shootAnimDuration;
        float durationReturn = gunData.returnShootAnimDuration;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;  
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / durationReturn;
            transform.localPosition = Vector3.Lerp(targetPos, initialLocalPos, t);
            yield return null;
        }

        transform.localPosition = initialLocalPos;
        recoilRoutine = null;
    }
}
