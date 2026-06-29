using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private GunManagerData gunManagerDataSO;
    [SerializeField] private WeaponStateSO weaponStateSO;
    [SerializeField] private GunUIData[] gunUIDataList;

    public int selectedWeapon = -1;
    int currentIndex = -1;
    private AllGunData[] guns;
    void Start()
    {
        guns = gunManagerDataSO.guns;
        SwitchTo(0);
    }

    private void Update()
    {
        SwitchInput();
    }

    private void SwitchInput()
    {
        if (InputManager.Instance.switchWeaponAction1.IsPressed())
            SwitchTo(0);
        else if (InputManager.Instance.switchWeaponAction2.IsPressed())
            SwitchTo(1);
        else if (InputManager.Instance.switchWeaponAction3.IsPressed())
            SwitchTo(2);
    }

    private void SwitchTo(int index)
    {
        if (index == selectedWeapon) return;


        SetUI(selectedWeapon, false);

        selectedWeapon = index;
        weaponStateSO.selectedGunIndex = index;

        SetUI(selectedWeapon, true);

        EquipGun(index);
    }

    private void SetUI(int index, bool isActive)
    {
        if (index < 0 || index >= gunUIDataList.Length) return;

        var ui = gunUIDataList[index];
        if (ui.crosshair != null) ui.crosshair.SetActive(isActive);
        if (ui.bigIcon != null) ui.bigIcon.SetActive(isActive);
        if (ui.smallIcon != null) ui.smallIcon.SetActive(!isActive); // Small ters çalýþýyor
    }

    private void UpdateWeaponObjects(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(i == index);
    }

    public void EquipGun(int index)
    {
        if (index == currentIndex) return;
        if (index < 0 || index >= guns.Length) return;

        currentIndex = index;
        UpdateWeaponObjects(index);
    }
}

[System.Serializable]
public struct GunUIData
{
    public string gunName; 
    public GameObject crosshair;
    public GameObject bigIcon;
    public GameObject smallIcon;
}
