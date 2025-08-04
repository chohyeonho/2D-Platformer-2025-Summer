using UnityEngine;

// ▶︎ 플레이어 사운드를 담당하는 스크립트
public class PlayerSound : MonoBehaviour
{
	// ▶︎ 플레이어 설정값 참조
	[SerializeField] private PlayerConfig config;

	// ● 사운드 출력용 오디오 소스
	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	// ▶︎ 공통 재생 함수
	private void Play(AudioClip clip)
	{
		if (clip != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	// ▶︎ 사운드 종류별 재생
	public void PlaySwing() => Play(config.swingClip);
	public void PlayHit() => Play(config.hitClip);
	public void PlayJump() => Play(config.jumpClip);
	public void PlayDamaged() => Play(config.damagedClip);
	public void PlayItem() => Play(config.itemClip);
	public void PlayDie() => Play(config.dieClip);
	public void PlayFinish() => Play(config.finishClip);
}
