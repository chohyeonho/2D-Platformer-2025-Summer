using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
	Rigidbody2D rigid;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		rigid.linearVelocity = new Vector2(-1, rigid.linearVelocity.y);
	}
}
