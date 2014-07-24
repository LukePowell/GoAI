using UnityEngine;
using System.Collections;

public class SelectAIMode : MonoBehaviour {

	BoardInfo.GameMode gameMode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		BoardInfo.Mode = gameMode;
	}
}

