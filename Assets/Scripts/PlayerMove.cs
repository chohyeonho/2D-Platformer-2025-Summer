using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;           // �ְ� �ӵ�
	Rigidbody2D rigid;               // Rigidbody2D ������Ʈ ����

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>(); // Rigidbody2D ����
	}

	void FixedUpdate()
	{
		// Ű �Է¿� ���� �¿� ���� ����
		float h = Input.GetAxisRaw("Horizontal");
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// �ӵ� ���� (������)
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		// �ӵ� ���� (����)
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}
	}
}
