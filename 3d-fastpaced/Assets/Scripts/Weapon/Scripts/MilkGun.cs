using UnityEngine;

public class MilkGun : Gun
{
    private void Update()
    {
        if(InputManager.Instance.fireAction.WasPressedThisFrame())
        {
            TryShoot();
        }
    }


    public override void TryShoot()
    {
        
    }

    public override void Shoot()
    {
        Debug.Log("MilkGun fired!");
    }

    public override void WeaponShootAnimation()
    {

    }
}
