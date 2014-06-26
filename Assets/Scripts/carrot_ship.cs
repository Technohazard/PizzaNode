using UnityEngine;
using System.Collections;

public class enemy_beacon : MonoBehaviour {
	public float beacon_health = 100.0f;
	public float max_health = 100.0f;
	public float damage_limit = 0.10f; // 10% of max health
	public bool beacon_alerted = false;
	public bool beacon_damaged = false;

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		beacon_health = max_health;
	}
	
	// Update is called once per frame
	void Update () {

		if ((beacon_health / max_health) <= damage_limit)
		{
		// Always check to see if the beacon has enough health to function / die
			if (beacon_health < 1)
			{
				// le beacon is le dead
				beacon_destroy();
			}
			else
			{
				beacon_damaged = true;
			}
		}
		else
		{
		// beacon is of sufficient health to animate
		if (beacon_damaged == true)
			{
				animator.SetBool("Damaged", true);
			}
			else
			{
				animator.SetBool("Damaged", false);

				if (beacon_alerted==true) {
					animator.SetBool("Alerted",true);
				}
				else if (beacon_alerted==false) {
					animator.SetBool("Alerted", false);
				}
			}
		}
	}

	void beacon_destroy() {
		//Animator.SetBool("Destroy", true);
		Destroy(gameObject);
	}

}
