using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class MonteCarloAI : AI
{
	public int startingMoves = 10;//Choose ten random moves
	public int depth = 10;//Go down each path depth moves
	public float nodeCount = 0;
	private bool thinking = false;

	private Thread thinkingThread;

	private struct TreePath
	{
		public float value;
		public Vector3 validMove;
	}

	public UILabel progress;


	private System.Random nonUnityRandomDueToThreading = new System.Random();
	private TreePath MonteCarloBranching(int depth, GhostBoard board)
	{
		if(depth == 0 || board.getValidMoves().Count == 0)
		{
			//Estimate the score
			TreePath p = new TreePath();
	
			p.value = board.estimateScore(color) - board.estimateScore(color == Move.Color.E_BLACK ? Move.Color.E_WHITE : Move.Color.E_BLACK);;
			nodeCount++;
			return p;
		}

		TreePath[] paths = new TreePath[startingMoves];
		List<Vector2> movesOutOfAtari = CheckBoard.findMovesOutOfAtari(color,board.stones);
		List<Vector2> opponentInAtari = CheckBoard.findMovesOutOfAtari(color == Move.Color.E_BLACK ? Move.Color.E_WHITE : Move.Color.E_BLACK,board.stones);
		List<Vector3> validMoves = new List<Vector3>();

		if(movesOutOfAtari.Count != 0)
		{
			Debug.Log("I am in atari");
			Move curent = board.currentMove;
			foreach(Vector2 v in movesOutOfAtari)
			{
				if(CheckBoard.isMoveValid((int)v.x,(int)v.y,board.stones,color))
				{
					validMoves.Add(new Vector3(v.x,v.y));
				}
			}

			if(validMoves.Count == 0)
			{
				validMoves = board.getValidMoves();
			}
		}
		else if(opponentInAtari.Count != 0)
		{
			Debug.Log("Oponent in atari");
			Move curent = board.currentMove;
			foreach(Vector2 v in opponentInAtari)
			{
				if(CheckBoard.isMoveValid((int)v.x,(int)v.y,board.stones,color))
				{
					validMoves.Add(new Vector3(v.x,v.y));
				}
			}
			
			if(validMoves.Count == 0)
			{
				validMoves = board.getValidMoves();
			}
		}
		else
		{	
			validMoves = board.getValidMoves();
		}


		Move current = board.currentMove;
		for(int i = 0; i < startingMoves; ++i)
		{
			BoardInfo.push();
			int move = (int)(nonUnityRandomDueToThreading.NextDouble() * (validMoves.Count - 1));
			GhostBoard g = new GhostBoard(board);
			g.makeMove((int)validMoves[move].y,(int)validMoves[move].x);
			paths[i].validMove = validMoves[move];
			paths[i].value = MonteCarloBranching(depth-1,g).value;
			board.playTilMove(current);//Reverse this move
			BoardInfo.pop();
		}

		TreePath max = paths[0];
		foreach(TreePath tp in paths)
		{
			if(tp.value > max.value)
			{
				max = tp;
			}
		}
		return max;
	}

	private float count(int depth, int branches)
	{
		if(depth == 1)
			return branches;
		else
			return Mathf.Pow(branches,depth) + count (depth-1,branches);
	}

	TreePath max;
	

	private void startThinking()
	{
		max = MonteCarloBranching(depth,new GhostBoard(board));
	}

	protected bool genMove(ref Vector2 move)
	{
		if(board.opponentPassed())
		{
			board.pass(color);
			Debug.Log("AI PASSED");
			return false;
		}
		
		if(thinking)
		{
			if(thinkingThread.Join(TimeSpan.FromSeconds(0.0016)))
			{
				thinking = false;
				move.x = max.validMove.x;
				move.y = max.validMove.y;
				thinkingThread = null;
				return true;
			}
			return false;
		}
		
		nodeCount = 0;
		thinkingThread = new Thread(this.startThinking);
		thinkingThread.Start();
		thinking = true;
		return false;
	}

	public override void makeMove (List<Vector3> validMoves)
	{
		Vector2 move = new Vector2();
		if(genMove(ref move))
		{
			board.makeMove((int)move.y,(int)move.x);
		}

		//TreePath max = MonteCarloBranching(depth,new GhostBoard(board));
		//if(toBeat <= max.value)
		//else
			//board.pass(color);
	}
	
	public override List<Vector3> getValidMoves()
	{
		return board.getValidMoves();
	}
}
