using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class AI : MonoBehaviour {
	
	public Move.Color color = Move.Color.E_BLACK;
	public bool debugStep = false;

	protected Board board;
	protected bool paused = false;

	void OnOverlayChange(bool visible)
	{
		paused = true;
	}

	// Use this for initialization
	void Start () {
		board = GameObject.FindGameObjectWithTag("board").GetComponent<Board>();
		board.overlayChange += OnOverlayChange;
	}

	void OnGUI()
	{
		if(debugStep && board.CurrentMoveColor == color)
		{
			if(GUILayout.Button("Step"))
			{
				makeMove(getValidMoves());
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if(paused)
			return;


		if(board.CurrentMoveColor == color && !debugStep)
		{
			List<Vector3> validMoves = getValidMoves();
			makeMove(validMoves);
		}
		else if(debugStep && board.CurrentMoveColor == color)
		{
			List<Vector3> validMoves = getValidMoves();
			board.drawValidMoves(validMoves);
		}
	}

	public abstract void makeMove(List<Vector3> validMoves);
	public abstract List<Vector3> getValidMoves();
}
