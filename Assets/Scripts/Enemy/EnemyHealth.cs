using UnityEngine;

// ▶︎ 적의 체력을 관리하는 스크립트
public class EnemyHealth : MonoBehaviour, IDamageable
{
	// ▶︎ 적 설정값 참조 (ScriptableObject)
	[SerializeField] private EnemyConfig enemyConfig;

	// ▶︎ 현재 체력
	private float currentHealth;

	// ▶︎ 피격 중복 방지 플래그
	public bool HasTakenDamage { get; set; }

	// ▶︎ 컴포넌트 캐싱
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rigid;
	private CapsuleCollider2D capsuleCollider;
	private Animator anim;

	// ▶︎ 컴포넌트 초기화
	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigid = GetComponent<Rigidbody2D>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		anim = GetComponent<Animator>();
	}

	// ▶︎ 시작 시 체력 초기화
	private void Start()
	{
		currentHealth = enemyConfig.maxHealth;
	}

	// ▶︎ 데미지 처리
	public void Damage(float damageAmount)
	{
		if (HasTakenDamage)
		{
			return;
		}
		HasTakenDamage = true;
		currentHealth -= damageAmount;

		anim.SetTrigger("doHit");

		if (currentHealth <= 0f)
		{
			Die();
		}
	}

	// ▶︎ 사망 처리
	private void Die()
	{
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		capsuleCollider.enabled = false;

		rigid.AddForceY(enemyConfig.deathBounceForce, ForceMode2D.Impulse);

		GetComponent<EnemySound>()?.PlayDieSound();

		Invoke(nameof(Deactivate), enemyConfig.deactivateDelay);
	}

	// ▶︎ 비활성화
	private void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
