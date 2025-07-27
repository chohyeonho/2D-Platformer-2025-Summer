using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;
	public int nextMove;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		Invoke("Think", 5);
	}

	void FixedUpdate()
	{
		// ▷ 이동 처리 (Unity 6 문법: linearVelocity 사용)
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// ▷ 플랫폼 체크 (앞쪽 바닥이 있는지 Ray로 확인)
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);

		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0)); // 시각적 디버그
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		if (rayHit.collider == null)
		{
			Debug.Log("경고! 이 앞 낭떠러지.");
			// 필요 시 방향 전환 로직 추가 가능
		}
	}

	void Think()
	{
		nextMove = Random.Range(-1, 2);
		Invoke("Think", 5);
	}
}
