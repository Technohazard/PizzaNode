using UnityEngine;
using UnityEngine.UI;

 // for weapon icon
using System.Collections;

// TODO: put all the enemy-specific weapon stuff like the AI link and 'if tag == enemy' stufff in an EnemyWeapon class.
// it's still a weapon and can be treated like one so we can use weapons interchangably.

/// <summary>
/// The base class for a game Weapon.
/// Has a bullet prefab it instantiates and fires.
/// </summary>
[RequireComponent (typeof(AudioSource))]
public class Weapon : MonoBehaviour
{

		/// <summary>
		/// Prefab of bullet this weapon fires.
		/// </summary>
		public GameObject bullet_prefab;
		// TODO: make this a prefab array.

		/// <summary>
		/// Reference to target for aiming
		/// </summary>
		public GameObject Target;

		/// <summary>
		/// "Pretty" display name.
		/// </summary>
		public string DisplayName = "Please Ignore";

		/// <summary>
		/// Icon representing this weapon.
		/// like a cool cannon or something.
		/// </summary>
		public Sprite Icon;

		/// <summary>
		/// Icon representing the primary type of damage this weapon deals.
		/// ex. blue, red, green, purple
		/// </summary>
		public Sprite DamageTypeIcon;

		/// <summary>
		/// Maximum charge
		/// </summary>
		public float ChargeMax = 100.0f;

		/// <summary>
		/// Charged enery available
		/// </summary>
		public float Charge = 100.0f;

		/// <summary>
		/// Charge gained over ChargeTime
		/// </summary>
		public float ChargeRate = 10.0f;

		/// <summary>
		/// Number of seconds between Charge Rate updates
		/// </summary>
		public float ChargeTime = 1.0f;

		/// <summary>
		/// Charge spent everytime a bullet fires
		/// </summary>
		public float ChargePerBullet = 20.0f;
	
		/// <summary>
		/// Seconds to wait before firing another shot.
		/// </summary>
		public float ShotSpacing = 0.3f;

		/// <summary>
		/// True if weapon has enough charge to fire.
		/// </summary>
		public bool ChargeReady = false;

		/// <summary>
		/// True if weapon firing cooldown is reset.
		/// </summary>
		public bool ShotReady = false;

		// Internal timers for charging and cooldowns
		private float _chargeTimer = 0.0f;
		private float _shotTimer = 0.0f;

		// Damage ranges
		/// <summary>
		/// The minimum damage dealt by this weapon
		/// </summary>
		public int MinimumDamage = 1;

		/// <summary>
		/// The maximum damage dealt by this weapon.
		/// </summary>
		public int MaximumDamage = 2;

		private GameObject tmpBullet;
		// semi-temporary object used to instantiate new bullets.
	
		/// <summary>
		/// Sounds emitted by this weapon.
		/// </summary>
		public AudioClip[] AudioClips;
		// ex:
		// firing (1,2,3...n) waves of shots?
		// out of charge
		// fully charged
		// each charge tick

		/// <summary>
		/// Set to true if audio component is detected 
		/// </summary>
		private bool audio_connected = true;

		/// <summary>
		/// if this is an enemy weapon, a link to its AI for targeting purposes, etc.
		/// should be in the child class of Wepaon, EnemyWeapon
		/// </summary>
		private enemy_pilot _EnemyPilot;

		/// <summary>
		/// Ref to the player's crosshairs object.
		/// </summary>
		public GameObject CrosshairsObject; 

		// Use this for initialization
		void Start ()
		{
				// if no bullet type is assigned, use defaults
				if (bullet_prefab == null) {
						bullet_prefab = (GameObject)Resources.Load ("bullet_red_01");
				}

				AcquireFirstTarget ();

				if (GetComponent<AudioSource> ()) {
						audio_connected = true;
				} else {
						Debug.Log (gameObject.name.ToString () + ": No AudioSource connected on this weapon!");
				}

				if (DisplayName == "") {
						DisplayName = "+Unnamed Weapon+";
				}
		}

