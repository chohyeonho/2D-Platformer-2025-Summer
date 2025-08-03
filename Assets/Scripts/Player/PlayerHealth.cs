using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

	// ● 무적 시간
	[SerializeField] private float invincibleTime = 3f;

	// ● 무적 여부
	private bool isInvincible = false;

	// ● 컴포넌트 캐싱
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

	// ▶︎ 데미지 입기 처리
	public void TakeDamage(Vector2 attackerPos)
	{
		Debug.Log("피해 시도됨");

		if (isInvincible || PlayerData.instance.currentHealth <= 0) return;

		PlayerData.instance.SetHealth(PlayerData.instance.currentHealth - 1);
		UIManager.instance.UpdateHealth(PlayerData.instance.currentHealth);

		if (PlayerData.instance.currentHealth <= 0)
		{
			Die();
			return;
		}



		isInvincible = true;
		gameObject.layer = 11;
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		// ★ 반동 처리: 공격자 위치와 다를 때만 적용
		if (attackerPos != (Vector2)transform.position)
		{
			int dirc = transform.position.x - attackerPos.x > 0 ? 1 : -1;
			rigid.AddForce(new Vector2(dirc, 1) * 7f, ForceMode2D.Impulse);
		}

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
	public void Die()
	{
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);
		spriteRenderer.flipY = true;
		capsuleCollider.enabled = false;
		rigid.AddForceY(5f, ForceMode2D.Impulse);
		sound?.PlayDie();
		UIManager.instance.ShowRestartButton("Retry");
	}

	// ▶︎ 체력 초기화
	public void ResetHealth()
	{
		PlayerData.instance.ResetHealth();
		UIManager.instance.UpdateHealth(PlayerData.instance.currentHealth);

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
