using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;
	Animator anim;
	SpriteRenderer spriteRenderer;
	public int nextMove;

	void Awake()
	{
		// 필요한 컴포넌트 연결
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		// 초기 행동 예약
		Invoke("Think", 5);
	}

	void FixedUpdate()
	{
		// 이동 처리 (Unity 6 문법: linearVelocity)
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// 낭떠러지 감지용 Ray 시작 지점 설정
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

		// 시각적 디버깅 Ray 표시 (녹색)
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		// 아래 방향으로 Raycast 발사, Platform 레이어에 충돌하는지 확인
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		// 바닥이 없으면 방향 전환
		if (rayHit.collider == null)
		{
			Turn();
		}
	}

	// 행동 결정 함수
	void Think()
	{
		// 이동 방향 무작위 선택 (-1, 0, 1)
		nextMove = Random.Range(-1, 2);

		// 애니메이터에 이동 값 전달
		anim.SetInteger("WalkSpeed", nextMove);

		// 이동 방향에 따라 좌우 반전
		if (nextMove != 0)
		{
			spriteRenderer.flipX = nextMove == 1;
		}

		// 다음 Think 실행 시간 설정 (2초 ~ 5초)
		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);
	}

	// 낭떠러지 또는 벽에 부딪혔을 때 방향을 바꾸는 함수
	void Turn()
	{
		// 방향 반전
		nextMove *= -1;

		// 반전된 방향에 맞춰 Sprite 좌우 반전
		spriteRenderer.flipX = nextMove == 1;

		// 기존 행동 예약 제거 후 다시 예약
		CancelInvoke();
		Invoke("Think", 2);
	}
}
