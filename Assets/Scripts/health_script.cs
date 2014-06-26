﻿using UnityEngine;
using System.Collections;

public class health_script : MonoBehaviour {

	public int hp = 1;
	public int max_hp = 1;

	public bool isEnemy = true;

	// Use this for initialization
	void Start () {
		if (gameObject.tag == "Enemy")
			{
				isEnemy = true;
			}
		else if (gameObject.tag == "Player")
		{
			isEnemy = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Damage(int damageCount)
	{
		hp -= damageCount;
		
		if (hp <= 0)
		{
			// Dead!
			Destroy(gameObject);
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
					Destroy(shot.gameObject); // Remember to always target the game object, otherwise you will just remove the script
				}
			}		
	}

}
