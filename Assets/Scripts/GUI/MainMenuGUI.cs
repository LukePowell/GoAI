using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	private enum MenuState
	{
		E_CHOOSE_MODE,
		E_CHOOSE_SIZE,
		E_CHOOSE_1_V_1_MODE,
		E_CHOOSE_HOST_OR_SERVER,
		E_CHOOSE_SERVER,
		E_CHOOSE_AI_V_AI_MODE,
		E_CHOSE_1_V_AI_MODE
	}
	MenuState currentState = MenuState.E_CHOOSE_MODE;
	HostData[] hosts;
	Vector2 scrollPostion = new Vector2();
	// Use this for initialization
	void Start () {
	
	}

	void Awake()
	{
		MasterServer.RequestHostList("GoMatch");
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		float width = (int)(0.80f * Screen.width);
		float height = (int)(0.20f * Screen.height);


		switch(currentState)
		{
		case MenuState.E_CHOOSE_MODE:
			GUI.Label(new Rect(width/8,Screen.height / 2 - (2 * height),width,height),"Choose Mode:");
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - (height + height / 2),width,height),"1v1"))
			{
				currentState = MenuState.E_CHOOSE_1_V_1_MODE;
				BoardInfo.Mode = BoardInfo.GameMode.E_1_V_1_LOCAL;
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - height/2,width,height),"1vAI"))
			{
				currentState = MenuState.E_CHOSE_1_V_AI_MODE;
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 +  height / 2,width,height),"AIvAI"))
			{
				currentState = MenuState.E_CHOOSE_AI_V_AI_MODE;
			}
			break;
		case MenuState.E_CHOOSE_1_V_1_MODE:
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - height / 2,width,height),"Local"))
			{
				currentState = MenuState.E_CHOOSE_SIZE;
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 + height / 2,width,height),"Networked"))
			{
				currentState = MenuState.E_CHOOSE_HOST_OR_SERVER;
				BoardInfo.Mode = BoardInfo.GameMode.E_1_V_1_NETWORKED;
				BoardInfo.Networked = true;
			}
			break;
		case MenuState.E_CHOOSE_HOST_OR_SERVER:
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - height / 2,width,height),"Host Server"))
			{
				currentState = MenuState.E_CHOOSE_SIZE;
				BoardInfo.Host = true;
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 + height / 2,width,height),"Find Game"))
			{
				hosts = MasterServer.PollHostList();
				BoardInfo.Host = false;
				currentState = MenuState.E_CHOOSE_SERVER;
			}
			break;
		case MenuState.E_CHOOSE_SERVER:
			//Set up master server info here
			GUILayout.BeginScrollView(scrollPostion);
			foreach(HostData host in hosts)
			{
				if(GUILayout.Button(host.gameName))
				{
					BoardInfo.Size = int.Parse(host.gameName.Substring(0,2));
					Network.Connect(host);
					Application.LoadLevel(1);
				}
			}
			GUILayout.EndScrollView();
			break;
		case MenuState.E_CHOOSE_AI_V_AI_MODE:
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - height / 2,width,height),"Random AI"))
			{
				currentState = MenuState.E_CHOOSE_SIZE;
				BoardInfo.Mode = BoardInfo.GameMode.E_AI_V_AI_RANDOM;
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 + height / 2,width,height),"Monte Carlo AI"))
			{
				currentState = MenuState.E_CHOOSE_SIZE;
				BoardInfo.Mode = BoardInfo.GameMode.E_AI_V_AI_MONTE_CARLO;
			}
			break;
		case MenuState.E_CHOSE_1_V_AI_MODE:
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - height / 2,width,height),"Random AI"))
			{
				currentState = MenuState.E_CHOOSE_SIZE;
				BoardInfo.Mode = BoardInfo.GameMode.E_1_V_AI_RANDOM;
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 + height / 2,width,height),"Monte Carlo AI"))
			{
				currentState = MenuState.E_CHOOSE_SIZE;
				BoardInfo.Mode = BoardInfo.GameMode.E_AI_V_AI_MONTE_CARLO;
			}
			break;
		case MenuState.E_CHOOSE_SIZE:
			GUI.Label(new Rect(width/8,Screen.height / 2 - (2 * height),width,height),"Choose Board Size:");
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - (height + height / 2),width,height),"9x9"))
			{
				BoardInfo.Size = 9;
				Application.LoadLevel(1);
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 - height/2,width,height),"13x13"))
			{
				BoardInfo.Size = 13;
				Application.LoadLevel(1);
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 +  height / 2,width,height),"19x19"))
			{
				BoardInfo.Size = 19;
				Application.LoadLevel(1);
			}
			if(GUI.Button(new Rect(width/8,Screen.height / 2 +  ((3 * height) / 2),width,height),"Back"))
			{
				currentState = MenuState.E_CHOOSE_MODE;
			}
			break;
		}
	}
}