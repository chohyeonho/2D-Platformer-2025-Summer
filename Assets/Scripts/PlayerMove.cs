using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	// ※ 최대 이동 속도
	public float maxSpeed;

	// ※ 점프에 사용하는 힘
	public float jumpPower;

	// ● 물리 계산용 리지드바디
	Rigidbody2D rigid;

	// ● 좌우 반전 및 피격 표현용 스프라이트 렌더러
	SpriteRenderer spriteRenderer;

	// ● 애니메이션 제어용 애니메이터
	Animator anim;

	// ★ 필수 컴포넌트 연결
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
	}

	// ▶︎ 키 입력 처리
	void Update()
	{
		// ✓ 점프 입력 시 위로 힘을 가하고 점프 상태 설정
		if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
		{
			rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
		}

		// ✓ 수평 이동 키에서 손을 뗐을 때 속도 감속
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
		}

		// ✓ 이동 방향에 따라 스프라이트 좌우 반전
		if (Input.GetButton("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		// ✓ 이동 속도에 따라 걷기 애니메이션 설정
		if (Mathf.Abs(rigid.linearVelocity.x) < 0.3f)
		{
			anim.SetBool("isWalking", false);
		}
		else
		{
			anim.SetBool("isWalking", true);
		}
	}

	// ▶︎ 물리 기반 이동 처리
	void FixedUpdate()
	{
		// ✓ 수평 입력을 이용한 이동 힘 적용
		float h = Input.GetAxisRaw("Horizontal");
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// ✓ 최고 속도를 넘지 않도록 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}

		// ★ 하강 중일 때만 바닥 존재 여부를 확인
		if (rigid.linearVelocity.y < 0)
		{
			// ● 레이를 그려 디버깅
			Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));

			// ● 바닥 확인용 레이캐스트 실행
			RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));

			// ● 레이캐스트가 바닥에 닿았을 경우 착지 상태로 전환
			if (rayHit.collider != null)
			{
				if (rayHit.distance < 0.5f)
				{
					anim.SetBool("isJumping", false);
				}
			}
		}
	}

	// ▶︎ 충돌 발생 시 처리
	void OnCollisionEnter2D(Collision2D collision)
	{
		// ✓ 적과 충돌했을 때 공격 또는 피격 판단
		if (collision.gameObject.tag == "Enemy")
		{
			// ✓ 적보다 높은 위치에서 충돌하면 공격
			if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y)
			{
				OnAttack(collision.transform);
			}
			// ✓ 그 외의 경우에는 피격 처리
			else
			{
				OnDamaged(collision.transform.position);
			}
		}
	}

	// ▶︎ 피격 시 반응 처리
	void OnDamaged(Vector2 targetPos)
	{
		// ★ 무적 상태를 위한 레이어 변경
		gameObject.layer = 11;

		// ✓ 알파값 변경으로 피격 표현
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		// ✓ 적 반대 방향으로 튕겨나가는 힘 적용
		int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
		rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

		// ✓ 피격 애니메이션 실행
		anim.SetTrigger("doDamaged");

		// ★ 일정 시간 후 무적 해제 예약
		Invoke("OffDamaged", 3);
	}

	// ▶︎ 무적 해제 처리
	void OffDamaged()
	{
		gameObject.layer = 10;
		spriteRenderer.color = new Color(1, 1, 1, 1);
	}

	// ▶︎ 적 공격 처리
	void OnAttack(Transform enemy)
	{
		// ● 반동 점프 효과
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

		// ● 적에 데미지를 가함
		EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
		enemyMove.OnDamaged();
	}
}
