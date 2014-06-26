using UnityEngine;
using System.Collections;

public class carrot_ship : MonoBehaviour {
	private health_script info_checker; // used to read internal health variables for controlling damage state

	private health_script scan_target; // reports information about anything entering sensor range

	public float damage_limit = 0.10f; // 10% of max health
	public bool alerted = false;
	public bool damaged = false;

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator>();
		info_checker = gameObject.GetComponent<health_script>();
	}
	
	// Update is called once per frame
	void Update () {

		if ((info_checker.hp / info_checker.max_hp) <= damage_limit)
		{
			damaged = true;
			animator.SetBool("Damaged", true);
		}
		else
		{
		// beacon is of sufficient health to animate
		if (damaged == true)
			{
				animator.SetBool("Damaged", true);
			}
			else
			{
				animator.SetBool("Damaged", false);

				if (alerted==true) {
					animator.SetBool("Alerted",true);
				}
				else if (alerted==false) {
					animator.SetBool("Alerted", false);
				}
			}
		}
	}

	void unit_destroy() {
		//Animator.SetBool("Destroy", true);
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		// Is this a shot?
		scan_target = otherCollider.gameObject.GetComponent<health_script>();
		if (scan_target != null)
		{
			// Go into alert mode if enemies nearby
			if (scan_target.isEnemy)
			{
				alerted = true;
			}
		}
	}

}
