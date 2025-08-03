using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

	private void Start()
	{
		Button btn = restartButton.GetComponent<Button>();
		btn.onClick.RemoveAllListeners();
		btn.onClick.AddListener(() => GameManager.instance.Restart());

		if (GameManager.isRestart)
		{
			PlayerData.instance?.ResetAll();
			GameManager.isRestart = false;
		}
	}

	// ▶︎ 점수 갱신
	public void UpdateScore(int totalScore)
	{
		scoreText.text = totalScore.ToString();
	}

	// ▶︎ 스테이지 번호 갱신
	public void UpdateStage(int stageIndex)
	{
		stageText.text = "STAGE " + (stageIndex + 1);
	}

	// ▶︎ 체력 이미지 갱신 (삼항 연산자 제거)
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
