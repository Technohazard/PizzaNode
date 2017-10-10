using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour 
{
		public string AssetBundlePath = "Assets/AssetBundles/scenes";
		private AssetBundle _LoadedAssetBundle;

		public string[] scenePaths;
		public bool LoadPathsFromAssetBundle = false;

	// Use this for initialization
	void Start() 
	{
		// Create UI elements 
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Initialize()
	{
		if (LoadPathsFromAssetBundle)
		{
			LoadScenePathsFromAssetBundle(AssetBundlePath);
		}
	}

	public void LoadScenePathsFromAssetBundle(string bundlePath)
	{
		Debug.Log(string.Format("Loading Scene Paths from Asset Bundle '{0}'", bundlePath));

		_LoadedAssetBundle = AssetBundle.LoadFromFile(bundlePath);
		scenePaths = _LoadedAssetBundle.GetAllScenePaths();

		Debug.Log(string.Format("Loaded {0} Scene Paths", scenePaths.Length));
	}

	/// <summary>
	/// Loads the level by name <param name="scenePath">
	/// </summary>
	/// <param name="scenePath">Scene path.</param>
	public void OnLoadLevelButtonClick(string scenePath)
	{
		LoadLevel(scenePath, LoadSceneMode.Single);
	}
			
	/// <summary>
	/// Loads the level by name <param name="scenePath">
	/// </summary>
	/// <param name="scenePath">Scene path.</param>
	public void LoadLevel(string scenePath, LoadSceneMode sceneMode = LoadSceneMode.Additive)
	{
		Debug.Log(string.Format("<color=green>Scene '{0} loading...</color>", scenePath));
		SceneManager.LoadScene(scenePath, sceneMode);
	}

	/// <summary>
	/// Loads the level index <param name="sceneIndex">
	/// </summary>
	/// <param name="sceneIndex">Scene index.</param>
	public void LoadLevel(int sceneIndex, LoadSceneMode sceneMode = LoadSceneMode.Additive)
	{
		Debug.Log(string.Format("<color=green>Loading Scene by Index: {0}</color>", sceneIndex));
		LoadLevel(scenePaths[sceneIndex], sceneMode);
	}
}
