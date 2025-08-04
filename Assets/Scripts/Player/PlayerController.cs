using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// ★ 플레이어 설정값 (ScriptableObject)
	[SerializeField] private PlayerConfig config;

	// ● 물리 계산용 리지드바디
	private Rigidbody2D rigid;

	// ● 좌우 반전 및 피격 표현용 스프라이트 렌더러
	private SpriteRenderer spriteRenderer;

	// ● 충돌 판정용 캡슐 콜라이더
	private CapsuleCollider2D capsuleCollider;

	// ● 애니메이션 제어용 애니메이터
	private Animator anim;

	// ● 바닥 체크용 플래그
	private bool isGrounded = false;

	// ● 이동 입력값 캐싱
	private float xInput;
	private float prevXInput;

	// ● 사운드 제어용
	private PlayerSound sound;

	// ● 체력 처리용
	private PlayerHealth health;

	// ● 아이템 타입 분류용 enum
	private enum ItemType { Bronze, Silver, Gold }

	// ▶︎ 필수 컴포넌트 초기화
	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		anim = GetComponent<Animator>();
		sound = GetComponent<PlayerSound>();
		health = GetComponent<PlayerHealth>();
	}

	// ▶︎ 시작 시 초기 설정
	private void Start()
	{
		if (GameManager.instance != null)
		{
			GameManager.instance.player = this;
			GameManager.instance.PlayerReposition();
		}
	}

	// ▶︎ 매 프레임 입력 처리
	private void Update()
	{
		prevXInput = xInput;
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(config.jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;
			sound?.PlayJump();
		}

		if (Mathf.Abs(prevXInput) > 0.01f &&
			Mathf.Abs(xInput) <= 0.01f &&
			Mathf.Abs(rigid.linearVelocity.x) > 0.5f)
		{
			rigid.linearVelocityX = rigid.linearVelocity.normalized.x * config.brakeSpeed;
		}

		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}

		anim.SetBool("isWalking", Mathf.Abs(rigid.linearVelocity.x) >= 0.3f);
	}

	// ▶︎ 물리 기반 이동 처리
	private void FixedUpdate()
	{
		rigid.AddForceX(xInput, ForceMode2D.Impulse);

		if (rigid.linearVelocity.x > config.moveSpeed)
		{
			rigid.linearVelocityX = config.moveSpeed;
		}
		else if (rigid.linearVelocity.x < -config.moveSpeed)
		{
			rigid.linearVelocityX = -config.moveSpeed;
		}

		if (rigid.linearVelocity.y < 0)
		{
			Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
			Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;
			Debug.DrawRay(leftFoot, Vector2.down * 1f, Color.green);
			Debug.DrawRay(rightFoot, Vector2.down * 1f, Color.green);

			RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
			RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));

			isGrounded = (leftRay.collider != null || rightRay.collider != null);
			anim.SetBool("isJumping", !isGrounded);
		}
	}

	// ▶︎ 충돌 감지 처리
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y + 0.3f)
			{
				OnAttack(collision.transform);
			}
			else
			{
				health?.TakeDamage(collision.transform.position);
			}
		}
	}

	// ▶︎ 아이템 및 피니시 충돌 처리
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Item"))
		{
			ItemType type = ItemType.Bronze;
			if (collision.gameObject.name.Contains("Gold")) type = ItemType.Gold;
			else if (collision.gameObject.name.Contains("Silver")) type = ItemType.Silver;

			switch (type)
			{
				case ItemType.Bronze: PlayerData.instance.AddStageScore(50); break;
				case ItemType.Silver: PlayerData.instance.AddStageScore(100); break;
				case ItemType.Gold: PlayerData.instance.AddStageScore(300); break;
			}

			collision.gameObject.SetActive(false);
			sound?.PlayItem();
		}
		else if (collision.gameObject.CompareTag("Finish"))
		{
			GameManager.instance.SetSpawnPosition(new Vector3(0f, 0f, -2f));
			GameManager.instance.NextStage();
			sound?.PlayFinish();
		}
	}

	// ▶︎ 적 공격 처리
	private void OnAttack(Transform enemy)
	{
		PlayerData.instance.AddStageScore(100);
		rigid.AddForceY(config.attackBounceForce, ForceMode2D.Impulse);

		IDamageable damageable = enemy.GetComponent<IDamageable>();
		if (damageable != null)
		{
			damageable.Damage(1f);
		}

		enemy.GetComponent<EnemySound>()?.PlayStompSound();
	}

	// ▶︎ 속도 초기화
	public void VelocityZero()
	{
		rigid.linearVelocity = Vector2.zero;
	}
}