		/// <summary>
		/// Depending on parent tag.
		/// Enemy: Acquire the parent's target (usually player / Earth)
		/// Player: the player's crosshairs object.
		/// </summary>
		private void AcquireFirstTarget()
		{
				switch(transform.parent.tag)
				{
					case "Enemy":
							{
									_EnemyPilot = gameObject.transform.parent.GetComponent<enemy_pilot> ();
									Target = _EnemyPilot.Target;
							}
							break;
					case "Player":
					case "Player_Units":
							{
									if (CrosshairsObject == null) {
											CrosshairsObject = GameObject.Find ("player_target");
									}

									Target = CrosshairsObject;

									if (Target == null) {
											Debug.Log ("Weapon target not set: crosshairs object == null.");
									}	
							}
							break;
					default:
							{
									Debug.Log ("Weapon target not set: unknown parent tag (not Player or Enemy");
							}
							break;
				}
		}

		// Update is called once per frame
		void Update ()
		{
				ReTarget ();
				UpdateChargeTimer ();
				UpdateShotTimer ();
		}

		void ReTarget ()
		{
				if (transform.parent.tag == "Enemy") {
						if (_EnemyPilot != null)
						{
								Target = _EnemyPilot.Target;		
						}
				}
				else 
				{
					if (Target == null)
					{
						Target = CrosshairsObject;
					}
										
					if (Target == null) 
					{
						Debug.Log (string.Format("Player Weapon {0} target == null.", name));
					}
				}
		}

		/// <summary>
		/// Called from enemy_pilot.cs script as a child object.
		/// Checks to see if a bullet can be fired and instantiates prefab.
		/// </summary>
		public void Fire ()
		{
				if (Charge >= ChargePerBullet) {
						if (_shotTimer == 0.0f) {
								Charge -= ChargePerBullet; // ZAP
		
								tmpBullet = Instantiate (bullet_prefab, 
										transform.position, 
										Quaternion.identity) 
											as GameObject;

								// check to see whether fired by enemy or player
								if (transform.parent.tag == "Enemy") {
										tmpBullet.transform.parent = GameObject.Find ("Enemy_Bullet_Group").transform;
								} else {
										tmpBullet.transform.parent = GameObject.Find ("Player_Bullet_Group").transform;
								}

								// Set the bullet's target to the current playertarget
								tmpBullet.GetComponent<move_to_target> ().selected = Target;
				
								// register 'reached target' callback if appropriate.


								// after a bullet has been fired, set the spacing timer to disallow further bullets
								_shotTimer = ShotSpacing;
				
								if (audio_connected) {
										shot_sound_play ();
								}

						} // end shot_timer == 0.0f
			else {
								// weapon not ready to fire next bullet. (cooldown timer still counting down)
								ShotReady = false; 
						}
				} else {
						// weapon doesn't have enough charge to fire
						ChargeReady = false;
				}
		}

		/// <summary>
		/// instantiate a new bullet from prefab and return the object
		/// </summary>
		/// <returns>The bullet.</returns>
		private GameObject SpawnBullet ()
		{
				GameObject tmpObj = null;
				tmpObj = (GameObject)Instantiate (bullet_prefab, transform.position, Quaternion.identity);
				return tmpObj;
		}

		void UpdateChargeTimer ()
		{
				_chargeTimer += Time.deltaTime;
				if (_chargeTimer > ChargeTime) {
						_chargeTimer = 0.0f; // reset timer

						// recharge on update time
						if (Charge < ChargeMax) {
								if ((Charge + ChargeRate) <= ChargeMax) {
										Charge += ChargeRate;
								} else if ((Charge + ChargeRate) > ChargeMax) {
										Charge = ChargeMax;
								}
						}

						// weapon is ready to fire if there's enough charge to shoot 1 bullet
						if (Charge >= ChargePerBullet) {
								ChargeReady = true;
						} else {
								ChargeReady = false;
						}

		

				}
		}

		void UpdateShotTimer ()
		{
				if (_shotTimer > 0.0f) {
						if ((_shotTimer - Time.deltaTime) < 0.0f) {
								_shotTimer = 0.0f;
								ShotReady = true;
						} else {
								_shotTimer -= Time.deltaTime;
								ShotReady = false;
						}
				} else {
						ShotReady = true;
						// no need to update, next bullet ready to fire.
				}
		}

		/// <summary>
		/// TODO: why did i getcomponents every time I wanted to play a sound, ffs.
		/// </summary>
		void shot_sound_play ()
		{
				if (AudioClips.Length > 0) {
						GetComponent<AudioSource> ().clip = AudioClips [0];
						GetComponent<AudioSource> ().Play ();
		
						//AudioSource.PlayClipAtPoint(snd_shot[0], transform.position);
				}


		}
	
}
