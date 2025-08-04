using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
	// ▶︎ 기본 이동 속도 및 점프력 설정
	[Header("이동 및 점프 설정")]

	// ▶︎ 기본 이동 속도
	public float moveSpeed = 5f;

	// ▶︎ 점프 시 적용할 힘
	public float jumpPower = 12f;

	// ▶︎ 이동 감속 시 적용할 속도 (버튼 뗐을 때 감속용)
	[Header("이동 세부 설정")]
	public float decelerationSpeed = 0.5f;

	// ▶︎ 체력 수치 관련 설정
	[Header("체력 설정")]

	// ▶︎ 최대 체력
	public int maxHealth = 3;

	// ▶︎ 피격 후 무적 지속 시간
	public float invincibleTime = 3f;

	// ▶︎ 공격 관련 설정들
	[Header("공격 설정")]

	// ▶︎ 공격 범위 반지름
	public float attackRange = 1.5f;

	// ▶︎ 공격 데미지 수치
	public float attackDamage = 1f;

	// ▶︎ 공격 쿨타임 (초)
	public float attackCooldown = 0.15f;

	// ▶︎ 적을 밟았을 때 튕겨 오르는 반동 힘
	public float bouncePowerOnAttack = 10f;

	// ▶︎ 각종 플레이어 사운드 클립들
	[Header("사운드 클립")]

	public AudioClip swingClip;
	public AudioClip hitClip;
	public AudioClip jumpClip;
	public AudioClip damagedClip;
	public AudioClip dieClip;
	public AudioClip itemClip;
	public AudioClip finishClip;
}
