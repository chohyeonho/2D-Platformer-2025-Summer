using UnityEngine;

// ▶︎ 적 사운드를 담당하는 스크립트
public class EnemySound : MonoBehaviour
{
	// ★ 사망 시 재생할 소리
	[SerializeField] private AudioClip dieClip;

	// ★ 밟혔을 때 재생할 소리
	[SerializeField] private AudioClip stompClip;

	// ● 사운드 출력용 오디오 소스
	private AudioSource audioSource;

	// ▶︎ 컴포넌트 초기화
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// ▶︎ 공통 사운드 재생 함수
	private void Play(AudioClip clip)
	{
		if (clip != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	// ▶︎ 사망 시 사운드 재생
	public void PlayDieSound() => Play(dieClip);

	// ▶︎ 밟혔을 때 사운드 재생
	public void PlayStompSound() => Play(stompClip);
}
