using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControlPanel_UI : MonoBehaviour {

	public GameObject BuildMenuPanel;
	public GameObject UpgradeMenuPanel;

	public Player PanelOwner;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

		public void OnUpgradeButtonClick()
		{
			// Toggle Upgrade Menu Visibility
			if (UpgradeMenuPanel != null)
			{
				UpgradeMenuPanel.SetActive(!UpgradeMenuPanel.activeSelf);
			}
		}

		public void OnBuildButtonClick()
		{
			// Toggle Upgrade Menu Visibility
			if (BuildMenuPanel != null)
			{
				BuildMenuPanel.SetActive(!BuildMenuPanel.activeSelf);
			}
		}

		public void OnResetButtonClick()
		{
				if (PanelOwner != null)
				{
					PanelOwner.ResetLocation();
				}
		}
}
