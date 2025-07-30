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

		// ✓ 전방 낭떠러지 감지용 위치 계산
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

		// ● 시각적 디버깅 레이
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		// ✓ 아래 방향 레이캐스트로 플랫폼 감지
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		// ✓ 바닥 없으면 방향 전환
		if (rayHit.collider == null)
		{
			Turn();
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

		// ✓ 다음 판단 예약
		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);
	}

	// ▶︎ 방향 전환 처리
	void Turn()
	{
		nextMove *= -1;
		spriteRenderer.flipX = nextMove != 1;
		CancelInvoke();
		Invoke("Think", 2);
	}

	// ▶︎ 피격 처리
	public void OnDamaged()
	{
		// ✓ 반투명 효과로 피격 표현
		spriteRenderer.color = new Color(1, 1, 1, 0.4f);

		// ✓ 스프라이트 상하 반전
		spriteRenderer.flipY = true;

		// ✓ 콜라이더 비활성화
		capsuleCollider.enabled = false;

		// ✓ 위로 튀어오르는 효과
		rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

		// ★ 삭제 예약
		Invoke("DeActive", 5);
	}

	// ▶︎ 오브젝트 비활성화
	void DeActive()
	{
		gameObject.SetActive(false);
	}
}
