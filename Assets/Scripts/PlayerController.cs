using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Manage all Players in game, keeps track of objects tagged "Player".
/// can spawn, despawn players.
/// </summary>
public class PlayerController : MonoBehaviour {

	private List<GameObject> PlayerList = new List<GameObject>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Refresh the player list.
	/// clear + include all objects w/ player
	/// </summary>
	public void RefreshPlayerList(bool clearList = true)
	{
		// stub

		if ((PlayerList.Count > 0)&&(clearList == true))
		{
			PlayerList.Clear();
		}

		// add all in-scene players to list.
		GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
		GameObject tmpObj = null;

		foreach (GameObject go in currentPlayers)
		{
			tmpObj = FindObjectRoot(go);
			PlayerList.Add(tmpObj);
		}

	}

	/// <summary>
	/// Finds the object root.
	/// </summary>
	/// <returns>The object root.</returns>
	/// <param name="go">GameObject to parse</param>
	private GameObject FindObjectRoot(GameObject go)
	{
		GameObject tmpObj = null;

		tmpObj = go;
		while ((tmpObj.transform.parent.CompareTag("Player"))&&(tmpObj.transform.parent != null))
		{
			tmpObj = tmpObj.transform.parent.gameObject;
		}

		return tmpObj;
	}
}
