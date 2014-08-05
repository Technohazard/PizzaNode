using UnityEngine;
using System.Collections;

// reference : http://www.indiedb.com/groups/unity-devs/tutorials/delegates-events-and-singletons-with-unity3d-c


public class Game_Manager : MonoBehaviour
{
	// Singleton
	private static Game_Manager instance;

	// Variable
	public string myGlobalVar = "Game_Manager Object String!";


	// Construct  
	private Game_Manager()
	{
	}

	//Instance
	public static Game_Manager Instance
	{

		get     
		{       
			if (instance ==  null)
				instance = GameObject.FindObjectOfType(typeof(Game_Manager)) as  Game_Manager;      
			return instance;    
		}   
	} // end Instance

	// Do something here, make sure this  is public so we can access it through our Instance.   
	public void DoSomething() 
	{ 
		Debug.Log("Called DoSomething() from the Game_Manager!");
	}  
	
}