using UnityEngine;
using System.Collections;

// Bullet_01 - nonsprite based

public class bullet_01 : MonoBehaviour {


	private Vector2 velocity;


	// Animation Variables
	private Animator my_anim;

	public float anim_start_time = 0.0F;
	public float anim_desired_time;

	public int damage = 1;
	public float life_span = 20f; // Destroy this object after X seconds
	public bool EnemyShot = false; // false=belongs to player. true=belongs to enemy
	public float explode_length = 0.1f; // number of seconds an exploder-type bullet persists after impact

	public enum bullet_behaviors {normal, anim_facing, exploder};
	public bullet_behaviors bullet_type = bullet_behaviors.normal;

	
	// Use this for initialization
	void Start () {
		my_anim = gameObject.GetComponent<Animator>();
		if (my_anim == null)
		{
			Debug.Log ("Animator not Found!");
		}
		else
		{
			anim_desired_time = anim_start_time;
			my_anim.StopPlayback();
		}

		// my_anim.StartPlayback();

		// 2 - Limited time to live to avoid any leak
		Destroy(gameObject, life_span); // limit the lifetime of a bullet
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (bullet_type == bullet_behaviors.anim_facing)
		{
			AnimFacingSprite(); // pick 1 frame of an animated sprite and match it to 8-directional heading
		}


	}

	void AnimFacingSprite()
	{
		#region Choose Bullet Direction Sprite
		// Choose a sprite frame based on direction of bullet velocity component.

		/* 
		 * 
		 *  0 | 1 | 2     UL | UU | UR
		 *  3 | 4 | 5  =  LL | XX | RR
		 *  6 | 7 | 8     LD | DD | DR
		 * 
		 */


		velocity = rigidbody2D.velocity;

		if (velocity.x < 0)
		{
			// Left
			if(velocity.y < 0)
			{
				anim_desired_time = 6; // LD
			}
			else if (velocity.y == 0)
			{
				anim_desired_time = 3; // LL
			}
			else if (velocity.y > 0)
			{
				anim_desired_time = 0; // LU
			}
		}
		else if (velocity.x > 0)
		{
			// Right
			if(velocity.y < 0)
			{
				anim_desired_time = 8; // RD
			}
			else if (velocity.y == 0)
			{
				anim_desired_time = 5; // RR
			}
			else if (velocity.y > 0)
			{
				anim_desired_time = 5; // RU
			}
		}
		else if (velocity.x == 0)
		{
			// Vertical
			if(velocity.y < 0)
			{
				anim_desired_time = 7; // DD
			}
			else if (velocity.y == 0)
			{
				anim_desired_time = 4; // XX = Not Moving
			}
			else if (velocity.y > 0)
			{
				anim_desired_time = 1; // UU
			}
		}
		#endregion
		
		my_anim.playbackTime = anim_desired_time; // set animation frame to desired_time 
		my_anim.speed = 0.0f; // playback speed 0 so animation doesn't advance frames.
		my_anim.StartPlayback(); 
	}


	public void impact()
	{
		rigidbody2D.velocity = Vector3.zero; // stop movement

		if (bullet_type == bullet_behaviors.exploder)
		{
			SendMessage("explode");
			// explode();
		}
		else
		{
			Destroy(gameObject); // immediately destroy gameobject.
		}
	}

	void explode()
	{
		my_anim = gameObject.GetComponent<Animator>();
		if (my_anim != null)
		{
			my_anim.SetBool("explode", true);
		}

		Destroy(gameObject, explode_length);
	}

}
