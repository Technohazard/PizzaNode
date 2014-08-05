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

	public string levelName; // Name of the Level (collection of 'enemy_wave's)
	public List<GameObject> waves; // list of objects with enemy_wave scripts. for now, child object

	public int waveIndex = 0; // current player wave

	// Wave Text GUI definitions
	public GUIText waveText_guiText; // Link to wave text GUI element
	public GUIText LevelText_guiText; // Link to level text GUI element

	public bool show_text = true; // Display what's in the GUI element
	public float waveTextDisplayTime = 3.0f; // how long to display wave text.
	public float timerTextColor_rate = 0.2f; // how fast the flashing color effect cycles
	public Color text_color; // Initial color of text
	public bool waveTextMode_ = false;

	public AudioClip snd_wave; // sound of next wave
	public AudioClip snd_state; // sound of wave state advancing

	public enum wave_spawner_states {paused, wave_start, show_text, spawn, control, wave_over, next, player_died};
	public wave_spawner_states state; // usually start paused

	// dictionary of wave name to display
	public Dictionary<int, string> wave_names;

	// private List<GameObject> wave_list = new List<GameObject>(); 

	private string s_waveText; // private representation of string to display in guiTex
	private float timerText = 0.0f; // how long the wave text has been onscreen
	private float timerTextColor = 0.0f; // how long until next color change on wave text
	private Color text_colorTarget; // target color to fade to over time.

	private enemy_wave current_wave_script; // reference to current wave script component.

	#region GUI Components
	private Vector2 screen_size = new Vector2(Screen.width, Screen.height);

	private Vector2 menuBox_size = new Vector2(100, 80);
	private Vector2 menuBox_pos = Vector2.zero; // new Vector2( (screen_size.x / 2) - (menuBox_size.x / 2), (screen_size.y - menuBox_size.y) ); 
	private string menuBox_text = "Wave Menu";
	private Rect menuBox_Rect;

	private static Vector2 spawnButton_size = new Vector2(80, 20);
	private static Vector2 spawnButton_pos = Vector2.zero; // new Vector2((Screen.width / 2) - (spawnButton_size.x/2), Screen.height - spawnButton_size.y);
	private static string spawnButton_text = "Spawn Wave";
	
	private static Vector2 phaseButton_size = new Vector2(80, 20);
	private static Vector2 phaseButton_pos = Vector2.zero; // new Vector2((Screen.width / 2) - (phaseButton_size.x/2), Screen.height - phaseButton_size.y - spawnButton_size.y - 5);
	private static string phaseButton_text = "Next Phase";
	
	private static Vector2 ResetButton_size = new Vector2(80, 20);
	private static Vector2 ResetButton_pos = Vector2.zero; // new Vector2(0, Screen.height - ResetButton_size.y);
	private	static string ResetButton_text = "Reset";

	#endregion

	void Awake()
	{
		Game_Manager.Instance.DoSomething();
		Debug.Log(Game_Manager.Instance.myGlobalVar);
	}


	// Use this for initialization
	void Start () 
	{
		// Define values for all member variables
		text_color = Color.white;
		state = wave_spawner_states.paused;
		wave_names = new Dictionary<int, string>();
		text_colorTarget = new Color(Random.value, Random.value, Random.value);

		int temp_num = 0;

		foreach (Transform child in transform)
		{

			// get list of all child objects for future reference.
			waves.Add(child.gameObject);
			wave_names.Add(temp_num, child.GetComponent<enemy_wave>().waveName);
			temp_num++;
		}

		waveText_guiText.guiText.enabled = true;
		LevelText_guiText.guiText.enabled = true;

		waveText_guiText.guiText.text = "";
		LevelText_guiText.guiText.text = "";


		GUI_init(); // define sizes for UI boxes and buttons
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (state)
		{
			case wave_spawner_states.paused:
			{
				// Do nothing while paused.
				hide_waveText();
				break;
			}

			case wave_spawner_states.wave_start:
			{
				// Display wave name
				hide_waveText();
				break;
			}
			case wave_spawner_states.show_text:
			{
				// Display text for wave_text_displayTime sec, then turn back off.
				if (show_text == true)
				{
					updateWaveText(); // sets text, color
				}
				else
				{
					//show_text is false, hide text;
					hide_waveText();
				}

				break;
			}
			case wave_spawner_states.spawn:
			{
				// immediately spawn a wave then move to control phase
				// if you don't do this, it will INFINITELY SPAWN WAVES and break the game.
				handle_wave_spawn();
				current_wave_script = waves[waveIndex].GetComponent<enemy_wave>();
				AdvanceState();
				break;
			}
			case wave_spawner_states.control:
			{
				if (current_wave_script != null)
				{
					if (current_wave_script.state != enemy_wave.waveStates.dead)
					{
						// enemy wave is still being controlled, don't do anything
					}
					else
					{
						Debug.Log ("Wave_Spawner detected dead enemy wave. Advancing wave state");
						AdvanceState();
					}
				}
				break;
			}
			case wave_spawner_states.player_died:
			{
				// Player is dead, do nothing.
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
		bool state_did_change = false;
		switch (state)
		{
			case wave_spawner_states.paused:
			{
				state = wave_spawner_states.show_text;
				show_text = true;
				timerText = 0.0f; // reset show text timer 
				show_waveText();
				Debug.Log("W~S: Showing Wave Text");
				state_did_change = true;
				break;
			}
			case wave_spawner_states.show_text:
			{
				state = wave_spawner_states.spawn;
				Debug.Log("W~S: Spawning Wave");
				state_did_change = true;
				break;
			}
			case wave_spawner_states.spawn:
			{
				state = wave_spawner_states.control;
				Debug.Log("W~S: Control Phase");
				state_did_change = true;
				break;
			}
			case wave_spawner_states.control:
			{
				state = wave_spawner_states.wave_over;
				Debug.Log("W~S: Wave Over");
				state_did_change = true;
				break;
			}
			case wave_spawner_states.wave_over:
			{
				state = wave_spawner_states.next;
				Debug.Log("W~S: Next Wave...");
				state_did_change = true;
				NextWave();
				break;
			}
			case wave_spawner_states.next:
			{
				state = wave_spawner_states.paused;
				Debug.Log("W~S: Wave Paused...");
				state_did_change = true;
				break;
			}
			case wave_spawner_states.player_died:
			{
				state = wave_spawner_states.paused;
				Debug.Log("W~S: Wave Paused...");
				state_did_change = true;
				break;
			}

		}

		if (state_did_change)
		{
			playSound_State();
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
		// find child gameobject matching wave #, and spawn it

		if (waveIndex < waves.Count)
		{
			waves[waveIndex].SendMessage("triggerSpawn");
		}
		else
		{
			Debug.Log ("No more waves to spawn!");
		}

		/* 
		GameObject wave_core = new GameObject();
		Vector3 new_wave_offset = original_offset;

		wave_core.transform.position = Vector3.zero;
		wave_core.transform.Translate(new_wave_offset);

		wave_core.AddComponent<enemy_wave>();
		// add a new enemy wave to the master wave list
		wave_list.Add(wave_core);
		 */
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

	void setWaveText(string to_set)
	{
		s_waveText = to_set;
	}

	void updateWaveText()
	{
		timerText += Time.deltaTime;
		
		if(timerText > waveTextDisplayTime)
		{
			// Text has displayed for its lifetime, turn it off now.
			show_text = false;
			hide_waveText();
			
			AdvanceState(); // text display time is over - spawn enemy wave	
		}
		else
		{
			// Still displaying text, make sure the object is enabled
			if (!waveText_guiText.enabled)
			{
				waveText_guiText.enabled = true;
			}
		}

		// update the GUI object with wave text
		if (wave_names.ContainsKey(waveIndex))
		{
			setWaveText(wave_names[waveIndex]);
		}
		else
		{
			setWaveText("+++ : " + waveIndex.ToString());
		}

		waveText_guiText.guiText.text = s_waveText;
		waveText_guiText.guiText.color = color_cycle();

		LevelText_guiText.guiText.text = levelName; // Link to GUI element
		LevelText_guiText.guiText.color = color_cycle();

	}

	void NextWave()
	{
		if (waveIndex < wave_names.Count)
		{
			playSound_Wave();
			waveIndex++;
		}
		else
		{
			Debug.Log ("Last Wave Reached!");
		}
	}

	void OnGUI() 
	{
		// Render all wave-related GUI components

		screen_size = new Vector2(Screen.width, Screen.height);
		
		// setup and initialize UI element variables
		menuBox_pos = new Vector2( (screen_size.x / 2) - (menuBox_size.x / 2), (screen_size.y - menuBox_size.y) ); 
		menuBox_Rect = new Rect(menuBox_pos.x , menuBox_pos.y, menuBox_size.x, menuBox_size.y);


		GUI.Box(menuBox_Rect, menuBox_text);

		// "Spawn Wave" button.
		if (GUI.Button(new Rect(spawnButton_pos.x, spawnButton_pos.y, spawnButton_size.x, spawnButton_size.y), spawnButton_text) ) 
		{
			spawn_wave();
			NextWave();
		}

		//  "Next Phase" button.
		if (GUI.Button(new Rect(phaseButton_pos.x, phaseButton_pos.y, phaseButton_size.x, phaseButton_size.y), phaseButton_text)) 
		{
			AdvanceState();
		}

		if(GUI.Button(new Rect(ResetButton_pos.x, ResetButton_pos.y, ResetButton_size.x, ResetButton_size.y), ResetButton_text)) 
		{
			GameObject.Find("Player").SendMessage("location_reset");
		}
	}

	void playSound_Wave()
	{
		if (snd_wave.length > 0.0f)
		{
			audio.clip = snd_wave;
			audio.Play();
		}
	}

	void playSound_State()
	{
		if (snd_state.length > 0.0f)
		{
			audio.clip = snd_state;
			audio.Play();
		}
	}

	Color color_cycle()
	{
		// cycles text color rapidly.
		// returns appropriately lerped color between current color and target

		Color lerpedColor = Color.white;

		timerTextColor += Time.deltaTime;

		if (timerTextColor >= timerTextColor_rate)
		{
			// time to change colors
			text_colorTarget = new Color(Random.value, Random.value, Random.value);
			timerTextColor = 0.0f; // reset timer. 
		}
		// haven't yet reached target color
		if (text_color != text_colorTarget)
		{
			lerpedColor = Color.Lerp(text_color, text_colorTarget, Time.time);
		}

		return lerpedColor;
	}

	void show_waveText()
	{
		waveText_guiText.guiText.text = "";
		waveText_guiText.guiText.enabled = true;

		LevelText_guiText.guiText.text = "";
		LevelText_guiText.guiText.enabled = true;
	}

	void hide_waveText()
	{
		resetTextTimer();
		waveText_guiText.guiText.text = "";
		waveText_guiText.guiText.enabled = false;

		LevelText_guiText.guiText.text = "";
		LevelText_guiText.guiText.enabled = false;

	}

	void resetTextTimer()
	{
		timerText = 0.0f; // reset timer
	}

	void displayWaveDict()
	{
		string crapstring; 
		
		foreach (KeyValuePair<int, string> pair in wave_names)
		{
			crapstring = "Wave Ready:" + pair.Key + ":" + pair.Value;
			Debug.Log (crapstring);
		}
	}

	void GUI_init()
	{
		screen_size = new Vector2(Screen.width, Screen.height);

		// setup and initialize UI element variables
		menuBox_pos = new Vector2( (screen_size.x / 2) - (menuBox_size.x / 2), (screen_size.y - menuBox_size.y) ); 
		menuBox_Rect = new Rect(menuBox_pos.x , menuBox_pos.y, menuBox_size.x, menuBox_size.y);

		spawnButton_pos = new Vector2((Screen.width / 2) - (spawnButton_size.x/2), Screen.height - spawnButton_size.y);
		phaseButton_pos = new Vector2((Screen.width / 2) - (phaseButton_size.x/2), Screen.height - phaseButton_size.y - spawnButton_size.y - 5);
		ResetButton_pos = new Vector2(0, Screen.height - ResetButton_size.y);
	}

	void PlayerDied()
	{
		// player died! do wave reset things.
		state = wave_spawner_states.player_died; // don't do anything else
		current_wave_script.WaveReset();
	}
}
