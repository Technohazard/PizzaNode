using UnityEngine;
using System.Collections.Generic;

// Wave Spawner
/*
 * States:
 * paused - not doing anything, starts this way
 * show_text - shows wave text for current wave
 * spawn - spawns ships into array
 * control - control active ships, checks to see if wave is over
 * wave_over - clean up, display over text
 * next - Advance to next wave, pause
 */

public class wave_spawner : MonoBehaviour {

	public string level_name;
	public GameObject[] enemy_prefabs; // list of the prefabs to use for making waves
	public GameObject[] group_centers; // array of group centers to act as parents for each wave of enemies.

	public int wave_num = 0; // current player wave

	public GUIText wave_text_guiText;
	public bool show_text = false;
	public float wave_text_displayTime = 3.0f; // how long to display wave text.

	private List<GameObject> wave_list = new List<GameObject>();
	private float timer;

	public enum wave_spawner_states {paused, show_text, spawn, control, wave_over, next};

	public wave_spawner_states state = wave_spawner_states.paused;

	// dictionary of wave name to display
	Dictionary<int, string> wave_names =
		new Dictionary<int, string>()
	{
		{0, "-Wave 0 -"},
		{1, "- Wave 1! -"},
		{2, "- Wave 2!! -"},
		{3, "- Wave 3!!! -"}
	};

	// Use this for initialization
	void Start () 
	{
		level_name = "+ Level 0 - Alpha Wave +";

		wave_names.Add(4, "+ Wave 4 +");
		wave_names.Add(5, "+ Wave 5! +");
		wave_names.Add(6, "+ Wave 6!! +");
		wave_names.Add(7, "+ Wave 7!!! +");

		string crapstring; 

		foreach (KeyValuePair<int, string> pair in wave_names)
		{
			crapstring = "Wave Ready:" + pair.Key + ":" + pair.Value;
			Debug.Log (crapstring);
		}

		/*
		 * if (dictionary.ContainsKey(num))
		{

		}
		*/

		// List<string> list = new List<string>(wave_names.Keys);

	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (state)
		{
			case wave_spawner_states.paused:
			{

				break;
			}
			case wave_spawner_states.show_text:
			{
				if (show_text == true)
				{
					timer += Time.deltaTime;
					if(timer > wave_text_displayTime)
					{
						timer = 0.0f; // reset timer
						show_text = false;
					}
					update_wave_text();
				}
				else
				{

				}

				break;
			}
			case wave_spawner_states.spawn:
			{
				handle_wave_spawn();
				break;
			}
			default:
			{
				// unhandled wave_spawner states
				break;
			}
		} // end switch

	}

	void AdvanceState()
	{
		switch (state)
		{
			case wave_spawner_states.paused:
			{
				Debug.Log("W~S: Showing Wave Text");
				state = wave_spawner_states.show_text;
				break;
			}
			case wave_spawner_states.show_text:
			{
				Debug.Log("W~S: Spawning Wave");
				state = wave_spawner_states.spawn;
				break;
			}
			case wave_spawner_states.spawn:
			{
				Debug.Log("W~S: Control Phase");
				state = wave_spawner_states.control;
				break;
			}
			case wave_spawner_states.control:
			{
				Debug.Log("W~S: Wave Over");
				state = wave_spawner_states.wave_over;
				break;
			}
			case wave_spawner_states.wave_over:
			{
				Debug.Log("W~S: Next Wave...");
				state = wave_spawner_states.next;
				NextWave();
				break;
			}
			case wave_spawner_states.next:
			{
				state = wave_spawner_states.paused;
				break;
			}
		}
	}

	void handle_wave_spawn()
	{
		// determine appropriate amount of crap to spawn for a wave
		// create wave objects
		spawn_wave();

	}

	void spawn_wave()
	{
		GameObject wave_core = new GameObject();
		wave_core.transform.position = new Vector3(0,0,0);

		// add a new enemy wave to the master wave list
		wave_list.Add(wave_core);
	}

	void load_all_prefabs()
	{
		GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");
		Debug.Log("Got: " + prefabs.Length);
		foreach (var go in prefabs)
		{
			Debug.Log(go.name);
		}
	}

	void update_wave_text()
	{
		string poostring; 

		if (wave_names.ContainsKey(wave_num))
		{
			poostring = wave_names[wave_num];
			wave_text_guiText.text = poostring;
		}

	}

	void NextWave()
	{
		if (wave_num < wave_names.Count)
		{
			wave_num++;
		}
		else
		{
			Debug.Log ("Last Wave Reached!");
		}
	}

	void OnGUI() 
	{
		int menu_width = 100, menu_height = 40;
		int button_width = 80, button_height = 20;
		int reset_width = 80, reset_height = 20;

		GUI.Box(new Rect((Screen.width / 2) - (menu_width/2) , (Screen.height - menu_height), menu_width, menu_height), "Wave Menu");
		
		// Make the first button.
		if(GUI.Button(new Rect((Screen.width / 2) - (button_width/2),Screen.height - button_height, button_width, button_height), "Start Wave")) 
		{
			spawn_wave();
		}

		if(GUI.Button(new Rect(0, Screen.height - reset_height, reset_width, reset_height), "Reset")) 
		{
			GameObject.Find("Player").SendMessage("location_reset");
		}

		//GUI.Box(new Rect(0,0,100,50), "Top-left");
		//GUI.Box(new Rect(Screen.width - 100,0,100,50), "Top-right");
		//GUI.Box(new Rect(0,Screen.height - 50,100,50), "Bottom-left");
		//GUI.Box(new Rect(Screen.width - 100, Screen.height - 50,100,50), "Bottom-right");

		// GUI.Label (new Rect ((Screen.width / 2) - 50, 100, 100, 50), "Label Control");



	}
}
