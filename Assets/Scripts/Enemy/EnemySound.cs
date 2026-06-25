using System;
using UnityEngine;

// ▶︎ 적 사운드를 담당하는 스크립트
public class EnemySound : MonoBehaviour
{
	// ▶︎ 적 설정값 참조 (ScriptableObject)
	[SerializeField] private EnemyConfig enemyConfig;

	// ● 사운드 출력용 오디오 소스
	private AudioSource audioSource;

	// ▶︎ 컴포넌트 초기화
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		EnemyEvents.OnEnemyDamaged += HandleDamaged;
		EnemyEvents.OnEnemyDied += HandleDied;
		EnemyEvents.OnEnemyStomped += HandleStomped;
	}

	private void OnDisable()
	{
		EnemyEvents.OnEnemyDamaged -= HandleDamaged;
		EnemyEvents.OnEnemyDied -= HandleDied;
		EnemyEvents.OnEnemyStomped -= HandleStomped;
	}

	private void HandleDamaged(GameObject enemyObject)
	{
		if (enemyObject == gameObject)
		{
			Play(enemyConfig.hitClip);
		}
	}

	private void HandleDied(GameObject enemyObject)
	{
		if (enemyObject == gameObject)
		{
			Play(enemyConfig.dieClip);
		}
	}

	private void HandleStomped(GameObject enemyObject)
	{
		if (enemyObject == gameObject)
		{
			Play(enemyConfig.stompClip);
		}
	}

	// ▶︎ 사운드 재생 함수
	private void Play(AudioClip clip)
	{
		if (clip != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}
}
