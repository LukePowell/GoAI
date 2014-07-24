using UnityEngine;
using System.Collections;

public class CreateBoard : MonoBehaviour {

	public GameObject line;
	public GameObject starPoint;
	public GameObject randomAI;
	public GameObject monteCarloAI;
	public GameObject playerController;
	public GameObject serverPrefab;
	public GameObject clientPrefab;


	public Vector2[] starPoints9_9;
	public Vector2[] starPoints13_13;
	public Vector2[] starPoints19_19;

	// Use this for initialization
	public void Start () {

		switch(BoardInfo.Mode)
		{
		case BoardInfo.GameMode.E_1_V_1_LOCAL:
			GameObject playerOne = GameObject.Instantiate(playerController) as GameObject;
			GameObject playerTwo = GameObject.Instantiate(playerController) as GameObject;
			playerTwo.GetComponent<PlayerInput>().color = Move.Color.E_BLACK;
			break;
		case BoardInfo.GameMode.E_1_V_1_NETWORKED:
			GameObject networkedPlayer = GameObject.Instantiate(playerController) as GameObject;
			networkedPlayer.GetComponent<PlayerInput>().color = BoardInfo.Host ? Move.Color.E_BLACK : Move.Color.E_WHITE;
			break;
		case BoardInfo.GameMode.E_1_V_AI_MONTE_CARLO:
			GameObject.Instantiate(playerController);
			GameObject.Instantiate(monteCarloAI);
			break;
		case BoardInfo.GameMode.E_1_V_AI_RANDOM:
			GameObject.Instantiate(playerController);
			GameObject.Instantiate(randomAI);
			break;
		case BoardInfo.GameMode.E_AI_V_AI_MONTE_CARLO:
			GameObject.Instantiate(monteCarloAI);//Black
			GameObject whiteMonte = GameObject.Instantiate(monteCarloAI) as GameObject;
			whiteMonte.GetComponent<AI>().color = Move.Color.E_WHITE;
			break;
		case BoardInfo.GameMode.E_AI_V_AI_RANDOM:
			GameObject.Instantiate(randomAI);//Black
			GameObject white = GameObject.Instantiate(randomAI) as GameObject;
			white.GetComponent<RandomAI>().color = Move.Color.E_WHITE;
			break;
		case BoardInfo.GameMode.E_GTP://We should have a GTP listener and that is all
			break;
		}

		Debug.Log("Playing on a " + BoardInfo.Size + "x" + BoardInfo.Size + " board.");

		int posY = 0;//-BoardInfo.Size / 2;
		int posX = 0;//-BoardInfo.Size / 2;

		for(int y = 0; y < BoardInfo.Size; ++y)
		{
			GameObject obj = GameObject.Instantiate(line,new Vector3(BoardInfo.Size / 2,posY),Quaternion.AngleAxis(90.0f,new Vector3(0.0f,0.0f,1.0f))) as GameObject;
			obj.transform.localScale = new Vector3(0.10f,BoardInfo.Size - 1,0.1f);
			posY++;
		}
		for(int x = 0; x < BoardInfo.Size; ++x)
		{
			GameObject obj = GameObject.Instantiate(line,new Vector3(posX,BoardInfo.Size / 2),Quaternion.identity) as GameObject;
			obj.transform.localScale = new Vector3(0.10f,BoardInfo.Size - 1,0.1f);
			posX++;
		}

		GameObject.FindGameObjectWithTag("background").transform.localScale = new Vector3(BoardInfo.Size+1,BoardInfo.Size+1,1.0f);

		//Set up the size of the orthographic camera to minimize ammount of wasted space
		switch(BoardInfo.Size)
		{
		case 9:
			Camera.main.orthographicSize = 5;
			foreach(Vector2 v in starPoints9_9)
			{
				GameObject obj = GameObject.Instantiate(starPoint,new Vector3(v.x,v.y,0.0f),Quaternion.Euler(new Vector3(90,0,0))) as GameObject;
			}
			break;
		case 13:
			Camera.main.orthographicSize = 7;
			foreach(Vector2 v in starPoints13_13)
			{
				GameObject obj = GameObject.Instantiate(starPoint,new Vector3(v.x,v.y,0.0f),Quaternion.Euler(new Vector3(90,0,0))) as GameObject;
			}
			break;
		case 19:
			Camera.main.orthographicSize = 10;
			foreach(Vector2 v in starPoints19_19)
			{
				GameObject obj = GameObject.Instantiate(starPoint,new Vector3(v.x,v.y,0.0f),Quaternion.Euler(new Vector3(90,0,0))	) as GameObject;
			}
			break;
		}

		if(BoardInfo.Networked)
		{
			//We need to set up networking
			if(BoardInfo.Host)
			{
				GameObject.Instantiate(serverPrefab);
			}
			else
			{
				GameObject.Instantiate(clientPrefab);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
