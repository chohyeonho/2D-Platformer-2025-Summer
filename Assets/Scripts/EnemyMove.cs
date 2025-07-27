using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;
	Animator anim;
	public int nextMove;

	void Awake()
	{
		// 컴포넌트 연결
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		// 초기 지연 후 행동 시작
		Invoke("Think", 5);
	}

	void FixedUpdate()
	{
		// 이동 처리 (Unity 6 기준)
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// 낭떠러지 감지용 Ray 시작 위치 계산
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

		// 시각적 디버깅 Ray
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		// Raycast로 플랫폼 유무 확인
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		// 낭떠러지면 방향 반전 + Think 재실행
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
		// 이동 방향 설정 (-1, 0, 1 중 랜덤)
		nextMove = Random.Range(-1, 2);

		// 애니메이터에 이동 방향 전달
		anim.SetInteger("WalkSpeed", nextMove);

		// 다음 행동까지 시간 설정 (2~5초 사이)
		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);
	}
}
