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

	// ● 무적 지속 시간
	public float invincibleTime = 3f;

	// ● 아이템 타입 분류용 enum
	enum ItemType { Bronze, Silver, Gold }

	private float xInput;
	private float prevXInput;

	// ● 추가: 사운드 캐싱
	private PlayerSound sound;

	// ● 추가: 체력 스크립트 참조
	private PlayerHealth health;


	// ★ 필수 컴포넌트 연결
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		anim = GetComponent<Animator>();

		// ★ 추가: 사운드 캐싱
		sound = GetComponent<PlayerSound>();

		// ★ 추가: 헬스 캐싱
		health = GetComponent<PlayerHealth>();
	}

	private void Start()
	{
		if (GameManager.instance != null)
		{
			GameManager.instance.player = this;
		}
	}


	// ▶︎ 키 입력 처리
	void Update()
	{
		// ✓ 이전 프레임 입력값 저장
		prevXInput = xInput;

		// ✓ 현재 입력값 갱신
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		// ▶︎ 점프 입력 (Input System 기반)
		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;

			// ★ 수정: 캐싱된 사운드 사용
			sound?.PlayJump();
		}

		// ▶︎ 이동 중 키에서 손 뗐을 때 감속 처리
		// ▶︎ 입력을 방금 뗐고,
		// ▶︎ 현재 속도가 일정 이상일 때만 감속
		if (Mathf.Abs(prevXInput) > 0.01f &&
			Mathf.Abs(xInput) <= 0.01f &&
			Mathf.Abs(rigid.linearVelocity.x) > 0.5f)
		{
			rigid.linearVelocityX = rigid.linearVelocity.normalized.x * 0.5f;
		}

		// ▶︎ 좌우 반전 처리
		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}

		// ▶︎ 걷기 애니메이션 처리
		anim.SetBool("isWalking", Mathf.Abs(rigid.linearVelocity.x) >= 0.3f);
	}

	// ▶︎ 물리 기반 이동 처리
	void FixedUpdate()
	{
		// ▶︎ 수평 이동 처리
		rigid.AddForceX(xInput, ForceMode2D.Impulse);

		// ▶︎ 최고 속도 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocityX = maxSpeed;
		}
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocityX = -maxSpeed;
		}

		// ★ 하강 중일 때 양발 기준으로 바닥 존재 여부를 확인
		if (rigid.linearVelocity.y < 0)
		{
			// ● 양발 위치 기준으로 레이를 그려 디버깅
			Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
			Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;
			Debug.DrawRay(leftFoot, Vector2.down * 1f, new Color(0, 1, 0));
			Debug.DrawRay(rightFoot, Vector2.down * 1f, new Color(0, 1, 0));

			// ● 양발 기준 바닥 확인용 레이캐스트 실행
			RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
			RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));

			// ● 둘 중 하나라도 바닥에 닿았을 경우 착지 상태로 전환
			if ((leftRay.collider != null) || (rightRay.collider != null))
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

	// ▶︎ 충돌 발생 시 처리
	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("적과 충돌 감지");

		// ✓ 적과 충돌했을 때 공격 또는 피격 판단
		if (collision.gameObject.CompareTag("Enemy"))
		{
			// ✓ 적보다 높은 위치에서 충돌하면 공격
			if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y + 0.3f)
			{
				OnAttack(collision.transform);
			}
			// ✓ 그 외의 경우에는 피격 처리
			else
			{
				// ★ 수정: 헬스 컴포넌트에 위임
				health?.TakeDamage(collision.transform.position);
			}
		}
	}

	// ▶︎ 아이템과 충돌 처리
	void OnTriggerEnter2D(Collider2D collision)
	{
		// ✓ 아이템 종류 판별 및 점수 처리 (enum 기반)
		if (collision.gameObject.CompareTag("Item"))
		{
			ItemType type = ItemType.Bronze;
			if (collision.gameObject.name.Contains("Gold"))
			{
				type = ItemType.Gold;
			}
			else if (collision.gameObject.name.Contains("Silver"))
			{
				type = ItemType.Silver;
			}

			switch (type)
			{
				case ItemType.Bronze:
					PlayerData.instance.AddStageScore(50);
					break;
				case ItemType.Silver:
					PlayerData.instance.AddStageScore(100);
					break;
				case ItemType.Gold:
					PlayerData.instance.AddStageScore(300);
					break;
			}


			// ✓ 아이템 비활성화
			collision.gameObject.SetActive(false);

			// ★ 아이템 사운드 재생
			sound?.PlayItem();
		}
		else if (collision.gameObject.CompareTag("Finish"))
		{
			// ✓ 다음 스테이지로 이동 처리
			gameManager.NextStage();

			// ★ 클리어 사운드 재생
			sound?.PlayFinish();
		}
	}

	// ● 적 공격 처리
	void OnAttack(Transform enemy)
	{
		// ✓ 점수 증가
		PlayerData.instance.AddStageScore(100);

		// ● 반동 점프 효과
		rigid.AddForceY(10, ForceMode2D.Impulse);

		// ★ 적에게 데미지를 가함
		IDamageable damageable = enemy.GetComponent<IDamageable>();
		if (damageable != null)
		{
			damageable.Damage(1f);
		}

		// ★ 밟혔을 때 사운드 (적 쪽에서 발생)
		enemy.GetComponent<EnemySound>()?.PlayStompSound();
	}

	// ▶︎ 속도 정지 처리
	public void VelocityZero()
	{
		rigid.linearVelocity = Vector2.zero;
	}
}
