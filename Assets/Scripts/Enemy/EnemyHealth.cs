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
		// ★ 중복 피격 방지 플래그 설정
		HasTakenDamage = true;

		// ★ 체력 감소
		currentHealth -= damageAmount;

		// ★ 피격 애니메이션 트리거 (죽지는 않음)
		GetComponent<Animator>()?.SetTrigger("doHit");

		// ★ 체력이 0 이하로 떨어졌을 때 사망 처리
		if (currentHealth <= 0f)
		{
			Die();
		}
	}


	// ▶︎ 적 사망 시 처리
	private void Die()
	{
		// ✓ 반투명 효과 적용
		GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);

		// ✓ 스프라이트 뒤집기
		GetComponent<SpriteRenderer>().flipY = true;

		// ✓ 콜라이더 제거
		GetComponent<CapsuleCollider2D>().enabled = false;

		// ✓ 튀어오르는 연출
		GetComponent<Rigidbody2D>().AddForceY(5, ForceMode2D.Impulse);

		// ✓ 애니메이션 트리거 실행
		GetComponent<Animator>()?.SetTrigger("doHit");

		// ★ 일정 시간 후 비활성화
		Invoke("DeActive", 5f);
	}

	// ▶︎ 일정 시간 후 비활성화 처리
	private void DeActive()
	{
		gameObject.SetActive(false);
	}
}