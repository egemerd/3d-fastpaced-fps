using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    [SerializeField] GameObject shotgunCrosshair;
    [SerializeField] GameObject pistolCrosshair;

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
        }
        else if (InputManager.Instance.switchWeaponAction2.IsPressed())
        {
            selectedWeapon = 1;
            shotgunCrosshair.SetActive(false);
            pistolCrosshair.SetActive(true);
        }
        else if (InputManager.Instance.switchWeaponAction3.IsPressed())
        {
            selectedWeapon = 2;
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
}
