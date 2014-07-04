using UnityEngine;
using System.Collections;

public class beacon_sensor : MonoBehaviour {

	private bool scan_is_enemy; // reports information about anything entering sensor range

	private health_script enemy_scan; // used to scan enemy colliders for enemy data

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		
		if (otherCollider != null)
		{
			// Is this a shot?
			if (otherCollider.gameObject.tag == "Enemy")
			{
				scan_is_enemy = true;
			}
			
			// Go into alert mode if enemies nearby
			if (scan_is_enemy)
			{
				transform.parent.GetComponent<enemy_beacon>().alerted = true;
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D otherCollider)
	{
		
		if (otherCollider != null)
		{
			enemy_scan = gameObject.transform.parent.GetComponent<health_script>();

			if (otherCollider.gameObject.CompareTag(gameObject.tag))
			{
				scan_is_enemy = false;
			}

			// Go out of alert mode if enemies nearby
			if (scan_is_enemy)
			{
				transform.parent.GetComponent<enemy_beacon>().alerted = false;
			}
		}
	}

}
