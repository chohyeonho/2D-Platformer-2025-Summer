using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;                      // 이동 최고 속도
	public float jumpPower;                     // 점프 힘

	Rigidbody2D rigid;                          // 물리 엔진
	SpriteRenderer spriteRenderer;              // 방향 반전용
	Animator anim;                              // 애니메이터

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();                 // Rigidbody 연결
		spriteRenderer = GetComponent<SpriteRenderer>();     // SpriteRenderer 연결
		anim = GetComponent<Animator>();                     // Animator 연결
	}

	void Update()
	{
		// ▶︎ 점프 입력 (공중 판정 없이 항상 가능)
		if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
		{
			rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
		}

		// ▶︎ 수평 이동 키에서 손을 뗐을 때 감속
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.linearVelocity = new Vector2(
				rigid.linearVelocity.normalized.x * 0.5f,
				rigid.linearVelocity.y
			);
		}

		// ▶︎ 좌우 반전
		if (Input.GetButton("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		// ▶︎ 애니메이션 전환 (걷기 상태 판단)
		if (Mathf.Abs(rigid.linearVelocity.x) < 0.3f)
		{
			anim.SetBool("isWalking", false);
		}
		else
		{
			anim.SetBool("isWalking", true);
		}
	}

	void FixedUpdate()
	{
		// ▶︎ 이동 입력
		float h = Input.GetAxisRaw("Horizontal");
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// ▶︎ 최고 속도 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}

		// ▶︎ 착지 판정
		if (rigid.linearVelocity.y < 0)
		{
			Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));

			RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));

			if (rayHit.collider != null)
			{
				if (rayHit.distance < 0.5f)
				{
					anim.SetBool("isJumping", false);
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		// ▶︎ 적에게 맞았을 때 로그 출력
		if (collision.gameObject.tag == "Enemy")
		{
			Debug.Log("플레이어가 맞았습니다!");
		}
	}
}
