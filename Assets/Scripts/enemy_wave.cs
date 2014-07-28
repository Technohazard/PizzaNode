using UnityEngine;
using System.Collections.Generic;

public class enemy_wave : MonoBehaviour {

	public string waveName; // Name of this wave
	public string wave_type = "random"; // soon: "strongest_first";
	public bool waveSpawnOnStart = false;
	public int spawnCounter = 3; // how many times this wave can spawn before running out of dudes.

	// State machine for wave control
	// inactive = default state, not yet ready to spawn d00ds
	// spawn = set up dudes and move immediately to control
	// control = make dudes fly around, attack waves
	// paused = like control but nothing moves
	// dead = all dudes have been killed
	public enum waveStates {inactive, spawn, control, paused, dead}
	public waveStates state = waveStates.inactive;

	public GameObject[] enemy_prefab_array; // array of prefab links
	public List<GameObject> enemy_prefabs; // fill this with prefabs from the array to use for making this wave

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

	// Use this for initialization
	void Start () 
	{
		foreach (GameObject pf in enemy_prefab_array)
		{
			enemy_prefabs.Add(pf);
		}

		if (enemy_prefabs.Count < 1)
		{
			// Need at least one prefab element.
			Debug.Log (gameObject.name.ToString() + ": No Enemy Prefab List Found for Wave.");

			// put state into Inactive for now.
			state = waveStates.inactive;
			// loadWave();
		};
	}

	void triggerSpawn()
	{
		state = waveStates.spawn;
	}

	void spawn()
	{
		// generate a wave, spawn all prefabs.
		if (spawnCounter > 0)
		{
			// Generate a wave of enemiex, size (x, y)
			generate_wave((int)group_size.x, (int)group_size.y);

			spawnCounter--; // can't spawn infinitely.
		}
		else
		{
			Debug.Log(gameObject.name.ToString() + ": Wave Exhausted!");
		}

	}

	void generate_wave(int size_x, int size_y)
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

	/**
     * Add a bird to the flock.
     *
     * @param  bird The bird to add
     */
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
		GameObject temp_spawn_prefab;

		prefab_index = calculate_enemy_for(xp, yp);
		max_prefabs = enemy_prefabs.Count;


		if (prefab_index < max_prefabs)
		{
			clone = enemy_prefabs[prefab_index];

			temp_spawn_prefab = (GameObject)Instantiate(clone, gameObject.transform.position, Quaternion.identity);
			temp_spawn_prefab.transform.parent = gameObject.transform;
			temp_spawn_prefab.transform.Translate(new Vector3(xp * grid_spacing.x, yp * grid_spacing.y,0), Space.Self);			
		}
		else
		{
			// spawn_prefab = (GameObject)Instantiate(Resources.Load<GameObject>("beacon_Proximity"));
			Debug.Log (gameObject.name.ToString() +" : prefab_index < enemy_prefabs" + " PI: " + prefab_index.ToString()) ;
			temp_spawn_prefab = new GameObject();
			temp_spawn_prefab.transform.position = gameObject.transform.position;
		}
		return temp_spawn_prefab;
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

		if (enemy_prefabs != null)
		{ 
			num_prefabs = enemy_prefabs.Count;
		}
		else
		{
			// enemy_prefabs == null; // FUCK
			Debug.Log(gameObject.name.ToString() + ": No Enemy Prefabs defined!");
		}

		switch (wave_type)
		{
			case "random":
			{
				result = (int)(Random.value * num_prefabs);
				break;
			}
			case "strongest_first":
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
		}

		return result;
	}

	// Update is called once per frame
	void Update () {
		switch (state)
		{
			case waveStates.inactive:
			{
				// zzzz
				break;
			}
			case waveStates.spawn:
			{
				// spawn dudes
				spawn();

				// move immediately to control state so we don't multi-spawn
				state = waveStates.control;
				break;
			}
			case waveStates.control:
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
					// all enemies destroyed
					Debug.Log ("All enemies in wave destroyed!");
					Debug.Log (gameObject.name.ToString() + ": Self Destructing in 5.0!");
					state = waveStates.dead;
				}

				break;
			}
			case waveStates.paused:
			{
				// zzzz here also
				break;
			}
			case waveStates.dead:
			{
				// All dudes are dead. Destroy in 5 s
				Destroy(gameObject, 5.0f);
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
			CoM += part.rigidbody.worldCenterOfMass * part.rigidbody.mass;
			c += part.rigidbody.mass;
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

		if (enemy_prefabs.Count <= 0)
		{
			GameObject temp_enemy = Resources.Load<GameObject>("Tyrian_Carrot_trans_0");
			enemy_prefabs.Add(temp_enemy);
			Debug.Log ("Added Default Prefab: " + temp_enemy.name.ToString());

			temp_enemy = Resources.Load<GameObject>("Tyrian_Carrot_trans_1");
			enemy_prefabs.Add(temp_enemy);
			Debug.Log ("Added Default Prefab: " + temp_enemy.name.ToString());
		}
		else
		{
			Debug.Log ("enemy prefab list length: " + enemy_prefabs.Count.ToString());
		}

		int num_prefabs = enemy_prefabs.Count;
		Debug.Log (gameObject.name.ToString() + "DEFAULTS LOADED: new Num Prefabs: " + num_prefabs.ToString());
		foreach (GameObject go in enemy_prefabs)
		{
			Debug.Log ("EP+: " + go.name.ToString ());
		}

	}
}
