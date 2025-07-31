using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	[SerializeField] private float maxHealth = 3f;
	private float currentHealth;

	private void Start()
	{
		currentHealth = maxHealth;
	}

	// �� ����: ü�� ��� ������ ó���� �Լ�
	// public void Damage(float amount)
	// {
	//     currentHealth -= amount;
	//     if (currentHealth <= 0f)
	//     {
	//         Die();
	//     }
	// }

	// �� ����: �� ��� �� ȣ���� �Լ�
	// private void Die()
	// {
	//     gameObject.SetActive(false);
	// }

	// �� ����: �������̽� ���� �� �Ʒ�ó�� �ۼ�
	// public bool hasTakenDamage { get; set; }
	// public void Damage(float amount)
	// {
	//     if (hasTakenDamage) return;
	//     hasTakenDamage = true;
	//     currentHealth -= amount;
	//     if (currentHealth <= 0f) Die();
	// }
}
