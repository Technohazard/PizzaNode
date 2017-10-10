using UnityEngine;
using System.Collections.Generic;

public class enemy_wave : MonoBehaviour {
	
		/// <summary>
		/// Display name of this wave.
		/// </summary>
	public string waveName;
	
		/// <summary>
		/// The index of this wave as assigned by a wave spawner.
		/// </summary>
		public int assignedIndex = 0; 
	public enum WaveTypes{
							GameObjects,
							TotallyRandom, 
							StrongestFirst, 
							Ring
							}; 
		// GameObjects: Hides GO's on start, Activates child gameObjects on spawn.

	public WaveTypes wave_type = WaveTypes.TotallyRandom;
	public float RingRadius = 4.0f;
	public Vector3 original_offset;

	/// <summary>
	/// If all enemies from this wave die, destroy the game object.
	/// </summary>
	public bool DestroyOnComplete = false;
	public int MaxSpawnCounter = 3; // how many times this wave can spawn before running out of dudes.
	private int spawnCounter; // how many times has this wave spawned?
	
	/// <summary>
	/// Spawned objects will start with a target of this Target. Usually the Player
	/// </summary>
	public GameObject Target;
	
	/// <summary>
	/// distance at which the arrow stops nav'ing to point to the center of the wave
	/// </summary>
	public float mNavMinDistance = 2.0f; 

	// State machine for wave control
	// inactive = default state, not yet ready to spawn d00ds
	// spawn = set up dudes and move immediately to control
	// control = make dudes fly around, attack waves
	// paused = like control but nothing moves
	// dead = all dudes have been killed
	public enum WaveStates {Inactive, Spawn, Control, Paused, Dead}
	public WaveStates State = WaveStates.Inactive;

	public GameObject[] EnemyPrefabArray; // array of prefab links
	public List<GameObject> EnemyPrefabList; // fill this with prefabs from the array to use for making this wave

	public List<GameObject> enemies; // Master list of spawned enemies

	public bool showCentroid = true; // show the position centroid on the debug screen
	protected Vector3 group_centroid = Vector3.zero; // calculated in drawCentroid();

	public float seperation_range = 1.0f; // used for flocking
	public float detection_range = 2.0f; // used for flocking

	public Vector2 grid_spacing = new Vector2(2.0f, 2.0f); // horiz/vert spacing from center of group.

	public Vector2 group_size = new Vector2(3,3); // should be square, indexed from 0


	// This is the size of the map // camera
	// defines area the ships will try to stay in. For now, limit to camera screen size.
	// private Vector2 screen_size = new Vector2(Screen.width, Screen.height);

	private GameObject spawn_prefab; // temp variable for spawning prefabs

	private int ring_index = 0; // index of prefab to use when spawning prefabs along a ring

	// Use this for initialization
	void Start () 
	{
		if (wave_type == WaveTypes.GameObjects)
		{
				ScanChildren ();
		}

		foreach (GameObject pf in EnemyPrefabArray)
		{
			EnemyPrefabList.Add(pf);
		}

		if (EnemyPrefabList.Count < 1)
		{
			// Need at least one prefab element.
			Debug.Log (gameObject.name.ToString() + ": No Enemy Prefab List Found for Wave.");

			// put state into Inactive for now.
			State = WaveStates.Inactive;
			// loadWave();
		};

		if (Target != null)
		{
			// Target found! Save the initial offset for re/spawning more enemies 
			original_offset = transform.position - Target.transform.position;
		}
		else
		{
			Debug.Log (gameObject.name.ToString() + ": No Target for this wave object. Targeting player...");
			Target = GameObject.FindGameObjectWithTag("Player");
			if (Target != null)
			{
				// Target Player
			}
			else 
			{
				Debug.Log (gameObject.name.ToString() + ": Targeting Player Failed! Now Targeting Earth!");
				Target = GameObject.FindGameObjectWithTag("Earth");
				if (Target != null)
				{
					// Target Earth
				}
				else
				{
					Debug.Log (gameObject.name.ToString() + ": Earth not found?!");
					Target = null;
				}
			}
		}

		spawnCounter = MaxSpawnCounter;
	}
	
	public void triggerSpawn()
	{
		State = WaveStates.Spawn;
	}

