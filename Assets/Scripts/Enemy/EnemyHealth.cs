using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
	[SerializeField] private float maxHealth = 3f;
	private float currentHealth;

	private void Start()
	{
		currentHealth = maxHealth;
	}

	// ※ 제안: 체력 깎는 데미지 처리용 함수
	// public void Damage(float amount)
	// {
	//     currentHealth -= amount;
	//     if (currentHealth <= 0f)
	//     {
	//         Die();
	//     }
	// }

	// ※ 제안: 적 사망 시 호출할 함수
	// private void Die()
	// {
	//     gameObject.SetActive(false);
	// }

	// ※ 제안: 인터페이스 도입 시 아래처럼 작성
	// public bool hasTakenDamage { get; set; }
	// public void Damage(float amount)
	// {
	//     if (hasTakenDamage) return;
	//     hasTakenDamage = true;
	//     currentHealth -= amount;
	//     if (currentHealth <= 0f) Die();
	// }
}
