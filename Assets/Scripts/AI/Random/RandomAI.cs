using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomAI : AI 
{
	public override void makeMove (List<Vector3> validMoves)
	{
		if(board.opponentPassed() || validMoves.Count == 0)
		{
			board.pass(color);
			Debug.Log("AI PASSED");
			return;
		}

		int move = Random.Range(0,validMoves.Count-1);
		board.makeMove((int)validMoves[move].y,(int)validMoves[move].x);
	}

	public override List<Vector3> getValidMoves()
	{
		List<Vector3> validMoves = board.getValidMoves();
		List<Vector3> actualValidMoves = new List<Vector3>();

		foreach(Vector3 v in validMoves)
		{
			if(!CheckBoard.isEmptyAndSurrounded(color,(int)v.x,(int)v.y,board.Stones))
			{
				actualValidMoves.Add(v);
			}
		}

		return actualValidMoves;
	}
}
