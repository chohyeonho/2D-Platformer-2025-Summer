using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;
	public int nextMove;

	void Awake()
	{
		// Rigidbody2D 컴포넌트 가져오기
		rigid = GetComponent<Rigidbody2D>();

		// 5초 후 Think() 실행
		Invoke("Think", 5);
	}

	void FixedUpdate()
	{
		// Unity 6 기준 이동 처리
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// 앞쪽 바닥이 있는지 감지할 위치 계산
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

		// 시각적 디버깅용 Ray
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		// 아래 방향으로 Platform 레이어 탐색
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		// 바닥이 없으면 방향 반전 및 Think 재시작
		if (rayHit.collider == null)
		{
			nextMove *= -1;
			CancelInvoke();
			Invoke("Think", 5);
		}
	}

	// 이동 방향을 재설정하는 함수
	void Think()
	{
		// -1, 0, 1 중 무작위로 이동 방향 설정
		nextMove = Random.Range(-1, 2);

		// 다음 Think까지의 시간도 랜덤 (2~5초)
		float nextThinkTime = Random.Range(2f, 5f);
		Invoke("Think", nextThinkTime);
	}
}
