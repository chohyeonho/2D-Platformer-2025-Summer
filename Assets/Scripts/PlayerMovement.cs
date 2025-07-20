using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 5f;
	public float jumpForce = 12f;
	public LayerMask groundLayer;
	public Transform groundCheck;
	public float groundCheckRadius = 0.1f;

	private Rigidbody2D rb;
	private bool isGrounded;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		Move();
		if (Input.GetButtonDown("Jump") && IsGrounded())
		{
			Jump();
		}
	}

	void Move()
	{
		float moveInput = Input.GetAxisRaw("Horizontal");
		Vector2 newVelocity = rb.linearVelocity;
		newVelocity.x = moveInput * moveSpeed;
		rb.linearVelocity = newVelocity;

		// ▶︎좌우 반전 (스프라이트 기준)
		if (moveInput != 0)
		{
			transform.localScale = new Vector3(Mathf.Sign(moveInput), 1f, 1f);
		}
	}

	void Jump()
	{
		Vector2 newVelocity = rb.linearVelocity;
		newVelocity.y = jumpForce;
		rb.linearVelocity = newVelocity;
	}

	bool IsGrounded()
	{
		return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
	}
}
