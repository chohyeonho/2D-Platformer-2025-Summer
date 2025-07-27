using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;
	Animator anim;
	SpriteRenderer spriteRenderer;
	public int nextMove;

	void Awake()
	{
		// 컴포넌트 연결
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		// 초기 행동 예약
		Invoke("Think", 5);
	}

	void FixedUpdate()
	{
		// 이동 처리 (Unity 6 기준)
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// 낭떠러지 감지용 Ray 위치 설정
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

		// 디버그용 Ray 시각화 (녹색)
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		// 플랫폼 레이어에 바닥이 있는지 검사
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		// 바닥 없으면 방향 전환 및 타이머 리셋
		if (rayHit.collider == null)
		{
			nextMove *= -1;
			CancelInvoke();
			Invoke("Think", 2);
		}
	}

	// 행동 결정 함수
	void Think()
	{
		// 이동 방향 무작위 설정 (-1, 0, 1)
		nextMove = Random.Range(-1, 2);

		// Animator 파라미터 설정 (애니메이션 전환)
		anim.SetInteger("WalkSpeed", nextMove);

		// 이동 방향에 따라 좌우 반전 (오른쪽이면 flipX = true)
		if (nextMove != 0)
		{
			spriteRenderer.flipX = nextMove == 1;
		}

		// 다음 Think 호출 시간 무작위 설정 (2초 ~ 5초)
		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);
	}
}
