using UnityEngine;

// ▶︎ 적의 체력을 관리하는 스크립트
public class EnemyHealth : MonoBehaviour, IDamageable
{
	// ★ 최대 체력 (기본값 3)
	[SerializeField] private float maxHealth = 3f;

	// ★ 현재 체력
	private float currentHealth;

	public bool HasTakenDamage { get; set; }

	// ▶︎ 시작 시 초기 체력 설정
	private void Start()
	{
		currentHealth = maxHealth;
	}

	// ▶︎ 데미지를 받을 때 호출되는 함수 (인터페이스 구현)
	public void Damage(float damageAmount)
	{
		HasTakenDamage = true;

		currentHealth -= damageAmount;

		// ★ 체력이 0 이하로 떨어졌을 때 사망 처리
		if (currentHealth <= 0f)
		{
			Die();
		}
	}

	// ▶︎ 적 사망 시 처리
	private void Die()
	{
		gameObject.SetActive(false);

		// ※ 제안: 사망 이펙트 재생 또는 점수 처리 추가 가능
		// Instantiate(deathEffect, transform.position, Quaternion.identity);
		// GameManager.instance.stagePoint += 100;
	}
}