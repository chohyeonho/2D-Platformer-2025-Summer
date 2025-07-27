using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;            // Rigidbody2D 컴포넌트를 담을 변수
	public int nextMove;          // 이동 방향 (-1: 왼쪽, 0: 정지, 1: 오른쪽)

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();  // Rigidbody2D 컴포넌트 가져오기
		Think();                              // 초기 이동 방향 설정
	}

	void FixedUpdate()
	{
		// 현재 방향대로 수평 이동. 수직 속도는 유지.
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
	}

	void Think()
	{
		// -1, 0, 1 중 하나를 랜덤으로 선택해 이동 방향 설정
		nextMove = Random.Range(-1, 2); // 두 번째 인자는 미포함이므로 2까지 아님
	}
}
