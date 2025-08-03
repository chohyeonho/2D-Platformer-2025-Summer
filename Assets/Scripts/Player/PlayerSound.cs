using UnityEngine;

// ▶︎ 플레이어 사운드를 담당하는 스크립트
public class PlayerSound : MonoBehaviour
{
	[SerializeField] private AudioClip swingClip;
	[SerializeField] private AudioClip hitClip;
	[SerializeField] private AudioClip jumpClip;
	[SerializeField] private AudioClip damagedClip;
	[SerializeField] private AudioClip itemClip;
	[SerializeField] private AudioClip dieClip;
	[SerializeField] private AudioClip finishClip;

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

	// ▶︎ 간결 람다 형식 메서드
	public void PlaySwing() => Play(swingClip);
	public void PlayHit() => Play(hitClip);
	public void PlayJump() => Play(jumpClip);
	public void PlayDamaged() => Play(damagedClip);
	public void PlayItem() => Play(itemClip);
	public void PlayDie() => Play(dieClip);
	public void PlayFinish() => Play(finishClip);
}