	void spawn()
	{
		// generate a wave, spawn all prefabs.
		if (spawnCounter > 0)
		{
			switch(wave_type)
			{
				case  WaveTypes.TotallyRandom:
				{
					// Generate a wave of enemiex, size (x, y)
					GenerateWave((int)group_size.x, (int)group_size.y);

										CreateNavArrow (gameObject);


					break;
				}
				case WaveTypes.StrongestFirst:
				{
					// Generate a wave of enemiex, size (x, y)
					GenerateWave((int)group_size.x, (int)group_size.y);
					break;
				}
				case WaveTypes.Ring:
				{
					// Generate a wave of enemiex, size (x, y)
					GenerateRingWave((int)group_size.x, RingRadius);
					break;
				}
				case WaveTypes.GameObjects:
				{
					// Activate all child gameobjects.
					Debug.Log(string.Format ("Spawning {0} enemies from GameObject", enemies.Count));
					foreach (GameObject obj in enemies) 
					{
						obj.SetActive (true);
					}
				}
				break;
			}

			spawnCounter--; // can't spawn infinitely.
		}
		else
		{
			Debug.Log(gameObject.name.ToString() + ": Wave Exhausted!");
		}
	}

	/// <summary>
	/// add a nav arrow pointing to the wave gameObject
	/// </summary>
	private void CreateNavArrow (GameObject obj)
	{
		GameObject fakeArrow = Resources.Load<GameObject> ("Prefabs/NavArrow_Enemy");

		if (fakeArrow != null) 
		{
			GameObject goArrow = (GameObject)GameObject.Instantiate (fakeArrow);
			if (goArrow != null) 
			{
				goArrow.transform.parent = GameObject.FindGameObjectWithTag ("Player").transform;
				NavArrow naControl = goArrow.GetComponent<NavArrow> ();
				if (naControl) 
				{
					naControl.SetTarget (gameObject);
				}
			}
		}
	}		

		/// <summary>
		/// Spawn a group of enemy prefabs in a group of size X, Y
		/// </summary>
		/// <param name="size_x">Size x.</param>
		/// <param name="size_y">Size y.</param>
	private void GenerateWave(int size_x, int size_y)
	{
		for (int yl = 0; yl <= size_y; yl++)
		{
			for (int xl = 0; xl <= size_x; xl++)
			{
				// run conditions to determine prefab index for this wave cell
				spawn_prefab = GenerateEnemy(xl, yl);
				
				addEnemy(spawn_prefab);
			}
		}
	}
	
	/// <summary>
	/// Spawn <paramref name="numDudes/>/ enemies in a ring of <paramref name="radius"/>
	/// </summary>
	/// <param name="numDudes">Number dudes.</param>
	/// <param name="radius">Radius.</param>
	void GenerateRingWave(int numDudes, float radius)
	{
		// float rotation = 0.0f; // starting rotation for spawning dudes

		for (int i = 0; i < numDudes; i++)
		{
			// run conditions to determine prefab index for this wave cell
			// the GenerateEnemy method knows it's WaveTypes.ring; 
			spawn_prefab = GenerateEnemy(i, 0);
			
			addEnemy(spawn_prefab);
		};
	}

	/// <summary>
	/// Add a bird to the flock.
	/// <para>enemy to add</para>
	/// </summary>
	public void addEnemy(GameObject to_add) 
	{
		enemies.Add(to_add);
	}

