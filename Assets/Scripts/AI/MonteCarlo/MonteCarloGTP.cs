using UnityEngine;
using System.Collections;

public class MonteCarloGTP : MonteCarloAI {

	public bool genMove(ref Vector2 move, Move.Color color)
	{
		//Change our color to reflect the requested generation
		this.color = color;
		return base.genMove(ref move);
	}

	public override void makeMove (System.Collections.Generic.List<Vector3> validMoves)
	{
	}
}
