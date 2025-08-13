using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	// ● 점수 및 스테이지 텍스트
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI stageText;

	// ● 체력 하트 이미지들
	[SerializeField] private Image[] heartImages;

	// ● 재시작 버튼 (게임 오버 or 클리어 시 노출)
	[SerializeField] private GameObject restartButton;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
	}

	private void OnEnable()
	{
		PlayerEvents.OnHealthChanged += HandleHealthChanged;
		PlayerEvents.OnPlayerDied += HandlePlayerDied;
		PlayerEvents.OnScoreChanged += HandleScoreChanged;
	}

	private void OnDisable()
	{
		PlayerEvents.OnHealthChanged -= HandleHealthChanged;
		PlayerEvents.OnPlayerDied -= HandlePlayerDied;
		PlayerEvents.OnScoreChanged -= HandleScoreChanged;
	}

	private void Start()
	{
		UpdateStage(); // ★ 여기서 간결하게 호출

		Button btn = restartButton.GetComponent<Button>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => GameManager.instance.Restart());
	}

	// ▶︎ 이벤트 수신: 체력 변경 시
	private void HandleHealthChanged(object sender, int currentHealth)
	{
		UpdateHealth(currentHealth);
	}

	// ▶︎ 이벤트 수신: 플레이어 사망 시
	private void HandlePlayerDied(object sender)
	{
		ShowRestartButton("Retry");
	}

	// ▶︎ 이벤트 수신: 점수 변경 시
	private void HandleScoreChanged(object sender, int totalScore)
	{
		UpdateScore(totalScore);
	}

	// ▶︎ 점수 텍스트 갱신
	public void UpdateScore(int totalScore)
	{
		scoreText.text = totalScore.ToString();
	}

	// ▶︎ 스테이지 번호 갱신
	public void UpdateStage()
	{
		string sceneName = SceneManager.GetActiveScene().name;

		if (sceneName.StartsWith("Stage_"))
		{
			string indexStr = sceneName.Substring("Stage_".Length);
			if (int.TryParse(indexStr, out int index))
			{
				stageText.text = "STAGE " + (index + 1); // 사람 기준 1부터
			}
		}
	}

	// ▶︎ 체력 이미지 갱신
	public void UpdateHealth(int currentHealth)
	{
		for (int i = 0; i < heartImages.Length; i++)
		{
			if (i < currentHealth)
			{
				heartImages[i].color = Color.white;
			}
			else
			{
				heartImages[i].color = new Color(0, 0, 1, 0.4f);
			}
		}
	}

	// ▶︎ 재시작 버튼 활성화
	public void ShowRestartButton(string text = "Restart")
	{
		restartButton.SetActive(true);

		TextMeshProUGUI btnText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
		if (btnText != null)
		{
			btnText.text = text;
		}
	}
}
