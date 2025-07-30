using UnityEngine;

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
		// ★ 다음 스테이지로 이동
		stageIndex++;

		// ★ 현재 스테이지 점수를 총점에 누적
		totalPoint += stagePoint;

		// ★ 현재 스테이지 점수 초기화
		stagePoint = 0;
	}

	void Update()
	{

	}
}
