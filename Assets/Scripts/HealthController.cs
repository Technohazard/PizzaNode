using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

	public double HP = 10.0f;
	public double HPMax = 10.0f;
	
	public double RechargeAmount = 10.0f; // amount added per charge time
	public double RechargeTime = 1.0f; // number of seconds before charge_rate updates

	private float _timer = 0.0f; // internal timer for restoring hp
	private double dmg_remainder = 0.0f; // if dmg is dealt resulting in health < 0, deal this remainder to parent.

	private bool _isEnemy;
	public bool isEnemy
	{
		get
		{
			if (gameObject.tag == "Enemy")
			{
				_isEnemy = true;	
			}
			else if (gameObject.tag == "Player")
			{
				_isEnemy = false;
			}

			return _isEnemy;
		}
		set
		{
			_isEnemy = value;
		}
	} 

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Recharge();
	}

	public void Damage(int damageCount)
	{
		if (HP > 0)
		{ 
			if (HP - damageCount > 0)
			{ 
				HP -= damageCount;
			}
			else
			{
				// damage dealt is greater than remaining health
				dmg_remainder = 0.0f;
				dmg_remainder = damageCount - HP;
				HP = 0.0f;

				gameObject.SendMessageUpwards("Damage", dmg_remainder); // this worked on 7/30

			}
		}
		else
		{ 
			HP -= damageCount;
		}
		
		if (HP <= 0)
		{
			// Dead!
			Die();
		}
	}

	void Die()
	{
		// TODO: 'death' stuff here
		// play death sound, anims, etc.

		// TODO: Oh no, this won't do.
		// Should only do this when a TRUE parent dies. What if we wanted to give children health?
		// Send message upward to parent instead.
		// tell parent enemy_wave manager that this unit died!
		if (gameObject != null)
		{
			if (gameObject.transform.parent != null)
			{
				if (transform.root != transform)
				{
					gameObject.transform.parent.SendMessage("removeEnemy", gameObject);
				}
			}
		}

		if (gameObject.tag == "Player")
		{
			// Oh poops, the player is dead? Game over baby.
			GameObject.Find("wave_spawner").SendMessage("PlayerDied");

		}

		Destroy(gameObject,0.1f);
	}

	/// <summary>
	/// Add HP over time, up to hpMax
	/// </summary>
	void Recharge()
	{
		_timer += Time.deltaTime;
		if(_timer > RechargeTime)
		{
			_timer = 0.0f; // reset timer
			
			if (HP < HPMax)
			{
				if ((HP + RechargeAmount) <= HPMax)
				{
					HP += RechargeAmount;
				}
				else if ((HP + RechargeAmount) > HPMax)
				{
					HP = HPMax;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
			// Is this a shot?
			bullet_01 shot = otherCollider.gameObject.GetComponent<bullet_01>();
			if (shot != null)
			{
				// Avoid friendly fire
				if (shot.EnemyShot != isEnemy)
				{
					Damage(shot.damage);
					
					// Destroy the shot
					shot.impact(); // Remember to always target the game object, otherwise you will just remove the script
				}
			}		
	}

}
