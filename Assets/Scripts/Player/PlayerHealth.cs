using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	// ▶︎ 플레이어 설정값 참조
	[SerializeField] private PlayerConfig config;

	// ● 무적 여부
	private bool isInvincible = false;

	// ● 컴포넌트 캐싱
	private SpriteRenderer spriteRenderer;
	private CapsuleCollider2D capsuleCollider;
	private Rigidbody2D rigid;
	private Animator anim;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	// ▶︎ 데미지 입기 처리
	public void TakeDamage(Vector2 attackerPos)
	{
		Debug.Log("피해 시도됨");

		if (isInvincible || PlayerData.instance.currentHealth <= 0) return;

		PlayerData.instance.SetHealth(PlayerData.instance.currentHealth - 1);

		// ● 체력 변경 이벤트 발생
		PlayerEvents.OnHealthChanged?.Invoke(this, PlayerData.instance.currentHealth);

		if (PlayerData.instance.currentHealth <= 0)
		{
			// ● 사망 이벤트 발생
			PlayerEvents.OnPlayerDied?.Invoke(this);
			return;
		}

		isInvincible = true;
		gameObject.layer = 11;
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		// ★ 반동 처리: 공격자 위치와 다를 때만 적용
		if (attackerPos != (Vector2)transform.position)
		{
			int dirc = transform.position.x - attackerPos.x > 0 ? 1 : -1;
			rigid.AddForce(new Vector2(dirc, 1) * config.damagedKnockbackForce, ForceMode2D.Impulse);
		}

		anim.SetTrigger("doDamaged");

		// ● 사운드 이벤트 호출
		PlayerEvents.OnPlayerDamaged?.Invoke(this);

		Invoke(nameof(EndInvincibility), config.invincibleTime);
	}

	// ▶︎ 무적 해제
	private void EndInvincibility()
	{
		isInvincible = false;
		gameObject.layer = 10;
		spriteRenderer.color = Color.white;
	}

	// ▶︎ 사망 처리
	public void Die()
	{
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		capsuleCollider.enabled = false;

		// ▶︎ 사망 직후 위로 튕기는 연출
		rigid.AddForceY(config.deathBounceForce, ForceMode2D.Impulse);

		// ● 사운드 이벤트 호출
		PlayerEvents.OnPlayerDied?.Invoke(this);
	}

	// ▶︎ 체력 초기화
	public void ResetHealth()
	{
		PlayerData.instance.ResetHealth();

		// ● 체력 초기화 시에도 체력 변경 이벤트 호출
		PlayerEvents.OnHealthChanged?.Invoke(this, PlayerData.instance.currentHealth);

		isInvincible = false;
		spriteRenderer.color = Color.white;
		gameObject.layer = 10;
		capsuleCollider.enabled = true;
	}

	// ▶︎ 외부 조회용 체력 접근
	public int GetCurrentHealth()
	{
		return PlayerData.instance.currentHealth;
	}
}
