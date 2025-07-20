using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;                      // 최고 이동 속도

	Rigidbody2D rigid;                          // Rigidbody2D 참조
	SpriteRenderer spriteRenderer;              // 스프라이트 반전용
	Animator anim;                              // 애니메이션 컨트롤러

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();                 // Rigidbody 연결
		spriteRenderer = GetComponent<SpriteRenderer>();     // SpriteRenderer 연결
		anim = GetComponent<Animator>();                     // Animator 연결
	}

	void Update()
	{
		// ▶︎ 이동 키에서 손 뗐을 때 감속 처리 (속도 절반)
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.linearVelocity = new Vector2(
				rigid.linearVelocity.normalized.x * 0.5f,
				rigid.linearVelocity.y
			);
		}

		// ▶︎ 방향 반전 처리 (왼쪽 이동 시 flipX true)
		if (Input.GetButton("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		// ▶︎ 애니메이션 상태 전환 (속도가 작으면 정지 상태로 간주)
		if (Mathf.Abs(rigid.linearVelocity.x) < 0.3f)
			anim.SetBool("isWalking", false);
		else
			anim.SetBool("isWalking", true);
	}

	void FixedUpdate()
	{
		// ▶︎ 입력값 받아오기 (-1, 0, 1)
		float h = Input.GetAxisRaw("Horizontal");

		// ▶︎ 수평 힘 적용 (Impulse 방식으로 순간 가속)
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// ▶︎ 오른쪽 이동 속도 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		// ▶︎ 왼쪽 이동 속도 제한
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}
	}
}
