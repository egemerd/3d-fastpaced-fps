using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private GunManagerData gunManagerDataSO;
    [SerializeField] private WeaponStateSO weaponStateSO;
    [SerializeField] private GunUIData[] gunUIDataList;
    [SerializeField] private GameObject[] weaponObjects;

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
        else if (InputManager.Instance.switchWeaponAction4.IsPressed())
            SwitchTo(3);
        //else if (InputManager.Instance.switchWeaponAction5.IsPressed())
        //    SwitchTo(4);
        Debug.Log(InputManager.Instance.switchWeaponAction4.IsPressed());
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
        for (int i = 0; i < weaponObjects.Length; i++)
        {
            GameObject weaponObj = weaponObjects[i];
            if (weaponObj == null) continue;

            bool shouldBeActive = (i == index);

            if (shouldBeActive)
            {
                weaponObj.SetActive(true);
            }
            else
            {
                if (weaponObj.activeSelf)
                {
                    ResetWeaponAnimator(weaponObj);
                    weaponObj.SetActive(false);
                }
            }
        }
    }

    public void EquipGun(int index)
    {
        if (index == currentIndex) return;
        if (index < 0 || index >= guns.Length) return;

        currentIndex = index;
        UpdateWeaponObjects(index);
    }

    private void ResetWeaponAnimator(GameObject weaponObj)
    {
        Animator animator = weaponObj.GetComponent<Animator>();
        if (animator != null)
        {
            // Idle yerine, Animator Controller'ýndaki GERÇEK state ismini yaz
            // Örneðin "Idle", "WeaponIdle", "Rest" ne ise onu kullan
            animator.Rebind();
            animator.Play("Idle", 0, 0f);
            animator.Update(0f);
        }
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

