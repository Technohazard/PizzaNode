using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPanel_UI : MonoBehaviour {

/// <summary>
/// The Camera_Controller that owns this panel.
/// </summary>
public Camera_Controller ControllerOwner = null;

public Text CameraModeText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnChangeCameraModeButtonClick()
	{
		if (ControllerOwner == null)
			return;

		ControllerOwner.ChangeCameraMode();
	}

	public void SetText(string toSet)
	{
		if (CameraModeText == null)
		return;

		CameraModeText.text = toSet;
	}
}
