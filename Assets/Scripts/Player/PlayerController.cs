using UnityEngine;

public enum PlayerState
{
	Idle,
	Move,
	Jump,
	Fall
}

public class PlayerController : MonoBehaviour
{
	[SerializeField] private GameSettings gameSettings;
	[SerializeField] private PlayerConfig config;

	private Rigidbody2D rigid;
	private SpriteRenderer spriteRenderer;
	private Animator anim;
	private PlayerSound sound;
	private PlayerHealth health;

	private float xInput;
	private float prevXInput;
	private bool isGrounded = false;

	private PlayerState currentState = PlayerState.Idle;
	private float jumpTimer;
	private float maxJumpTime = 0.2f;

	private enum ItemType { Bronze, Silver, Gold }

	// ▶︎ 컴포넌트 캐싱
	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		sound = GetComponent<PlayerSound>();
		health = GetComponent<PlayerHealth>();
	}

	// ▶︎ 시작 시 초기화 및 GameManager 등록
	private void Start()
	{
		if (GameManager.instance != null)
		{
			GameManager.instance.player = this;
			GameManager.instance.PlayerReposition();
		}
	}

	// ▶︎ 매 프레임 입력 처리 및 상태별 업데이트
	private void Update()
	{
		

		// ▶︎ 좌우 입력 갱신
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		switch (currentState)
		{
			case PlayerState.Idle:
				UpdateIdle();
				break;
			case PlayerState.Move:
				UpdateMove();
				break;
			case PlayerState.Jump:
				UpdateJump();
				break;
			case PlayerState.Fall:
				UpdateFall();
				break;
		}
	}

	// ▶︎ 물리 계산 처리, 감속 포함
	private void FixedUpdate()
	{

		// ▶︎ 최대 속도 제한
		if (rigid.linearVelocity.x > config.moveSpeed)
		{
			rigid.linearVelocityX = config.moveSpeed;
		}
		else if (rigid.linearVelocity.x < -config.moveSpeed)
		{
			rigid.linearVelocityX = -config.moveSpeed;
		}

		Debug.Log($"prevXInput={prevXInput}, xInput={xInput}, velocity.x={rigid.linearVelocity.x}");

		// ▶︎ 감속 처리: 좌우 키 뗐을 때 감속 (지상 및 공중 모두 적용)
		if (Mathf.Abs(prevXInput) > 0.01f &&
			Mathf.Abs(xInput) <= 0.01f &&
			Mathf.Abs(rigid.linearVelocity.x) > config.decelerationSpeed)
		{
			// ▶︎ 현재 속도 방향 유지하며 감속 속도 적용
			rigid.linearVelocityX = rigid.linearVelocity.normalized.x * config.decelerationSpeed;
			Debug.Log("감속 적용됨");
		}
		else
		{
			// ▶︎ 좌우 이동력 적용
			rigid.AddForceX(prevXInput, ForceMode2D.Impulse);
		}

		// ▶︎ 바닥 체크
		if (rigid.linearVelocity.y < 0)
		{
			Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
			Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;

			Debug.DrawRay(leftFoot, Vector2.down * 1f, Color.green);
			Debug.DrawRay(rightFoot, Vector2.down * 1f, Color.green);

			RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 0.6f, LayerMask.GetMask("Platform"));
			RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 0.6f, LayerMask.GetMask("Platform"));

			isGrounded = (leftRay.collider != null || rightRay.collider != null);
			anim.SetBool("isJumping", !isGrounded);

			// ▶︎ 착지 시 상태 전환
			if (isGrounded && currentState == PlayerState.Fall)
			{
				ChangeState(PlayerState.Idle);
			}
			// ▶︎ 공중에 있을 때는 Fall 상태 유지
			else if (!isGrounded && currentState != PlayerState.Jump)
			{
				ChangeState(PlayerState.Fall);
			}
		}

		// ▶︎ 이전 입력 저장
		prevXInput = xInput;
	}

	// ▶︎ 상태 전환 처리
	private void ChangeState(PlayerState newState)
	{
		if (currentState == newState) return;

		currentState = newState;

		// ▶︎ 점프 시작 시 타이머 초기화
		if (newState == PlayerState.Jump)
		{
			jumpTimer = 0f;
		}
	}

	// ▶︎ 대기 상태 업데이트
	private void UpdateIdle()
	{
		// ▶︎ 점프 키 눌렀고 지상에 있을 경우 점프 시작
		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(config.jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;
			sound?.PlayJump();
			ChangeState(PlayerState.Jump);
			return;
		}

		// ▶︎ 좌우 입력이 있을 경우 이동 상태로 변경
		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
			ChangeState(PlayerState.Move);
			return;
		}

		anim.SetBool("isWalking", false);
	}

	// ▶︎ 이동 상태 업데이트
	private void UpdateMove()
	{
		// ▶︎ 점프 입력 감지 및 점프 상태 전환
		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(config.jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;
			sound?.PlayJump();
			ChangeState(PlayerState.Jump);
			return;
		}

		// ▶︎ 좌우 입력 방향에 따라 스프라이트 반전 처리
		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}
		else
		{
			// ▶︎ 입력이 없으면 대기 상태로 전환
			ChangeState(PlayerState.Idle);
		}

		anim.SetBool("isWalking", Mathf.Abs(rigid.linearVelocity.x) >= 0.3f);
	}

	// ▶︎ 점프 상태 업데이트
	private void UpdateJump()
	{
		jumpTimer += Time.deltaTime;

		// ▶︎ 좌우 입력에 따른 스프라이트 반전
		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}

		// ▶︎ 점프 중 좌우 이동력 추가 적용
		rigid.AddForceX(xInput, ForceMode2D.Impulse);

		// ▶︎ 최대 이동 속도 제한
		if (rigid.linearVelocity.x > config.moveSpeed)
		{
			rigid.linearVelocityX = config.moveSpeed;
		}
		else if (rigid.linearVelocity.x < -config.moveSpeed)
		{
			rigid.linearVelocityX = -config.moveSpeed;
		}

		// ▶︎ 상승 종료 조건 (속도 음수 혹은 점프 시간 초과 시 낙하 상태로 전환)
		if (rigid.linearVelocity.y <= 0 || jumpTimer > maxJumpTime)
		{
			ChangeState(PlayerState.Fall);
		}
	}

	// ▶︎ 낙하 상태 업데이트
	private void UpdateFall()
	{
		// ▶︎ 좌우 입력에 따른 스프라이트 반전
		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}

		// ▶︎ 낙하 중 좌우 이동력 추가 적용
		rigid.AddForceX(xInput, ForceMode2D.Impulse);

		// ▶︎ 최대 이동 속도 제한
		if (rigid.linearVelocity.x > config.moveSpeed)
		{
			rigid.linearVelocityX = config.moveSpeed;
		}
		else if (rigid.linearVelocity.x < -config.moveSpeed)
		{
			rigid.linearVelocityX = -config.moveSpeed;
		}

		// 착지는 FixedUpdate의 바닥 체크에서 처리됨
	}

	// ▶︎ 적과 충돌 처리
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			// ▶︎ 적을 밟았을 경우
			if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y + 0.3f)
			{
				PerformStompAttack(collision.transform);
			}
			else
			{
				// ▶︎ 적에게 피해 입음
				health?.TakeDamage(collision.transform.position);
			}
		}
	}

	// ▶︎ 아이템 및 피니시 트리거 충돌 처리
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Item"))
		{
			ItemType type = ItemType.Bronze;
			if (collision.gameObject.name.Contains("Gold")) type = ItemType.Gold;
			else if (collision.gameObject.name.Contains("Silver")) type = ItemType.Silver;

			switch (type)
			{
				case ItemType.Bronze:
					PlayerData.instance.AddStageScore(gameSettings.bronzeItemScore);
					break;
				case ItemType.Silver:
					PlayerData.instance.AddStageScore(gameSettings.silverItemScore);
					break;
				case ItemType.Gold:
					PlayerData.instance.AddStageScore(gameSettings.goldItemScore);
					break;
			}

			// ▶︎ 아이템 비활성화 및 효과음 재생
			collision.gameObject.SetActive(false);
			sound?.PlayItem();
		}
		else if (collision.gameObject.CompareTag("Finish"))
		{
			// ▶︎ 다음 스테이지로 이동 준비 및 효과음
			GameManager.instance.SetSpawnPosition(new Vector3(0f, 0f, -2f));
			GameManager.instance.NextStage();
			sound?.PlayFinish();
		}
	}

	// ▶︎ 적 밟기 공격 처리
	private void PerformStompAttack(Transform enemy)
	{
		PlayerData.instance.AddStageScore(100);
		rigid.AddForceY(config.bounceForceOnAttack, ForceMode2D.Impulse);

		IDamageable damageable = enemy.GetComponent<IDamageable>();
		if (damageable != null)
		{
			damageable.Damaged(1f);
			damageable.ResetHitDamageFlag();
		}

		enemy.GetComponent<EnemySound>()?.PlayStompSound();
	}

	// ▶︎ 속도 초기화 함수 (재위치 등에서 사용)
	public void VelocityZero()
	{
		rigid.linearVelocity = Vector2.zero;
	}
}
