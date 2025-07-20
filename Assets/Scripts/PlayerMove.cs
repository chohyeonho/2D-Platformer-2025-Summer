using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;                      // 최고 속도

	Rigidbody2D rigid;                          // 물리 컴포넌트
	SpriteRenderer spriteRenderer;              // 방향 반전용
	Animator anim;                              // 애니메이터

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();                 // Rigidbody2D 연결
		spriteRenderer = GetComponent<SpriteRenderer>();     // SpriteRenderer 연결
		anim = GetComponent<Animator>();                     // Animator 연결
	}

	void Update()
	{
		// ▶︎ 수평 이동 키에서 손을 뗐을 때: 속도 절반 감속
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
		}

		// ▶︎ 수평 이동 키 누를 때: 좌우 방향 반전 (왼쪽이면 true)
		if (Input.GetButtonDown("Horizontal"))
		{
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		// ▶︎ 애니메이션 전환: x축 속도가 정확히 0일 때 멈춤 상태
		if (rigid.linearVelocity.normalized.x == 0)
			anim.SetBool("isWalking", false);
		else
			anim.SetBool("isWalking", true);
	}

	void FixedUpdate()
	{
		// ▶︎ 수평 입력 받기 (-1, 0, 1)
		float h = Input.GetAxisRaw("Horizontal");

		// ▶︎ 수평 방향 힘 가하기 (Impulse: 순간 힘)
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// ▶︎ 오른쪽 최고 속도 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		// ▶︎ 왼쪽 최고 속도 제한
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}
	}
}
