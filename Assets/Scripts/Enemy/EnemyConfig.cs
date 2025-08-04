using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Config/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
	[Header("�⺻ ����")]
	public float maxHealth = 3f;
	public float moveSpeed = 1f;

	[Header("AI �ൿ ����")]
	public float minThinkTime = 2f;
	public float maxThinkTime = 5f;

	[Header("��Ȱ��ȭ ������")]
	public float deactivateDelay = 5f;

	[Header("���� Ŭ��")]
	public AudioClip stompClip;
	public AudioClip dieClip;
}
