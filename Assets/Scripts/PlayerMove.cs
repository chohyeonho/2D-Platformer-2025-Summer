using UnityEngine;

public class PlayerMove : MonoBehaviour
{
	public float maxSpeed;                  // �ְ� �ӵ�
	Rigidbody2D rigid;                      // Rigidbody2D ����
	SpriteRenderer spriteRenderer;          // ��������Ʈ ������ ����

	void Awake()
	{
		// ������Ʈ ����
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		// Ű�� ������ �� ��������Ʈ ���� ��ȯ
		if (Input.GetButtonDown("Horizontal"))
		{
			// ����(-1)�̸� �¿� ���� true, ������(1)�̸� false
			spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
		}

		// Ű���� ���� ���� �� ���� ó��
		if (Input.GetButtonUp("Horizontal"))
		{
			rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
		}
	}

	void FixedUpdate()
	{
		// �̵� Ű �Է�
		float h = Input.GetAxisRaw("Horizontal");

		// ���� �� ���ϱ� (������ ��)
		rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

		// ������ �ְ� �ӵ� ����
		if (rigid.linearVelocity.x > maxSpeed)
		{
			rigid.linearVelocity = new Vector2(maxSpeed, rigid.linearVelocity.y);
		}
		// ���� �ְ� �ӵ� ����
		else if (rigid.linearVelocity.x < -maxSpeed)
		{
			rigid.linearVelocity = new Vector2(-maxSpeed, rigid.linearVelocity.y);
		}
	}
}
