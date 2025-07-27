using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;            // Rigidbody2D ������Ʈ�� ���� ����
	public int nextMove;          // �̵� ���� (-1: ����, 0: ����, 1: ������)

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();  // Rigidbody2D ������Ʈ ��������
		Think();                              // �ʱ� �̵� ���� ����
	}

	void FixedUpdate()
	{
		// ���� ������ ���� �̵�. ���� �ӵ��� ����.
		rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
	}

	void Think()
	{
		// -1, 0, 1 �� �ϳ��� �������� ������ �̵� ���� ����
		nextMove = Random.Range(-1, 2); // �� ��° ���ڴ� �������̹Ƿ� 2���� �ƴ�
	}
}
