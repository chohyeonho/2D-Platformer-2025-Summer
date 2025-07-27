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
		// ▷ Unity 6 기준 이동 처리
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);

		// ▷ 낭떠러지 감지
		Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
		Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
		RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

		if (rayHit.collider == null)
		{
			// ▷ 방향 반전 및 Think 리셋
			nextMove *= -1;
			CancelInvoke();
			Invoke("Think", 5);
		}
	}

	void Think()
	{
		nextMove = Random.Range(-1, 2);
		Invoke("Think", 5);
	}
}
