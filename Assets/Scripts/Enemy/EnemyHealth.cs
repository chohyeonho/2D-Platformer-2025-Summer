using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
	[SerializeField] private EnemyConfig enemyConfig;

	private float currentHealth;

	private bool hasTakenStompDamage = false;
	private bool hasTakenHitDamage = false;

	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rigid;
	private CapsuleCollider2D capsuleCollider;
	private Animator anim;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		rigid = GetComponent<Rigidbody2D>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		anim = GetComponent<Animator>();
	}

	private void Start()
	{
		currentHealth = enemyConfig.maxHealth;
	}

	public void Damaged(float amount, string attackType = "")
	{
		if (attackType == "stomp")
		{
			if (hasTakenStompDamage) return;
			hasTakenStompDamage = true;
		}
		else
		{
			if (hasTakenHitDamage) return;
			hasTakenHitDamage = true;
		}

		currentHealth -= amount;
		anim.SetTrigger("doHit");

		// ● 이벤트 호출: 데미지 발생 시 자신의 게임 오브젝트 전달
		EnemyEvents.OnEnemyDamaged?.Invoke(gameObject);

		if (currentHealth <= 0f)
		{
			Die();
		}
	}

	private void Die()
	{
		// ● 이벤트 호출: 사망 시 자신의 게임 오브젝트 전달
		EnemyEvents.OnEnemyDied?.Invoke(gameObject);

		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		capsuleCollider.enabled = false;

		rigid.AddForceY(enemyConfig.deathBounceForce, ForceMode2D.Impulse);

		// GetComponent<EnemySound>()?.PlayDieSound(); // 이벤트 시스템으로 대체

		Invoke(nameof(Deactivate), enemyConfig.deactivateDelay);
	}

	private void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void ResetStompDamageFlag()
	{
		hasTakenStompDamage = false;
	}

	public void ResetHitDamageFlag()
	{
		hasTakenHitDamage = false;
	}
}
