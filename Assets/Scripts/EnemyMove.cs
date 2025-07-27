using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;             // Rigidbody2D 컴포넌트 참조
	public int nextMove;           // 이동 방향 (-1, 0, 1)

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();  // Rigidbody2D 가져오기
		Invoke("Think", 5);                   // 5초 후 Think() 호출
	}

	void FixedUpdate()
	{
		// linearVelocity 사용 (Unity 6 기준)
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
	}

	void Think()
	{
		nextMove = Random.Range(-1, 2);       // 이동 방향 결정
		Invoke("Think", 5);                   // 5초마다 재귀 호출
	}
}
