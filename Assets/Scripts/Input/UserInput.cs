using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class UserInput : MonoBehaviour
{
	// ★ 싱글톤 인스턴스
	public static UserInput instance;

	// ★ Controls 입력 클래스 (Input Actions 기반)
	[HideInInspector]
	public GameInputActions gameInputActions;

	// ▶︎ 초기화 처리
	private void Awake()
	{
		// ✓ 싱글톤 인스턴스 등록
		if (instance == null)
		{
			instance = this;
		}

		// ✓ Controls 인스턴스 생성
		gameInputActions = new GameInputActions();
	}

	// ▶︎ 오브젝트 활성화 시 입력 활성화
	private void OnEnable()
	{
		gameInputActions.Enable();
	}

	// ▶︎ 오브젝트 비활성화 시 입력 비활성화
	private void OnDisable()
	{
		gameInputActions.Disable();
	}
}
