using UnityEngine;

public enum PlayerState
{
	Idle,
	Move,
	Jump,
	Fall
}

public class PlayerController : MonoBehaviour
{
	[SerializeField] private GameSettings gameSettings;
	[SerializeField] private PlayerConfig config;

	private Rigidbody2D rigid;
	private SpriteRenderer spriteRenderer;
	private Animator anim;
	private PlayerHealth health;

	private float xInput;
	private bool isGrounded = false;

	private PlayerState currentState = PlayerState.Idle;
	private float jumpTimer;
	private float maxJumpTime = 0.2f;

	private enum ItemType { Bronze, Silver, Gold }

	private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();
		health = GetComponent<PlayerHealth>();
	}

	private void Start()
	{
		if (GameManager.instance != null)
		{
			GameManager.instance.player = this;
			GameManager.instance.PlayerReposition();
		}
	}

	private void Update()
	{
		switch (currentState)
		{
			case PlayerState.Idle:
				UpdateIdle(); break;
			case PlayerState.Move:
				UpdateMove(); break;
			case PlayerState.Jump:
				UpdateJump(); break;
			case PlayerState.Fall:
				UpdateFall(); break;
		}
	}

	private void FixedUpdate()
	{
		ApplyHorizontalMovement();
		CheckGrounded();
	}

	private void ChangeState(PlayerState newState)
	{
		if (currentState == newState) return;
		currentState = newState;
		if (newState == PlayerState.Jump)
		{
			jumpTimer = 0f;
		}
	}

	private void UpdateIdle()
	{
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(config.jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;

			PlayerEvents.OnJumped?.Invoke(this);

			ChangeState(PlayerState.Jump);
			return;
		}

		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
			ChangeState(PlayerState.Move);
			return;
		}
		else
		{
			ApplyDeceleration();
		}

		anim.SetBool("isWalking", false);
	}

	private void UpdateMove()
	{
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		if (InputManager.instance.gameInputActions.Player.Jump.WasPressedThisFrame() && isGrounded)
		{
			rigid.AddForceY(config.jumpPower, ForceMode2D.Impulse);
			anim.SetBool("isJumping", true);
			isGrounded = false;

			PlayerEvents.OnJumped?.Invoke(this);

			ChangeState(PlayerState.Jump);
			return;
		}

		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
			ChangeState(PlayerState.Move);
		}
		else
		{
			ApplyDeceleration();
			ChangeState(PlayerState.Idle);
		}

		anim.SetBool("isWalking", Mathf.Abs(rigid.linearVelocity.x) >= 0.3f);
	}

	private void UpdateJump()
	{
		jumpTimer += Time.deltaTime;

		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}

		if (rigid.linearVelocity.y <= 0 || jumpTimer > maxJumpTime)
		{
			ChangeState(PlayerState.Fall);
		}
	}

	private void UpdateFall()
	{
		xInput = InputManager.instance.gameInputActions.Player.Move.ReadValue<Vector2>().x;

		if (Mathf.Abs(xInput) > 0.01f)
		{
			spriteRenderer.flipX = xInput < 0;
		}
	}

	private void ApplyHorizontalMovement()
	{
		rigid.AddForceX(xInput * 2f, ForceMode2D.Impulse);

		if (rigid.linearVelocity.x > config.moveSpeed)
		{
			rigid.linearVelocityX = config.moveSpeed;
		}
		else if (rigid.linearVelocity.x < -config.moveSpeed)
		{
			rigid.linearVelocityX = -config.moveSpeed;
		}
	}

	private void CheckGrounded()
	{
		if (rigid.linearVelocity.y < 0)
		{
			Vector2 leftFoot = rigid.position + Vector2.left * 0.3f;
			Vector2 rightFoot = rigid.position + Vector2.right * 0.3f;
			RaycastHit2D leftRay = Physics2D.Raycast(leftFoot, Vector2.down, 0.6f, LayerMask.GetMask("Platform"));
			RaycastHit2D rightRay = Physics2D.Raycast(rightFoot, Vector2.down, 0.6f, LayerMask.GetMask("Platform"));
			isGrounded = (leftRay.collider != null || rightRay.collider != null);
			anim.SetBool("isJumping", !isGrounded);
			if (isGrounded && currentState == PlayerState.Fall)
			{
				ChangeState(PlayerState.Idle);
			}
			else if (!isGrounded && currentState != PlayerState.Jump)
			{
				ChangeState(PlayerState.Fall);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			if (rigid.linearVelocity.y < 0 && transform.position.y > collision.transform.position.y + 0.3f)
			{
				PerformStompAttack(collision.transform);
			}
			else
			{
				health?.TakeDamage(collision.transform.position);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Item"))
		{
			ItemType type = ItemType.Bronze;
			if (collision.gameObject.name.Contains("Gold")) type = ItemType.Gold;
			else if (collision.gameObject.name.Contains("Silver")) type = ItemType.Silver;

			switch (type)
			{
				case ItemType.Bronze:
					PlayerData.instance.AddStageScore(gameSettings.bronzeItemScore);
					break;
				case ItemType.Silver:
					PlayerData.instance.AddStageScore(gameSettings.silverItemScore);
					break;
				case ItemType.Gold:
					PlayerData.instance.AddStageScore(gameSettings.goldItemScore);
					break;
			}

			collision.gameObject.SetActive(false);

			PlayerEvents.OnItemCollected?.Invoke(this);
		}
		else if (collision.gameObject.CompareTag("Finish"))
		{
			GameManager.instance.SetSpawnPosition(new Vector3(0f, 0f, -2f));
			GameManager.instance.NextStage();

			PlayerEvents.OnFinished?.Invoke(this);
		}
	}

	private void PerformStompAttack(Transform enemy)
	{
		PlayerData.instance.AddStageScore(100);
		rigid.AddForceY(config.bounceForceOnAttack, ForceMode2D.Impulse);

		IDamageable damageable = enemy.GetComponent<IDamageable>();
		if (damageable != null)
		{
			damageable.Damaged(1f, "stomp");
			damageable.ResetStompDamageFlag();
		}

		// enemy.GetComponent<EnemySound>()?.PlayStompSound(); // 이벤트 시스템으로 대체
		EnemyEvents.OnEnemyStomped?.Invoke(enemy.gameObject);
	}

	public void VelocityZero()
	{
		rigid.linearVelocity = Vector2.zero;
	}

	// ▶︎ 감속 처리 함수 (속도 기준만 검사)
	private void ApplyDeceleration()
	{
		if (Mathf.Abs(rigid.linearVelocity.x) > config.decelerationSpeed)
		{
			int dir = spriteRenderer.flipX ? -1 : 1;
			rigid.linearVelocityX = dir * config.decelerationSpeed;
		}
	}
}
