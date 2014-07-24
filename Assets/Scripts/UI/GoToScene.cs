using UnityEngine;
using System.Collections;

public class GoToScene : MonoBehaviour {

	public int scene = 1;
	bool goToScene = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(goToScene)
		{
			Application.LoadLevel(scene);
		}
	}

	void OnClick()
	{
		goToScene = true;
	}
}
