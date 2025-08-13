using UnityEngine;

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

	private void OnEnable()
	{
		PlayerEvents.OnSwingStarted += HandleSwing;
		PlayerEvents.OnHitEnemy += HandleHit;
		PlayerEvents.OnHealthChanged += HandleDamaged;
		PlayerEvents.OnPlayerDied += HandleDie;
		PlayerEvents.OnItemCollected += HandleItem;
		PlayerEvents.OnJumped += HandleJump;
		PlayerEvents.OnFinished += HandleFinish;
	}

	private void OnDisable()
	{
		PlayerEvents.OnSwingStarted -= HandleSwing;
		PlayerEvents.OnHitEnemy -= HandleHit;
		PlayerEvents.OnHealthChanged -= HandleDamaged;
		PlayerEvents.OnPlayerDied -= HandleDie;
		PlayerEvents.OnItemCollected -= HandleItem;
		PlayerEvents.OnJumped -= HandleJump;
		PlayerEvents.OnFinished -= HandleFinish;
	}

	// ▶︎ 공통 재생 함수
	private void Play(AudioClip clip)
	{
		if (clip != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	// ▶︎ 이벤트 수신 핸들러들
	private void HandleSwing(object sender) => Play(config.swingClip);
	private void HandleHit(object sender, GameObject _) => Play(config.hitClip);
	private void HandleDamaged(object sender, int hp) { if (hp > 0) Play(config.damagedClip); }
	private void HandleDie(object sender) => Play(config.dieClip);
	private void HandleItem(object sender) => Play(config.itemClip);
	private void HandleJump(object sender) => Play(config.jumpClip);
	private void HandleFinish(object sender) => Play(config.finishClip);
}
