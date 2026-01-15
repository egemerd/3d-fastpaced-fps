using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    [SerializeField] GameObject shotgunCrosshair;
    [SerializeField] GameObject pistolCrosshair;

    [SerializeField] GameObject shotgunBig;
    [SerializeField] GameObject pistolBig;

    [SerializeField] GameObject shotgunSmall;
    [SerializeField] GameObject pistolSmall;
    
    void Start()
    {
        SelectWeapon();
    }

    
    void Update()
    {
        SwitchInput();
        SelectWeapon();
    }

    void SwitchInput()
    {
        if (InputManager.Instance.switchWeaponAction1.IsPressed())
        {
            selectedWeapon = 0;
            shotgunCrosshair.SetActive(true);
            pistolCrosshair.SetActive(false);
            ShotgunEquip();

        }
        else if (InputManager.Instance.switchWeaponAction2.IsPressed())
        {
            selectedWeapon = 1;
            shotgunCrosshair.SetActive(false);
            pistolCrosshair.SetActive(true);
            PistolEquip();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    private void ShotgunEquip()
    {
        shotgunBig.SetActive(true);
        pistolBig.SetActive(false);
        shotgunSmall.SetActive(false);
        pistolSmall.SetActive(true);
    }

    private void PistolEquip()
    {
        pistolBig.SetActive(true);
        shotgunBig.SetActive(false);
        pistolSmall.SetActive(false);
        shotgunSmall.SetActive(true);
    }
}
