using UnityEngine;

public class EnemyController : MonoBehaviour
{
	// ● 이동 제어용 리지드바디
	Rigidbody2D rigid;

	// ● 애니메이션 제어용
	Animator anim;

	// ● 시각적 표현용 렌더러
	SpriteRenderer spriteRenderer;

	// ● 충돌 판정용 캡슐 콜라이더
	CapsuleCollider2D capsuleCollider;

	// ● 이동 방향 (-1: 왼쪽, 0: 정지, 1: 오른쪽)
	public int nextMove;

	// ● 비활성화까지 대기 시간
	public float deactivateDelay = 5f;

	// ● 방향 전환 중인지 여부
	bool isTurning = false;

	// ● 적 설정값 참조 (ScriptableObject)
	[SerializeField] private EnemyConfig enemyConfig;

	// ▶︎ 컴포넌트 초기화
	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();

		// ▶︎ 초기 AI 판단 예약 (최소 대기 시간 사용)
		Invoke("Think", enemyConfig.thinkIntervalRange.x);
	}

	// ▶︎ 물리 기반 이동 처리
	private void FixedUpdate()
	{
		// ▶︎ 이동 속도 및 방향 적용
		rigid.linearVelocityX = enemyConfig.moveSpeed * nextMove;

		// ▶︎ 양발 위치 계산 (레이캐스트 기준점)
		Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
		Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;

		// ▶︎ 디버그용 레이 시각화
		Debug.DrawRay(leftFoot, Vector2.down * 1f, Color.green);
		Debug.DrawRay(rightFoot, Vector2.down * 1f, Color.green);

		// ▶︎ 양발 기준 바닥 체크용 레이캐스트 실행
		RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 0.6f, LayerMask.GetMask("Platform"));
		RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 0.6f, LayerMask.GetMask("Platform"));

		// ▶︎ 한쪽 발이라도 공중이면 방향 전환 (방향 전환 중 중복 방지)
		if (!isTurning && (leftRay.collider == null || rightRay.collider == null))
		{
			Turn();
			isTurning = true;
		}
		// ▶︎ 양발 모두 바닥에 닿으면 방향 전환 가능 상태 해제
		else if (leftRay.collider != null && rightRay.collider != null)
		{
			isTurning = false;
		}
	}

	// ▶︎ 무작위 방향 결정 및 행동 예약
	private void Think()
	{
		// ▶︎ -1, 0, 1 중 무작위 이동 방향 선택
		nextMove = Random.Range(-1, 2);

		// ▶︎ 애니메이터에 이동 속도 값 전달
		anim.SetInteger("WalkSpeed", nextMove);

		// ▶︎ 이동 방향에 따른 스프라이트 좌우 반전 처리
		if (nextMove != 0)
		{
			spriteRenderer.flipX = nextMove != 1;
		}

		// ▶︎ 다음 판단 예약 (랜덤 대기 시간 범위 사용)
		float wait = Random.Range(enemyConfig.thinkIntervalRange.x, enemyConfig.thinkIntervalRange.y);
		Invoke("Think", wait);
	}

	// ▶︎ 방향 전환 처리
	private void Turn()
	{
		// ▶︎ 이동 방향 반전
		nextMove *= -1;

		// ▶︎ 스프라이트 반전 처리
		spriteRenderer.flipX = nextMove != 1;

		// ▶︎ 기존 Think 예약 취소
		CancelInvoke("Think");

		// ▶︎ 새로운 Think 예약 (최소 대기 시간)
		Invoke("Think", enemyConfig.thinkIntervalRange.x);
	}
}