	/**
     * Remove a bird from the flock. One bird of the given color is removed.
     *
     * @param  color The color of the bird to remove
     */
	void removeEnemy(GameObject to_remove) 
	{
		enemies.Remove(to_remove);

		/*
		 * 
		for (int i=0; i < enemies.Count; i++) 
		{
			Bird bird = (Bird)birds.elementAt(i);
		
			if (bird.getColor() == color) 
			{
				birds.removeElementAt(i);
				break;
			}

		*/

	}
	/// <summary>
	/// Create and return enemy prefab for (x,y)
	/// </summary>
	/// <param name="x">Grid x coordinate.</param>
	/// <param name="y">Grid y coordinate.</param>
	GameObject GenerateEnemy(int xp = 0, int yp = 0)
	{
		int prefab_index = 0;
		int max_prefabs = 0;

		GameObject clone; 
		GameObject temp_spawn_prefab = new GameObject();

		prefab_index = calculate_enemy_for(xp, yp);
		max_prefabs = EnemyPrefabList.Count;

		if (prefab_index < max_prefabs)
		{
			clone = EnemyPrefabList[prefab_index];
			temp_spawn_prefab = (GameObject)Instantiate(clone, gameObject.transform.position, Quaternion.identity);

			if (wave_type == WaveTypes.TotallyRandom) 
			{
				temp_spawn_prefab.transform.parent = gameObject.transform;
				temp_spawn_prefab.transform.Translate(new Vector3(xp * grid_spacing.x, yp * grid_spacing.y,0), Space.Self);			
			}
			else if ((wave_type == WaveTypes.StrongestFirst))
			{
				// test of new spawn formula, tries to instantiate at the right location, instead of a 2-step translate process
				Vector3 to_translate = transform.position + new Vector3(xp * grid_spacing.x, yp * grid_spacing.y,0);
				temp_spawn_prefab = (GameObject)Instantiate(clone, to_translate, Quaternion.identity);
				temp_spawn_prefab.transform.parent = gameObject.transform;
			}

			else if (wave_type == WaveTypes.Ring)
			{
				// still using old style location spawning. needs to be lerped around a ring!
				temp_spawn_prefab = (GameObject)Instantiate(clone, gameObject.transform.position, Quaternion.identity);
				temp_spawn_prefab.transform.parent = gameObject.transform;
				temp_spawn_prefab.transform.Translate(new Vector3(xp * grid_spacing.x, yp * grid_spacing.y,0), Space.Self);	
			}
		
			return temp_spawn_prefab;
		}
		else
		{
			// spawn_prefab = (GameObject)Instantiate(Resources.Load<GameObject>("beacon_Proximity"));
			Debug.Log (gameObject.name.ToString() +" : prefab_index < enemy_prefabs" + " PI: " + prefab_index.ToString()) ;
			temp_spawn_prefab = new GameObject();
			temp_spawn_prefab.transform.position = gameObject.transform.position;
			return temp_spawn_prefab;
		}
	}


	/// <summary>
	/// Based on current wave_type, generate the enemy type that would spawn at grid location (x,y)
	/// </summary>
	/// <param name="x">Grid x coordinate.</param>
	/// <param name="y">Grid y coordinate.</param>
	int calculate_enemy_for(int x, int y)
	{
		int result = 0; // returns 1st enemy if nothing changes
		int num_prefabs = 0;

		if (EnemyPrefabList != null)
		{ 
			num_prefabs = EnemyPrefabList.Count;
		}
		else
		{
			// enemy_prefabs == null; // FUCK
			Debug.Log(gameObject.name.ToString() + ": No Enemy Prefabs defined!");
		}

		switch (wave_type)
		{
			case WaveTypes.TotallyRandom:
			{
				result = (int)(Random.value * num_prefabs);
				break;
			}
			case WaveTypes.StrongestFirst:
			{
				if (y == group_size.y)
				{
					// last/bottom row 
					result = 1; // bigger unit 
				}
				else if (y == 0)
				{
					// first/top row
					result = 2; // cooler unit
				}
				else
				{
					result = 0; // default unit
				}
				break;
			}

			case WaveTypes.Ring:
			{
				// deploys enemies from list in a repeating, circular pattern
				if ((ring_index < num_prefabs) && (ring_index >= 0))
				{
					result = ring_index;
				ring_index++;
				}
				else
				{
					ring_index = 0;
				}
				break;
			}
		}

		return result;
	}

	// Update is called once per frame
	void Update () {
		switch (State)
		{
			case WaveStates.Inactive:
			{
				// zzzz
				break;
			}
			case WaveStates.Spawn:
			{
				// spawn dudes
				spawn();

				// move immediately to control state so we don't multi-spawn
				State = WaveStates.Control;
				break;
			}
			case WaveStates.Control:
			{
				// not much here yet
				// in the future - talk to pilot AI, issue orders, etc.

				// calculate + draw the centroid of the group in scene view
				if (showCentroid)
				{
					drawCentroid ();
				}
				
				// it sucks to do this every update. Fix it!
				checkEnemyState();

				if (enemies.Count <= 0)
				{
					// all enemies in this wave are destroyed
					Debug.Log ("All enemies in wave destroyed!");

					// If we have a respawn left, respawn!
					// otherwise destroy the gameobject and be dead! 
					if (spawnCounter > 0)
					{ 
						State = WaveStates.Spawn; // don't forget, this automatically decrements SpawnCounter for us, so we don't have to.
					}
					else
					{
					    State = WaveStates.Dead;
					}
				}

				break;
			}
			case WaveStates.Paused:
			{
				// zzzz here also
				break;
			}
			case WaveStates.Dead:
			{
				if (DestroyOnComplete)
				{	Debug.Log (gameObject.name.ToString() + ": Self Destructing in 5.0!");
					Destroy(gameObject, 5.0f);
					
				}
				break;
			}
		}
	}

