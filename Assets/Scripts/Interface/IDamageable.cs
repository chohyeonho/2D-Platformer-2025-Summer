using UnityEngine;

// ▶︎ 피해를 받을 수 있는 객체가 구현해야 하는 인터페이스
public interface IDamageable
{
	// ★ 데미지를 처리하는 함수 (float 타입 데미지 수치 전달)
	public void Damage(float damageAmount);
}
