using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

/// <summary>
/// Cache a list of prefabs
/// maintains a dict of prefabs keyed with prefab name.
/// </summary>
public class PrefabCache
{

		private Dictionary<string, GameObject> CacheDict = new Dictionary<string, GameObject>();

		/// <summary>
		/// Add a path to the known prefabs path lists
		/// Attempts to instantiate prefab and cache it.
		/// </summary>
		/// <param name="toAdd">To add.</param>
		public void AddPath(string toAdd)
		{
				if (string.IsNullOrEmpty (toAdd))
				{
						return;
				}

				if (!CacheDict.ContainsKey (toAdd))
						{
						GameObject newObj = LoadPrefab (toAdd);

						if (newObj != null)
							{
									// add new entry.
									CacheDict.Add (toAdd, newObj);
							}	
						}
						else
						{
							// TODO: refresh cache.
							Debug.Log(string.Format("Cache already contains {0} but you're trying to load it.", toAdd));
						}
		}

		/// <summary>
		/// Hit the prefab cache for object
		/// </summary>
		/// <param name="toGet">To get.</param>
		public GameObject GetPrefab(string toGet)
		{
				GameObject gameObj;

				if (CacheDict.TryGetValue (toGet, out gameObj))
				{
						if (gameObj != null)
						{
								GameObject instance;

								try
								{
									instance = GameObject.Instantiate(gameObj);
								}
								catch (Exception ex)
								{
										Debug.Log (string.Format("Error instantiating gameObject '{0}' (e = {1})", toGet, ex.ToString ()));
										instance = null;
								}

								return instance;
						}

						return null;
				}
				else
				{
					// try to load in place
						gameObj = LoadPrefab(toGet);
						if (gameObj != null)
						{
								Debug.Log (string.Format ("Successfully loaded '{0}' in place.", toGet));
								return gameObj;
						}

						// Failed load.
						Debug.Log(string.Format("Cache miss! Failed to load '{0}'", toGet));
						return null;
				}
		}

		/// <summary>
		/// Use Resource.Load to load prefab.
		/// </summary>
		/// <returns>The prefab.</returns>
		/// <param name="toGet">To get.</param>
		private GameObject LoadPrefab(string toGet)
		{
				GameObject newObj;

				newObj = Resources.Load(toGet) as GameObject;

				if (newObj != null)
				{
						Debug.Log (string.Format ("Resource Loaded: '{0}'", toGet));
						return newObj;
				}

				Debug.Log (string.Format ("Failed to Load: '{0}' from Resources", toGet));
				return null;
		}
}

