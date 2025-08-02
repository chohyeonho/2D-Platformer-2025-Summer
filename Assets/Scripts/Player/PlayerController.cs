using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// ※ 게임 매니저 참조
	public GameManager gameManager;

	// ※ 최대 이동 속도
	public float maxSpeed;

	// ※ 점프에 사용하는 힘
	public float jumpPower;

	// ● 물리 계산용 리지드바디
	Rigidbody2D rigid;

	// ● 좌우 반전 및 피격 표현용 스프라이트 렌더러
	SpriteRenderer spriteRenderer;

	// ● 충돌 판정용 캡슐 콜라이더
	CapsuleCollider2D capsuleCollider;

	// ● 애니메이션 제어용 애니메이터
	Animator anim;

	// ● 바닥 체크용 플래그
	bool isGrounded = false;

	// ● 아이템 타입 분류용 enum
	enum ItemType { Bronze, Silver, Gold }

	// ● 수평 입력값
	private float xInput;

	// ● 이전 프레임의 입력값
	private float prevXInput;

	// ● 사운드 재생용 캐싱 변수
	private PlayerSound sound;

	// ● 체력 관리용 컴포넌트 캐싱
	private PlayerHealth health;


	// ▶︎ 필수 컴포넌트 캐싱 처리
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		anim = GetComponent<Animator>();
		sound = GetComponent<PlayerSound>();
		health = GetComponent<PlayerHealth>();
	}


	// ▶︎ 프레임마다 키 입력 처리
	void Update()
	{
		// ✓ 이전 입력 저장
		prevXInput = xInput;

		// ✓ 현재 입력값 갱신
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		// ★ 점프 입력 감지
		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;

			sound?.PlayJump();
		}

		// ★ 감속 처리: 키에서 손을 뗐고, 일정 속도 이상일 때
		if (Mathf.Abs(prevXInput) > 0.01f &&
			Mathf.Abs(xInput) <= 0.01f &&
			Mathf.Abs(rigid.linearVelocity.x) > 0.5f)
		{
			rigid.linearVelocityX = rigid.linearVelocity.normalized.x * 0.5f;
		}

		// ★ 좌우 반전 처리
		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}

		// ★ 걷기 애니메이션 처리
		anim.SetBool("isWalking", Mathf.Abs(rigid.linearVelocity.x) >= 0.3f);
	}


	// ▶︎ 물리 기반 이동 처리
	void FixedUpdate()
	{
		// ★ 수평 이동 처리
		rigid.AddForceX(xInput, ForceMode2D.Impulse);

		// ★ 최고 속도 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocityX = maxSpeed;
		}
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocityX = -maxSpeed;
		}

		// ★ 하강 중일 때 바닥 체크
		if (rigid.linearVelocity.y < 0)
		{
			Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
			Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;

			Debug.DrawRay(leftFoot, Vector2.down * 1f, Color.green);
			Debug.DrawRay(rightFoot, Vector2.down * 1f, Color.green);

			RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
			RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));

			if (leftRay.collider != null || rightRay.collider != null)
			{
				anim.SetBool("isJumping", false);
				isGrounded = true;
			}
			else
			{
				isGrounded = false;
			}
		}
	}


	// ▶︎ 적과 충돌 처리
	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			// ✓ 위에서 밟았을 경우 공격
			if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y + 0.3f)
			{
				OnAttack(collision.transform);
			}
			// ✓ 옆에서 닿으면 피격 처리
			else
			{
				health?.TakeDamage(collision.transform.position);
			}
		}
	}


	// ▶︎ 아이템, 스테이지 오브젝트와 충돌 처리
	void OnTriggerEnter2D(Collider2D collision)
	{
		// ★ 아이템 충돌 처리
		if (collision.gameObject.CompareTag("Item"))
		{
			ItemType type = ItemType.Bronze;
			if (collision.gameObject.name.Contains("Gold")) { type = ItemType.Gold; }
			else if (collision.gameObject.name.Contains("Silver")) { type = ItemType.Silver; }

			switch (type)
			{
				case ItemType.Bronze: gameManager.stagePoint += 50; break;
				case ItemType.Silver: gameManager.stagePoint += 100; break;
				case ItemType.Gold: gameManager.stagePoint += 300; break;
			}

			collision.gameObject.SetActive(false);
			sound?.PlayItem();
		}

		// ★ 스테이지 도달 처리
		else if (collision.gameObject.CompareTag("Finish"))
		{
			gameManager.NextStage();
			sound?.PlayFinish();
		}
	}


	// ▶︎ 적 위에서 밟았을 때의 처리
	void OnAttack(Transform enemy)
	{
		// ✓ 점수 증가
		gameManager.stagePoint += 100;

		// ✓ 반동 점프
		rigid.AddForceY(10, ForceMode2D.Impulse);

		// ★ 적에게 데미지 전달
		IDamageable damageable = enemy.GetComponent<IDamageable>();
		if (damageable != null)
		{
			damageable.Damage(1f);
		}

		// ★ 밟는 사운드 (적 측 재생)
		enemy.GetComponent<EnemySound>()?.PlayStompSound();
	}


	// ▶︎ 속도 초기화 처리
	public void VelocityZero()
	{
		rigid.linearVelocity = Vector2.zero;
	}
}
