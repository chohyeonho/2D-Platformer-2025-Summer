using System;
using UnityEngine;

public static class PlayerEvents
{

	// ▶︎ 스코어 변경 이벤트: 발신자와 총합 점수 전달
	public static Action<object, int> OnScoreChanged;

	public static Action<object> OnPlayerDied;
	public static Action<object, int> OnHealthChanged;
	public static Action<object, GameObject> OnHitEnemy;
	public static Action<object> OnSwingStarted;
	public static Action<object> OnItemCollected;
	public static Action<object> OnJumped;
	public static Action<object> OnFinished;
	public static Action<object> OnPlayerDamaged;

}
