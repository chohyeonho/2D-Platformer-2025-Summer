using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Config/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
	[Header("기본 스탯")]
	public float maxHealth = 3f;
	public float moveSpeed = 1f;

	[Header("AI 행동 간격")]
	public float minThinkTime = 2f;
	public float maxThinkTime = 5f;

	[Header("비활성화 딜레이")]
	public float deactivateDelay = 5f;

	[Header("사운드 클립")]
	public AudioClip stompClip;
	public AudioClip dieClip;
}
