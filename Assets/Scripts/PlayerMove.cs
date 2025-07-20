using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;                  // 최고 속도
	Rigidbody2D rigid;                      // Rigidbody2D 참조
	SpriteRenderer spriteRenderer;          // 스프라이트 렌더러 참조

	void Awake()
	{
		// 컴포넌트 연결
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		// 키를 눌렀을 때 스프라이트 방향 전환
		if (Input.GetButtonDown("Horizontal"))
		{
			// 왼쪽(-1)이면 좌우 반전 true, 오른쪽(1)이면 false
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		// 키에서 손을 뗐을 때 감속 처리
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
		}
	}

	void FixedUpdate()
	{
		// 이동 키 입력
		float h = Input.GetAxisRaw("Horizontal");

		// 수평 힘 가하기 (순간적 힘)
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// 오른쪽 최고 속도 제한
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		// 왼쪽 최고 속도 제한
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}
	}
}