	void checkEnemyState()
	{
		// go through the enemy list and make sure they're still around
		foreach (GameObject enemy in enemies)
		{
			if (enemy == null)
			{
				// enemies.Remove(enemy);
				Debug.Log ("Can't find enemy in wave: " + waveName);
			}
		}

	}

	void drawCentroid()
	{
		List<Transform> obj_list = new List<Transform>(); // objects to process

		// get a list of their transforms
		foreach (GameObject enemy in enemies)
		{
			if (enemy != null)
			{
				if (enemy.transform != null)
				{
					obj_list.Add(enemy.transform);
				}
			}
		}

		// calculate the centroid of all group objects,
		group_centroid = calculateCentroid(obj_list);

		// draw debug lines from each object to centroid.
			foreach (Transform t in obj_list)
		{
			Debug.DrawLine(t.position, group_centroid, Color.red);
		}
	}

	void CenterOfMass(GameObject[] assembly)
	{
		Vector3 CoM = Vector3.zero;
		float c = 0f; // sum of all weights
		
		foreach (GameObject part in assembly)
		{
			CoM += part.GetComponent<Rigidbody>().worldCenterOfMass * part.GetComponent<Rigidbody>().mass;
			c += part.GetComponent<Rigidbody>().mass;
		}
		
		CoM /= c;
	}

	// <summary>
	// given a list of transforms
	// calculate the center of that group and return it as Vector3
	// </summary>
	Vector3 calculateCentroid(List<Transform> t_list)
	{

		Vector3 centroid = Vector3.zero;
		int num_t = t_list.Count;

		foreach (Transform t in t_list)
		{
			centroid += t.position;
		}

		centroid /= num_t;

		return centroid;
	}

	void loadWave()
	{
		Debug.Log (gameObject.name.ToString() + ": Loading Default Prefabs.");

		if (EnemyPrefabList.Count <= 0)
		{
			GameObject temp_enemy = Resources.Load<GameObject>("Tyrian_Carrot_trans_0");
			EnemyPrefabList.Add(temp_enemy);
			Debug.Log ("Added Default Prefab: " + temp_enemy.name.ToString());

			temp_enemy = Resources.Load<GameObject>("Tyrian_Carrot_trans_1");
			EnemyPrefabList.Add(temp_enemy);
			Debug.Log ("Added Default Prefab: " + temp_enemy.name.ToString());
		}
		else
		{
			Debug.Log ("enemy prefab list length: " + EnemyPrefabList.Count.ToString());
		}

		int num_prefabs = EnemyPrefabList.Count;
		Debug.Log (gameObject.name.ToString() + "DEFAULTS LOADED: new Num Prefabs: " + num_prefabs.ToString());
		foreach (GameObject go in EnemyPrefabList)
		{
			Debug.Log ("EP+: " + go.name.ToString ());
		}

	}

	// Reset the wave and get ready to respawn it
	// call this when the player died, from wave_spawner.
	public void WaveReset()
	{
		// destroy all existing dudes;
		ClearWave();
	
		// reset spawn counter to max spawn counter
		spawnCounter = MaxSpawnCounter;
		State = WaveStates.Paused;
	}

	void ClearWave()
	{
		// destroy all existing dudes;
		Debug.Log ("Removing all enemies from wave: " + waveName);

		foreach (GameObject enemy in enemies)
		{
			if (enemy != null)
			{
				// Enemy Found!
				removeEnemy(enemy);
			}
		}
	}

	// returns the navigational distance to the center of the wave.
	public float GetNavMinDistance()
	{
		return mNavMinDistance;
	}

		/// <summary>
		/// Scan all child objects for enemies, add them to enemy array, and deactivate
		/// Used by "GameObjects" waves
		/// </summary>
		public void ScanChildren()
		{
				foreach (Transform tform in transform)
				{
						if (tform.CompareTag ("Enemy"))
						{
								Debug.Log ("Adding child " + tform.name.ToString ());
								addEnemy (transform.gameObject);
								transform.gameObject.SetActive (false);
						}
						else
						{
								Debug.Log ("child {0} not an enemy" + tform.name.ToString ());
						}
				}

				Debug.Log (string.Format ("Child Enemy Scan Count: {0}", enemies.Count));
		}
}
