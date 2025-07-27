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
		// 이동 처리 (Unity 6 문법: linearVelocity 사용)
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// 앞쪽 발 아래로 Ray를 쏘기 위한 위치 계산
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y);

		// 디버그용 Ray 그리기 (녹색 선)
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

		// 아래 방향으로 Raycast를 쏴서 Platform 레이어에 충돌이 있는지 확인
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		// 충돌한 바닥이 없으면 방향 전환 및 Think 타이머 초기화
		if (rayHit.collider == null)
		{
			nextMove *= -1;
			CancelInvoke();
			Invoke("Think", 5);
		}
	}

	void Think()
	{
		// -1, 0, 1 중 무작위 방향 설정
		nextMove = Random.Range(-1, 2);

		// 5초 후 다시 Think() 실행
		Invoke("Think", 5);
	}
}
