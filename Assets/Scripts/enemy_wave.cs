using UnityEngine;
using System.Collections.Generic;

public class enemy_wave : MonoBehaviour {

	public GameObject[] enemy_prefabs; // list of the prefabs to use for making waves
	private GameObject spawn_prefab; // temp variable for spawning prefabs

	public List<GameObject> enemies = new List<GameObject>(); // Master list of spawned enemies

	public Transform group_center; // calculated in drawCentroid();

	public float seperation_range = 1.0f; // used for flocking
	public float detection_range = 2.0f; // used for flocking

	public Vector2 grid_spacing = new Vector2(2.0f, 2.0f); // horiz/vert spacing from center of group.

	public Vector2 group_size = new Vector2(3,3); // should be square, indexed from 0

	public string wave_type = "random"; // soon: "strongest_first";

	// This is the size of the map // camera
	// defines area the ships will try to stay in. For now, limit to camera screen size.
	private static Vector2 map_size = new Vector2(Screen.width, Screen.height);

	public bool showCentroid = true; // show the position centroid on the debug screen

	// Use this for initialization
	void Start () 
	{
		// populate enemy grid
		// GameObject temp_enemy;

		generate_wave((int)group_size.x, (int)group_size.y);
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
	void removeEnemy(Color color) {
		for (int i=0; i < enemies.Count; i++) 
		{
			/*
			 * Bird bird = (Bird)birds.elementAt(i);
		
			if (bird.getColor() == color) 
			{
				birds.removeElementAt(i);
				break;
			}
			*/

			// nothing! for now...
		}
	}

	GameObject GenerateEnemy(int xp=0, int yp=0)
	{
		int prefab_index = calculate_enemy_for(xp, yp);
		
		spawn_prefab = (GameObject)Instantiate(enemy_prefabs[prefab_index], group_center.position, Quaternion.identity);
		spawn_prefab.transform.parent = group_center;
		spawn_prefab.transform.Translate(new Vector3(xp * grid_spacing.x, yp * grid_spacing.y,0), Space.Self);

		return spawn_prefab;
	}


	/// <summary>
	/// Based on current wave_type, generate the enemy type that would spawn at grid location (x,y)
	/// </summary>
	/// <param name="x">Grid x coordinate.</param>
	/// <param name="y">Grid y coordinate.</param>
	int calculate_enemy_for(int x, int y)
	{
		int result = 0; // returns 1st enemy if nothing changes

		switch (wave_type)
		{
			case "random":
			{
				result = (int)(Random.value * enemy_prefabs.Length);
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
		if (showCentroid)
		{
			drawCentroid ();
		}

		if (enemies.Count <= 0)
		{
			// all enemies destroyed

		}

	}

	void drawCentroid()
	{
		Vector3 centroid = Vector3.zero;
		List<Transform> obj_list = new List<Transform>(); // objects to process

		// get a list of their transforms
		foreach (GameObject enemy in enemies)
		{
			if (enemy != null)
			{
				obj_list.Add(enemy.transform);
			}
		}

		// calculate the centroid of all group objects,
		centroid = calculateCentroid(obj_list);

		// draw debug lines from each object to centroid.
			foreach (Transform t in obj_list)
		{
			Debug.DrawLine(t.position, centroid, Color.red);
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
}
