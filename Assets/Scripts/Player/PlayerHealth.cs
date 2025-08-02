using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	// ● 최대 체력
	[SerializeField] private int maxHealth = 3;

	// ● 현재 체력
	private int currentHealth;

	// ● 무적 시간 (초)
	[SerializeField] private float invincibleTime = 3f;

	// ● 무적 여부
	private bool isInvincible = false;

	// ● 컴포넌트 참조
	private SpriteRenderer spriteRenderer;
	private CapsuleCollider2D capsuleCollider;
	private Rigidbody2D rigid;
	private Animator anim;
	private PlayerSound sound;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		sound = GetComponent<PlayerSound>();
	}

	private void Start()
	{
		currentHealth = maxHealth;
		UIManager.instance.UpdateHealth(currentHealth);
	}

	// ▶︎ 데미지 입기 요청
	public void TakeDamage(Vector2 attackerPos)
	{
		if (isInvincible || currentHealth <= 0) return;

		currentHealth--;

		UIManager.instance.UpdateHealth(currentHealth);

		if (currentHealth <= 0)
		{
			Die();
			return;
		}

		// ● 피격 연출
		isInvincible = true;
		gameObject.layer = 11;
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		int dirc = transform.position.x - attackerPos.x > 0 ? 1 : -1;
		rigid.AddForce(new Vector2(dirc, 1) * 7f, ForceMode2D.Impulse);

		anim.SetTrigger("doDamaged");
		sound?.PlayDamaged();

		Invoke(nameof(EndInvincibility), invincibleTime);
	}

	// ▶︎ 무적 해제
	private void EndInvincibility()
	{
		isInvincible = false;
		gameObject.layer = 10;
		spriteRenderer.color = Color.white;
	}

	// ▶︎ 사망 처리
	private void Die()
	{
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		capsuleCollider.enabled = false;
		rigid.AddForceY(5f, ForceMode2D.Impulse);
		sound?.PlayDie();

		// ● 게임 오버 UI 표시
		UIManager.instance.ShowRestartButton("Retry");
	}

	// ▶︎ 체력 강제 초기화 (예: 리스폰 시)
	public void ResetHealth()
	{
		currentHealth = maxHealth;
		UIManager.instance.UpdateHealth(currentHealth);
		isInvincible = false;
		spriteRenderer.color = Color.white;
		gameObject.layer = 10;
		capsuleCollider.enabled = true;
	}

	// ▶︎ 현재 체력 외부 접근용
	public int GetCurrentHealth()
	{
		return currentHealth;
	}
}
