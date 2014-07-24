using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Simply contains a number of methods to check the state of a board.
//This leaves the board class a bit cleaner. Could simply pass
//down the whole of the board class by reference but simply passing the array gives
//all of the data that is needed to make this class work.
public class CheckBoard : MonoBehaviour {
	//Simple const values to make the code a good deal neater
	const int BLANK = 0;
	const int BLACK = 1;
	const int WHITE = 2;
	const int MARK = 3;
	const int MARK_BLACK = 4;
	const int MARK_WHITE = 5;

	private static bool countCaptures = true;

	public static int enumToInt(Move.Color color)
	{
		return color == Move.Color.E_BLACK ? BLACK : WHITE;
	}

	public static bool isGroupSurroundedHelper(int x, int y, Move.Color color, int[,] board)
	{
		if (y < 0 || y >= BoardInfo.Size || x < 0 || x >= BoardInfo.Size)
			return true;
		
		if(board[y,x] != enumToInt(color))
		{
			if(board[y,x] == BLANK)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		board [y, x] = board [y, x] == BLACK ? MARK_BLACK : MARK_WHITE;
		
		return isGroupSurroundedHelper(x - 1, y, color, board) && isGroupSurroundedHelper(x + 1, y, color, board) && isGroupSurroundedHelper(x, y - 1, color, board) && isGroupSurroundedHelper(x,y+1,color,board);
	}

	public static bool isGroupSurrounded(int x, int y, Move.Color color, int[,] board)
	{
		bool value = isGroupSurroundedHelper (x, y, color, board);
		clearMarks (board);
		return value;
	}

	private static int removeGroupHelper(int x, int y, int target, int[,] board)
	{
		if(x < 0 || x >= BoardInfo.Size || y < 0 || y >= BoardInfo.Size)
		{
			return 0;
		}
		
		int oppposite = target == BLACK ? WHITE : BLACK;
		
		if(board[y,x] == oppposite || board[y,x] == MARK)
			return 0;
		
		board [y, x] = MARK;

		int count = 1;

		count += removeGroupHelper (x + 1, y, target, board);
		count += removeGroupHelper (x - 1, y, target, board);
		count += removeGroupHelper (x, y + 1, target, board);
		count += removeGroupHelper (x, y - 1, target, board);
		return count;
	}

	public static void clearMarks(int[,] board)
	{
		for(int i = 0 ; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				if(board[i,j] >= MARK)
				{
					board[i,j] -= MARK;
				}
			}
		}
	}

    public static void detectAndRemoveDeadGroups(int[,] board, Move.Color moveColor)
    {
        int target = moveColor == Move.Color.E_BLACK ? enumToInt(Move.Color.E_WHITE) : enumToInt(Move.Color.E_BLACK);
        int ignore = enumToInt(moveColor);

        for (int i = 0; i < BoardInfo.Size; ++i)
        {
            for (int j = 0; j < BoardInfo.Size; ++j)
            {
                if (board[i, j] != 0 && board[i, j] != ignore && isGroupSurrounded(j, i, target == 1 ? Move.Color.E_BLACK : Move.Color.E_WHITE, board))
                {
                    int count = removeGroup(j, i, target,board);
					if(countCaptures)
					{
						if(moveColor == Move.Color.E_WHITE)
						{
							BoardInfo.WhiteStonesCaptured += count;
						}
						else
						{
							BoardInfo.BlackStonesCaptured += count;
						}
					}
                }
            }
        }
    }


	public static List<Vector2> findLiberties(Move.Color color, int x, int y, int[,] stones)
	{

		List<Vector2> liberties = new List<Vector2> ();
		if(x < 0 || x >= BoardInfo.Size || y < 0 || y >= BoardInfo.Size)
			return liberties;
		//if we find a stone of the opposite color
		if(stones[y,x] == BLANK)
		{
			stones[y,x] += MARK;
			liberties.Add(new Vector2(x,y));
			return liberties;
		}
		else if(stones[y,x] != enumToInt(color) || stones[y,x] >= MARK)
			return liberties;

		stones[y,x] += MARK;

		//Else we did not find any liberties go the other 4 directions from here
		List<Vector2> east = findLiberties(color,x+1,y,stones);
		List<Vector2> west = findLiberties(color,x-1,y,stones);
		List<Vector2> north = findLiberties(color,x,y+1,stones);
		List<Vector2> south = findLiberties(color,x,y-1,stones);

		liberties.AddRange(east);
		liberties.AddRange(west);
		liberties.AddRange(north);
		liberties.AddRange(south);

		return liberties;
	}

	public static List<Vector2> findMovesOutOfAtari(Move.Color color, int[,] stones)
	{
		List<Vector2> moves = new List<Vector2>();

		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				if(stones[i,j] == enumToInt(color))
				{
					List<Vector2> liberties = findLiberties(color,j,i,stones);
					if(liberties.Count == 1)
					{
						moves.Add(liberties[0]);
					}
				}
			}
		}
		clearMarks(stones);
		return moves;
	}

	static bool stoneFound = false;
	static bool moveOne = true;
	public static bool isEmptyAndSurroundedHelper(Move.Color color, int x, int y, int[,] stones)
	{
		if(moveOne)
		{
			return false;
		}

		if(x < 0 || y < 0 || x >= BoardInfo.Size || y >= BoardInfo.Size)
		{
			return true;
		}

		bool value = false;

		if(stones[y,x] == enumToInt(color) || stones[y,x] == enumToInt(color) + MARK)
		{
			stoneFound = true;
			return true;
		}
		else if(stones[y,x] == 0)
			value = true;
		else if(stones[y,x] == MARK)
			return true;
		else if(stones[y,x] > MARK)
			return false;

		stones[y,x] += MARK;

		value = value && isEmptyAndSurroundedHelper(color,x+1,y,stones);
		value = value && isEmptyAndSurroundedHelper(color,x-1,y,stones);
		value = value && isEmptyAndSurroundedHelper(color,x,y+1,stones);
		value = value && isEmptyAndSurroundedHelper(color,x,y-1,stones);
		return value;
	}

	public static bool isEmptyAndSurrounded(Move.Color color, int x, int y, int[,] stones)
	{
		stoneFound = false;
		bool value = isEmptyAndSurroundedHelper(color,x,y,stones);
		clearMarks(stones);
		value = value && stoneFound;
		return value;
	}

	public static int removeGroup(int x, int y, int target, int[,] board)
	{
		int count = removeGroupHelper (x, y, target, board);
		clearMarks (board);
		return count;
	}
	
	public static bool isMoveValid(int x, int y, int[,] board, Move.Color moveColor)
	{
		countCaptures = false;

		if (x < 0 || x >= BoardInfo.Size || y < 0 || y >= BoardInfo.Size)
			return false;
		if (board [y, x] != BLANK)
			return false;

		//NEED TO COPY THE BOARD! TO USE MAKE MOVE
		int[,] copy = new int[BoardInfo.Size, BoardInfo.Size];

		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				copy[i,j] = board[i,j];
			}
		}

		bool valid = makeMove(x,y,copy,moveColor);
		countCaptures = true;
		return valid;
	}

	//Returns false if a move is not valid
	public static bool makeMove(int x, int y, int[,] board, Move.Color moveColor)
	{
		int color = enumToInt (moveColor);

        board[y, x] = enumToInt(moveColor);

        detectAndRemoveDeadGroups(board,moveColor);

        if(isGroupSurrounded(x,y,moveColor,board))
        {
          board[y,x] = 0;//Cannot place a stone there
          return false;
        }
		moveOne = false;
        return true;
	}
}
