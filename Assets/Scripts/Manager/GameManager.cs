using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// ★ 전체 점수 누적용 변수
	public int totalPoint;

	// ★ 현재 스테이지에서 획득한 점수
	public int stagePoint;

	// ★ 현재 스테이지 인덱스 (예: 0, 1, 2…)
	public int stageIndex;

	// ★ 플레이어 체력
	public int health;

	// ★ 스테이지 배열
	public GameObject[] Stages;

	// ★ 플레이어 객체 참조용
	public PlayerController player;

	// ★ UI 체력 이미지 배열
	public Image[] UIhealth;

	// ★ 점수 UI
	public TextMeshProUGUI UIPoint;

	// ★ 스테이지 UI
	public TextMeshProUGUI UIStage;

	// ★ 재시작 버튼 오브젝트
	public GameObject RestartBtn;

	// ★ GameManager 싱글톤 인스턴스
	public static GameManager instance;

	// ※ 스테이지별 플레이어 시작 위치 변수 (추후 구현 예정)
	public Vector3 spawnPosition;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}



	void Update()
	{
		// ★ 점수 UI 실시간 갱신
		UIPoint.text = (totalPoint + stagePoint).ToString();
	}

	// ▶︎ 다음 스테이지로 전환 처리
	public void NextStage()
	{
		// ★ 스테이지 변경 처리
		if (stageIndex < Stages.Length - 1)
		{
			// ★ 현재 스테이지 비활성화
			Stages[stageIndex].SetActive(false);

			// ★ 스테이지 인덱스 증가
			stageIndex++;

			// ★ 다음 스테이지 활성화
			Stages[stageIndex].SetActive(true);

			// ★ 플레이어 위치 초기화
			PlayerReposition();

			// ★ 스테이지 UI 갱신
			UIStage.text = "STAGE " + (stageIndex + 1);
		}
		else
		{
			// ★ 게임 클리어 처리

			// ● 플레이어 조작 정지
			Time.timeScale = 0;

			// ● 결과 UI 출력
			Debug.Log("게임 클리어!");

			// ● 재시작 버튼 텍스트 변경 처리
			TextMeshProUGUI btnText = RestartBtn.GetComponentInChildren<TextMeshProUGUI>();
			btnText.text = "Clear!";

			// ● 재시작 버튼 UI 표시
			RestartBtn.SetActive(true);
		}

		// ★ 현재 스테이지 점수를 총점에 누적
		totalPoint += stagePoint;

		// ★ 현재 스테이지 점수 초기화
		stagePoint = 0;
	}

	// ▶︎ 체력 감소 처리
	public void HealthDown()
	{
		if (health > 1)
		{
			// ★ 체력 감소
			health--;

			// ★ 해당 체력 하트 이미지 색상 어둡게 처리 (파란색)
			UIhealth[health].color = new Color(0, 0, 1, 0.4f);
		}
		else
		{
			// ★ 모든 하트 UI 제거 처리 (마지막 하트 포함)
			UIhealth[0].color = new Color(0, 0, 1, 0.4f);

			// ★ 플레이어 사망 처리 (널 체크 포함)
			if (player != null)
			{
				player.OnDie();
			}

			// ★ 결과 UI 표시
			// ※ 구현 필요: 결과창 UI 활성화
			Debug.Log("죽었습니다!");

			// ★ 재시작 버튼 UI 표시
			RestartBtn.SetActive(true);
		}
	}

	// ▶︎ 플레이어 충돌 시 체력 조건에 따른 처리
	void OnTriggerEnter2D(Collider2D collision)
	{
		// ★ 플레이어 태그 충돌 감지 (CompareTag 사용)
		if (collision.CompareTag("Player"))
		{
			// ★ 체력이 2 이상일 경우 위치 초기화
			if (health > 1)
			{
				PlayerReposition();
			}

			// ★ 체력 감소
			HealthDown();
		}
	}

	// ▶︎ 플레이어 위치 초기화
	void PlayerReposition()
	{
		// ★ 플레이어 위치 초기화 (현재는 임시 하드코딩 위치)
		// ※ 실제 위치 지정은 추후 구현 필요
		player.transform.position = spawnPosition; 

		// ★ 속도 초기화
		player.VelocityZero();
	}

	// ▶︎ 씬 재시작 처리
	public void Restart()
	{
		// ★ TimeScale 초기화
		Time.timeScale = 1;

		// ★ 현재 씬 다시 로드
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
