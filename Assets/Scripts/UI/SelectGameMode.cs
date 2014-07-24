using UnityEngine;
using System.Collections;

public class SelectGameMode : MonoBehaviour {
	
	public BoardInfo.GameMode gameMode = BoardInfo.GameMode.E_1_V_1_LOCAL;
	
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
