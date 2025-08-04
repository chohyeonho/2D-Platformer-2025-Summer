using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

// ● 입력 흐름 전체를 제어하는 싱글톤 입력 매니저
public class InputManager : MonoBehaviour
{
	// ● 싱글톤 인스턴스 (static이므로 Inspector에는 자동으로 숨겨짐)
	public static InputManager instance;

	// ● 자동 생성된 입력 액션 클래스 (정확한 클래스명 기반 네이밍)
	public GameInputActions gameInputActions { get; private set; }

	// ● 인스턴스 초기화 및 입력 클래스 생성
	private void Awake()
	{
		// ● 이미 인스턴스가 존재할 경우 자신을 파괴
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		// ● 인스턴스 등록
		instance = this;

		// ● 입력 액션 클래스 초기화
		gameInputActions = new GameInputActions();
	}

	// ● 입력 활성화 처리 (씬 전환 후 자동 재활성화)
	private void OnEnable()
	{
		gameInputActions?.Enable();
	}

	// ● 입력 비활성화 처리 (비활성 시 안전하게 입력 제거)
	private void OnDisable()
	{
		gameInputActions?.Disable();
	}
}
