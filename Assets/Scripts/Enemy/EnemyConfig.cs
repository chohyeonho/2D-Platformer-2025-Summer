using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Config/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
	[Header("기본 스탯")]
	public float maxHealth = 3f;
	public float moveSpeed = 3f;

	[Header("AI 행동 간격")]
	public Vector2 thinkIntervalRange = new Vector2(2f, 5f);

	[Header("비활성화 딜레이")]
	public float deactivateDelay = 5f;

	// ▶︎ 피격 및 사망 반동 설정
	[Header("피격 및 사망 반동 설정")]

	// ▶︎ 사망 직후 위로 튕겨 오르는 연출용 반동 힘
	public float deathBounceForce = 5f;

	[Header("사운드 클립")]
	public AudioClip stompClip;  // 밟혔을 때
	public AudioClip dieClip;    // 사망 시
}
