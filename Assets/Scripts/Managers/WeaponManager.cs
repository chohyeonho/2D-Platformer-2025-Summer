using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private WeaponData startingWeapon;
    private List<WeaponData> weaponInventory = new List<WeaponData>();

    private int currentWeaponIndex = -1;

    private void Awake()
    {
        // 시작 무기가 있다면 인벤토리에 추가하고 초기 무기로 설정
        if (startingWeapon != null)
        {
            weaponInventory.Add(startingWeapon);
            currentWeaponIndex = 0;
        }
    }

    public WeaponData GetCurrentWeapon()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex >= weaponInventory.Count)
        {
            return null;
        }
        return weaponInventory[currentWeaponIndex];
    }

    // 3단계에서 구현할 무기 교체 메서드
    public void SwitchNextWeapon()
    {
        if (weaponInventory.Count <= 1) return;

        currentWeaponIndex = (currentWeaponIndex + 1) % weaponInventory.Count;
        Debug.Log($"Switched to weapon: {GetCurrentWeapon().weaponName}");
        
        // TODO: 무기 교체 이벤트를 발생시켜 PlayerAttack과 UI에 알림
    }
}
