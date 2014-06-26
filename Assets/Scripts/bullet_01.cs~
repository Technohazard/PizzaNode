using UnityEngine;
using System.Collections;

// Bullet_01 - nonsprite based

public class bullet_01 : MonoBehaviour {


	private Vector2 velocity;


	// Animation Variables
	private Animator my_anim;

	public float start_time = 0.0F;
	public float desired_time;

	public int damage = 1;
	public float life_span = 20f; // Destroy this object after X seconds
	public bool EnemyShot = false; // false=belongs to player. true=belongs to enemy

	
	// Use this for initialization
	void Start () {
		my_anim = this.transform.GetComponent<Animator>();
		desired_time = start_time;

		my_anim.StopPlayback();

		velocity = rigidbody2D.velocity;

		// 2 - Limited time to live to avoid any leak
		Destroy(gameObject, life_span); // default 20sec
	}
	
	// Update is called once per frame
	void Update () {

		velocity = rigidbody2D.velocity;

		#region Choose Bullet Direction Sprite
		// Choose a sprite frame based on direction of bullet velocity component.
		if (velocity.x < 0)
		{
			// Left
			if(velocity.y < 0)
			{
				desired_time = 6; // LD
			}
			else if (velocity.y == 0)
			{
				desired_time = 3; // LL
			}
			else if (velocity.y > 0)
			{
				desired_time = 0; // LU
			}
		}
		else if (velocity.x > 0)
		{
			// Right
			if(velocity.y < 0)
			{
				desired_time = 8; // RD
			}
			else if (velocity.y == 0)
			{
				desired_time = 5; // RR
			}
			else if (velocity.y > 0)
			{
				desired_time = 5; // RU
			}
		}
		else if (velocity.x == 0)
		{
			// Vertical
			if(velocity.y < 0)
			{
				desired_time = 7; // DD
			}
			else if (velocity.y == 0)
			{
				desired_time = 4; // Not Moving
			}
			else if (velocity.y > 0)
			{
				desired_time = 1; // UU
			}
		}
		#endregion

		//my_anim.playbackTime = desired_time; //set your frame to where you want (float)
		//my_anim.StartPlayback();
	}



}
