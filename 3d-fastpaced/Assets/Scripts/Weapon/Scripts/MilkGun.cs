using System;
using UnityEngine;

public class MilkGun : Gun
{
    [SerializeField] private MilkGunAttackArea attackArea;
    [SerializeField] private ParticleSystem milkGunParticle;
    private float damagePerSecond = 10;

    private bool isFiring = false;
    private int damage;

    private void Awake()
    {
        attackArea.EnemyEnteredEvent += StartDamaging;
        attackArea.EnemyExitedEvent += StopDamaging;
    }

    protected override void Start()
    {
        base.Start();
        damagePerSecond = gunData.gunDamage;  
    }


    public override void Update()
    {
        base.Update();

        if (InputManager.Instance.fireAction.IsPressed())
        {
            Debug.Log("MilkGun: Firing");
            if (!isFiring) StartFiring();
        }
        else
        {
            if (isFiring) StopFiring();
        }
    }

    

    void StartFiring()
    {
        Debug.Log("MilkGun: StartFiring");
        isFiring = true;
        attackArea.gameObject.SetActive(true);
        milkGunParticle.Play();
    }

    void StopFiring()
    {
        Debug.Log("MilkGun: StopFiring");
        isFiring = false;
        attackArea.gameObject.SetActive(false);
        milkGunParticle.Stop();
    }



    void StartDamaging(EnemyAI enemy)
    {
        if(enemy.TryGetComponent<IBurnable>(out var burnable))
        {
            burnable.StartBurning(damagePerSecond);
        }
    }

    void StopDamaging(EnemyAI enemy)
    {
        if (enemy.TryGetComponent<IBurnable>(out var burnable))
        {
            burnable.StopBurning();
        }
    }


    public override void Shoot()
    {
    }

    public override void WeaponShootAnimation()
    {
    }
}
