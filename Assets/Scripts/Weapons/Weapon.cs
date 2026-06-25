using UnityEngine;

/// <summary>
/// 게임 월드에 존재하는 무기의 실체.
/// WeaponData(설계도)를 참조하여 실제 행동을 정의할 수 있습니다.
/// </summary>
public class Weapon : MonoBehaviour
{
	[Tooltip("이 무기의 데이터 에셋(설계도)")]
	[SerializeField] private WeaponData weaponData;

	// 외부에서 이 무기의 데이터를 읽을 수 있도록 프로퍼티를 제공합니다.
	public WeaponData Data => weaponData;
}
