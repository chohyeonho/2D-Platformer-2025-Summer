using UnityEngine;

// ▶︎ 피해를 받을 수 있는 객체가 구현해야 하는 인터페이스
public interface IDamageable
{
	// 공격 타입 구분을 위한 매개변수 추가
	void Damaged(float amount, string attackType = "");

	// 공격 타입별 데미지 플래그 초기화 메서드들
	void ResetStompDamageFlag();
	void ResetHitDamageFlag();
}
