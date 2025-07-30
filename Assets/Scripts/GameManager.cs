using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
	public PlayerMove player;

	// ★ UI 체력 이미지 배열
	public Image[] UIhealth;

	// ★ 점수 UI
	public TextMeshPro UIPoint;

	// ★ 스테이지 UI
	public TextMeshPro UIStage;

	// ※ 필요 시 다른 클래스에서 GameManager를 쉽게 참조할 수 있도록 싱글톤화 고려
	// public static GameManager instance;

	void Start()
	{
		// ※ 위 싱글톤이 필요할 경우 아래 코드 포함 권장
		// instance = this;
	}

	// ▶︎ 다음 스테이지로 전환 처리
	public void NextStage()
	{
		// ★ 스테이지 변경 처리
		if (stageIndex < Stages.Length - 1)
		{
			// 현재 스테이지 비활성화
			Stages[stageIndex].SetActive(false);

			// 스테이지 인덱스 증가
			stageIndex++;

			// 다음 스테이지 활성화
			Stages[stageIndex].SetActive(true);

			// 플레이어 위치 초기화
			PlayerReposition();
		}
		else
		{
			// ★ 게임 클리어 처리

			// ● 플레이어 조작 정지
			Time.timeScale = 0;

			// ● 결과 UI 출력
			Debug.Log("게임 클리어!");

			// ● 재시작 버튼 UI 출력 예정
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
		}
		else
		{
			// ★ 플레이어 사망 처리
			player.OnDie();

			// ※ 제안: player가 null일 경우 NullReferenceException 발생 가능 → 아래 예외처리 고려 가능
			// if (player != null) player.OnDie();

			// ★ 결과 UI 표시
			// ※ 구현 필요: 결과창 UI 활성화
			Debug.Log("죽었습니다!");

			// ★ 재시도 버튼 UI 표시
			// ※ 구현 필요: 버튼 UI 활성화
		}
	}

	// ▶︎ 플레이어 충돌 시 체력 조건에 따른 처리
	void OnTriggerEnter2D(Collider2D collision)
	{
		// ※ 태그 비교는 CompareTag() 사용 권장 → 성능 미세 향상 + null 대응 안정성 증가
		if (collision.gameObject.tag == "Player")
		{
			// ★ 플레이어 위치 초기화 조건 확인
			// ※ 체력이 2 이상일 경우에만 위치와 속도 초기화
			if (health > 1)
			{
				PlayerReposition();
			}

			// ★ 체력 감소
			HealthDown();
		}
	}

	void Update()
	{

	}

	// ▶︎ 플레이어 위치 초기화
	void PlayerReposition()
	{
		// ★ 플레이어 위치 초기화
		player.transform.position = new Vector3(0, 0, -1);

		// ★ 속도 초기화
		player.VelocityZero();
	}
}
