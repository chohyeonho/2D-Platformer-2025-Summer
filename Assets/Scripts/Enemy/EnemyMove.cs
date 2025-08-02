using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	// ● 이동 제어용 리지드바디
	Rigidbody2D rigid;

	// ● 애니메이션 제어용
	Animator anim;

	// ● 시각적 표현용 렌더러
	SpriteRenderer spriteRenderer;

	// ● 충돌 판정용 캡슐 콜라이더
	CapsuleCollider2D capsuleCollider;

	// ● 이동 방향
	public int nextMove;

	// ● 비활성화까지 대기 시간
	public float deactivateDelay = 5f;

	// ● 방향 전환 중 여부
	bool isTurning = false;

	// ★ 필수 컴포넌트 연결
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();

		// ● 초기 AI 판단 예약
		Invoke("Think", 5);
	}

	// ▶︎ 이동 처리
	void FixedUpdate()
	{
		// ✓ 이동 방향 적용
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// ✓ 양발 위치 기준으로 바닥 존재 여부를 확인

		// ● 현재 위치 기준 양발 위치 설정
		Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
		Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;
		Debug.DrawRay(leftFoot, Vector2.down * 1f, Color.green);
		Debug.DrawRay(rightFoot, Vector2.down * 1f, Color.green);

		// ● 양발 기준 바닥 확인용 레이캐스트 실행
		RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 1f, LayerMask.GetMask("Platform"));
		RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 1f, LayerMask.GetMask("Platform"));

		// ● 한쪽 발이라도 공중이면 방향 전환 (한 번만)
		if (!isTurning && (
			leftRay.collider == null || leftRay.distance > 0.5f ||
			rightRay.collider == null || rightRay.distance > 0.5f))
		{
			Turn();
			isTurning = true;
		}
		// ● 양쪽 발 모두 바닥에 닿으면 방향 전환 가능 상태로 복귀
		else if (
			leftRay.collider != null && leftRay.distance < 0.5f &&
			rightRay.collider != null && rightRay.distance < 0.5f)
		{
			isTurning = false;
		}
	}

	// ▶︎ 방향 결정
	void Think()
	{
		// ✓ -1, 0, 1 중 무작위 선택
		nextMove = Random.Range(-1, 2);

		// ✓ 애니메이터에 값 전달
		anim.SetInteger("WalkSpeed", nextMove);

		// ✓ 방향에 따라 스프라이트 반전
		if (nextMove != 0)
		{
			spriteRenderer.flipX = nextMove != 1;
		}

		// ✓ 다음 판단 예약 (가독성을 위해 변수에 할당)
		float wait = Random.Range(2f, 5f);
		Invoke("Think", wait);
	}

	// ▶︎ 방향 전환 처리
	void Turn()
	{
		// ✓ 방향 반전
		nextMove *= -1;

		// ✓ 스프라이트 반전
		spriteRenderer.flipX = nextMove != 1;

		// ✓ 기존 Think 예약만 취소
		CancelInvoke("Think");

		// ✓ 새로운 Think 예약
		Invoke("Think", 2);
	}
}