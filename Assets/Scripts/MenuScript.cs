using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour 
{
	public Texture btnTexture;
	public Texture menuBoxTexture;

	public float buttonSpacing = 40.0f;

	public string boxMenuTitle = "Main Menu: ";
	private Vector2 boxMenuSize;
	private Vector2 boxMenuPos;
	private Rect boxRect;

	public string buttonLabel = "Start!";
	private Vector2 buttonMenuSize;
	private Vector2 buttonMenuPos;
	private Rect buttonRectStart;

	public string buttonLv1Label = "Lv. 01";
	private Vector2 buttonLv1Size;
	private Vector2 buttonLv1Pos;
	private Rect buttonRectLv1;

	public string buttonLv2Label = "Lv. 02";
	private Vector2 buttonLv2Size;
	private Vector2 buttonLv2Pos;
	private Rect buttonRectLv2;

	// Use this for initialization
	void Start() 
	{
		if (!btnTexture) {
			Debug.LogError(gameObject.name.ToString() + ": Please assign a button texture on the inspector");
			return;
		}
		if (!menuBoxTexture) {
			Debug.LogError(gameObject.name.ToString() + ": Please assign a menubox texture on the inspector");
			return;
		}

		// Create UI elements for 
		InitGUI();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitGUI()
	{
		// Initialize GUI element coordinates
		// menu box (center)
		boxMenuSize = new Vector2 (120, 180);
		boxMenuPos = new Vector2(((Screen.width / 2) - boxMenuSize.x / 2), (Screen.height / 2));
		Debug.Log("Box:" + boxMenuPos.ToString() + " Size: " + boxMenuSize.ToString());

		// menu button (center)
		buttonMenuSize = new Vector2(80, 20); 
		buttonMenuPos = new Vector2(boxMenuPos.x + (boxMenuSize.x / 2) - (buttonMenuSize.x / 2), boxMenuPos.y + buttonSpacing);
		Debug.Log("Start:" + buttonMenuPos.ToString() + " Size: " + buttonMenuSize.ToString());

		buttonLv1Size = new Vector2(80, 20); 
		buttonLv1Pos = new Vector2(0, buttonSpacing) + buttonMenuPos;
		Debug.Log("Lv1:" + buttonLv1Pos.ToString() + " Size: " + buttonLv1Size.ToString());

		buttonLv2Size = new Vector2(80, 20); 
		buttonLv2Pos =  new Vector2(0, buttonSpacing) + buttonLv1Pos;
		Debug.Log("Lv2:" + buttonLv2Pos.ToString() + " Size: " + buttonLv2Size.ToString());

		// Initialize Rects for use in OnGUI objects
		boxRect = CreatButtonRect(boxMenuPos, boxMenuSize);
		buttonRectStart = CreatButtonRect(buttonMenuPos, buttonMenuSize);
		buttonRectLv1 = CreatButtonRect(buttonLv1Pos, buttonLv1Size);
		buttonRectLv2 = CreatButtonRect(buttonLv2Pos, buttonLv2Size);
	}


	void OnGUI()
	{

		
		// Determine the button's place on screen
		// Center in X, 2/3 of the height in Y
		GUI.Box(boxRect, boxMenuTitle);

		// Draw/Update the Start Button
		if(GUI.Button(buttonRectStart, buttonLabel))
		{
			// Load the first scene FOR NOW! Should continue at most recent level.
			Application.LoadLevel("Level_01");
		}

		// Draw/Update Level 02 Button
		if(GUI.Button(buttonRectLv1, buttonLv1Label))
		{
			// Load the first scene.
			Application.LoadLevel("Level_01");
		}

		if(GUI.Button(buttonRectLv2, buttonLv2Label))
		{
			// Load the first scene.
			Application.LoadLevel("Level_02");
		}


		/*

		if (GUI.Button(new Rect(10, 10, 50, 50), btnTexture))
			Debug.Log("Clicked the button with an image");
		
		if (GUI.Button(new Rect(10, 70, 50, 30), "Text Button"))
			Debug.Log("Clicked the button with text");
			
		*/
	}

	Rect CreatButtonRect(Vector2 buttonSize, Vector2 buttonPos )
	{
		Rect tempRect = new Rect(
			buttonSize.x,
			buttonSize.y,
			buttonPos.x,
			buttonPos.y
			);

		return tempRect;
	}
}
