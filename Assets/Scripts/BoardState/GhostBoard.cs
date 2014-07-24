using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostBoard {

	public int[,] stones;
	Move.Color currentMoveColor;
	Move topMove = new Move();//This move has no color, or postion is simply the starting empty point of the board
	public Move currentMove;

	public GhostBoard(Board b)
	{
		stones = new int[BoardInfo.Size,BoardInfo.Size];
		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				stones[i,j] = b.Stones[i,j];
			}
		}
		currentMove = topMove;
	}


	public GhostBoard(GhostBoard b)
	{
		stones = new int[BoardInfo.Size,BoardInfo.Size];
		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				stones[i,j] = b.stones[i,j];
			}
		}
		currentMove = topMove;
	}

	public List<Vector3> getValidMoves()
	{
		List<Vector3> moves = new List<Vector3>();
		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				if(stones[i,j] == 0 && CheckBoard.isMoveValid(j,i,stones,currentMoveColor))
				{
					moves.Add(new Vector3(j, i , 0));
				}
			}
		}
		
		return moves;
	}

	private struct Score
	{
		public float black;
		public float white;
	}
	
	private int scoreSection(int row, int col, int[,] stonesToCount,ref int value,List<Vector2> points)
	{
		if(row < 0 || row >= BoardInfo.Size)
			return 0;
		if(col < 0 || col >= BoardInfo.Size)
			return 0;
		
		int ret;
		
		int color = stonesToCount [row, col];
		if(color != 0)
		{
			if(color == 1)
			{
				value = 1;
			}
			else if(color == 2)
			{
				value = 2;
			}
			return 0;
		}
		
		stonesToCount[row,col] = 3;//Scored
		
		points.Add(new Vector2(col,row));
		
		int sum = 1;
		
		int scoreEast = scoreSection (row - 1, col, stonesToCount,ref value,points);
		int scoreWest = scoreSection (row + 1, col, stonesToCount,ref value,points);
		int scoreNorth = scoreSection (row, col - 1, stonesToCount,ref value,points );
		int scoreSouth = scoreSection (row, col + 1, stonesToCount,ref value,points);
		
		return scoreEast + scoreWest + scoreNorth + scoreSouth + sum;
	}
	
	private void clearMarkers(int[,] stonesToClear)
	{
		for(int i = 0 ; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				if(stonesToClear[i,j] >= 3)
				{
					stonesToClear[i,j] = 0;
				}
			}
		}
	}
	
	private Score score(int[,] stonesToCount)
	{
		Score s = new Score ();
		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j =0; j < BoardInfo.Size; ++j)
			{
				if(stonesToCount[i,j] == 0)
				{
					int value = 0;
					List<Vector2> scoredPoints = new List<Vector2>();
					int score = scoreSection(i,j,stonesToCount,ref value,scoredPoints);
					if(value == 1)//Black
					{
						s.black += score;
					}
					else if(value == 2)//white
					{
						s.white += score;
					}
				}
			}
		}
		
		clearMarkers(stonesToCount);
		s.black += BoardInfo.BlackStonesCaptured;
		s.white += BoardInfo.WhiteStonesCaptured + BoardInfo.Komi;//TOOD: Store komi elsewhere
		return s;
	}

	public float estimateScore(Move.Color color)
	{
		int scored = 0;
		List<Vector2> points = new List<Vector2>();
		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				if(CheckBoard.isEmptyAndSurrounded(color,i,j,stones) && !points.Contains(new Vector2(j,i)))
				{
					List<Vector2> newPoints = new List<Vector2>();
					int value = 0;
					scored += scoreSection(i,j,stones,ref value,newPoints);

					points.AddRange(newPoints);
				}
			}
		}
		return color == Move.Color.E_BLACK ? scored + BoardInfo.BlackStonesCaptured : scored + BoardInfo.WhiteStonesCaptured + BoardInfo.Komi;
	}

	private void clearBoard()
	{
		BoardInfo.BlackStonesCaptured = 0;
		BoardInfo.WhiteStonesCaptured = 0;
		for(int i = 0; i < stones.GetLength(0); ++i)
		{
			for(int j = 0; j < stones.GetLength(1); ++j)
			{
				stones[i,j] =  0;//Simply set all stones to zero will be replacing the board state shortly
			}
		}
	}
	
	private void playMovesInList(List<Move> moves)
	{
		foreach(Move m in moves)
		{
			if(m != topMove)
			{
				makeMove(m.Row,m.Column, false);
			}
		}
	}

	public void playTilMove(Move m)
	{
		if(m == null)
			return;

		//We need q to traverse move m to the beginning.
		List<Move> movesTilTop = new List<Move>();
		while(m != null)
		{
			movesTilTop.Add(m);
			m = m.Parent;
		}
		movesTilTop.Reverse();

		//Now we have the list of moves to make
		clearBoard();//Reset the board to its starting state
		playMovesInList(movesTilTop);
		currentMove = m;
	}

	public void makeMove(int row, int col, bool change = true)
	{
		//Final catach against illegal moves
		if(!CheckBoard.makeMove(col,row,stones,currentMoveColor))
		{
			return;
		}

		if(change)
		{	
			Move m = new Move(currentMoveColor,col,row);
			m.Parent = currentMove;
			currentMove.moves.Add(m);
			currentMove = m;
		}

		currentMoveColor = currentMoveColor == Move.Color.E_BLACK ? Move.Color.E_WHITE : Move.Color.E_BLACK;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
