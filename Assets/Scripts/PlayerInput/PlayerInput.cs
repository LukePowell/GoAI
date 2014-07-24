using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	public GameObject playerCursorWhite;
	public GameObject playerCursorBlack;
	private GameObject playerCursor;
	public GameObject boardObject;
	public Move.Color color;
	private Board board;
	int myPosY;
	static int posY = 0;

	bool isHidden = false;
	GameObject zoomCamera;

	void Start () {
		zoomCamera = GameObject.FindWithTag("zoom");
		myPosY = posY;
		posY++;
		board = GameObject.FindGameObjectWithTag ("board").GetComponent<Board> ();

		if(color == Move.Color.E_BLACK)
		{
			playerCursor = GameObject.Instantiate(playerCursorBlack) as GameObject;
		}
		else
		{
			playerCursor = GameObject.Instantiate(playerCursorWhite) as GameObject;
		}

		board.GetComponent<Board> ().overlayChange += OnOverlayChange;
	}

	void OnOverlayChange(bool change)
	{
		isHidden = change;//change is true when the window is open else it is false.
	}

	void OnGUI()
	{
		return;
		if(isHidden)
			return;

		float width = (int)(0.20f * Screen.width);
		float height = (int)(0.20f * Screen.height);
		if(GUI.Button(new Rect(Screen.width - 2.5f * width,myPosY * height,width,height),"Pass"))
		{
			if(!BoardInfo.Networked)
				board.pass(color);
			else
				board.networkView.RPC("passInt",RPCMode.All,(color == Move.Color.E_BLACK ? 1 : 2));
		}
	}


	// Update is called once per frame
	void Update () {
		if ( color != board.CurrentMoveColor || isHidden)
				return;

#if UNITY_ANDROID || UNITY_IPHONE

		Vector3 newPosition = new Vector3(-1000,-1000);
		if(Input.touches.Length > 0)//The user is touching the screen
		{
		
			newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x,Input.touches[0].position.y,1.0f));

			newPosition.x = Mathf.Round(newPosition.x);
			newPosition.y = Mathf.Round(newPosition.y);

			if(newPosition.x >= (BoardInfo.Size) || newPosition.y >= (BoardInfo.Size ))
			{
				playerCursor.SetActive(false);
				zoomCamera.SetActive(false);
				return;
			}
			else if(newPosition.x < 0|| newPosition.y < 0)
			{
				playerCursor.SetActive(false);
				zoomCamera.SetActive(false);
				return;
			}

			zoomCamera.SetActive(true);
			zoomCamera.GetComponent<Transform>().position = new Vector3(Mathf.Round (newPosition.x),Mathf.Round (newPosition.y),zoomCamera.GetComponent<Transform>().position.z);
		}
		else
		{
			zoomCamera.SetActive(false);
			playerCursor.SetActive(false);
			return;
		}

#else
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,1.0f));
		newPosition.x = Mathf.Round(newPosition.x);
		newPosition.y = Mathf.Round(newPosition.y);
		newPosition.z = 1.0f;
#endif


		if(!board.Scoring)
		{
			playerCursor.transform.position = newPosition;

			if(playerCursor.transform.position.x >= (BoardInfo.Size) || playerCursor.transform.position.y >= (BoardInfo.Size ))
			{
				playerCursor.SetActive(false);
			}
			else if(playerCursor.transform.position.x < 0|| playerCursor.transform.position.y < 0)
			{
				playerCursor.SetActive(false);
			}
			else if(board.occupied((int)newPosition.y,(int)newPosition.x) || !board.isMoveValid((int)newPosition.x,(int)newPosition.y,color))
			{
				playerCursor.SetActive(false);
			}
			else
			{
				if(!playerCursor.activeSelf)
				{
					playerCursor.SetActive(true);
				}
			}

#if UNITY_STANDALONE
			if(Input.GetMouseButtonDown(0) && playerCursor.activeSelf)
			{
				if(!BoardInfo.Networked)
					board.makeMove((int)newPosition.y,(int)newPosition.x);
				else
					board.networkMakeMove((int)newPosition.y,(int)newPosition.x);
			}
#else
			//Mobile
			if(Input.touches.Length > 0 && playerCursor.activeSelf && Input.touches[0].phase == TouchPhase.Ended)
			{	
				if(!BoardInfo.Networked)
					board.makeMove((int)newPosition.y,(int)newPosition.x);
				else
					board.networkMakeMove((int)newPosition.y,(int)newPosition.x);
				zoomCamera.SetActive(false);
			}
#endif
		}
		else//Currently marking stones as dead. This should really only change their material and "remove" them from the board
		{
			if(newPosition.x >= (BoardInfo.Size) || newPosition.y >= (BoardInfo.Size))
			{
				return;
			}
			else if(newPosition.x < 0 || newPosition.y < 0)
			{
				return;
			}
			if(Input.GetMouseButtonDown(0))
			{
				Debug.Log ("Marking group as dead");
				board.markGroupDead((int)newPosition.y,(int)newPosition.x);
			}
		}
	}
}
