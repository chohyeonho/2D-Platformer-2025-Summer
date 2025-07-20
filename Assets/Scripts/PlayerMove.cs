using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;           // 최고 속도
	Rigidbody2D rigid;               // Rigidbody2D 컴포넌트 참조

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>(); // Rigidbody2D 연결
	}

	void FixedUpdate()
	{
		// 키 입력에 따라 좌우 힘을 가함
		float h = Input.GetAxisRaw("Horizontal");
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// 속도 제한 (오른쪽)
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		// 속도 제한 (왼쪽)
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}
	}
}
