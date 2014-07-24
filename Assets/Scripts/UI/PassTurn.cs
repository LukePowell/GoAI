using UnityEngine;
using System.Collections;

public class PassTurn : MonoBehaviour {

	public Board board;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		if(!BoardInfo.Networked)
			board.pass(board.CurrentMoveColor);
		else
			board.networkView.RPC("passInt",RPCMode.All,(board.CurrentMoveColor == Move.Color.E_BLACK ? 1 : 2));
	}
}
