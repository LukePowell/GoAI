using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A single node in what is essentially a list of moves
/// </summary>
public class Move
{
	public GameObject visual;
	//ArrayList<Comment> comments;//The array of commments on this node
	List<string> comments = new List<string>();
	public List<Move> moves =new List<Move>();

	public void addComment(string comment, string userName)
	{
		comments.Add(userName + ": " + comment);
	}

	public List<string> Comments 
	{
		get
		{
			return comments;
		}
	}

	public enum Color
	{
		E_WHITE,
		E_BLACK,
	}

	private Color moveColor;

	public Color MoveColor 
	{
		get
		{
			return moveColor;
		}

		set
		{
			moveColor = value;
		}
	}

	public GameObject Visual
	{
		get
		{
			return visual;
		}
		set
		{
			visual = value;
		}
	}

	/// <summary>
	/// The variants steming from this move
	/// </summary>
	List<Move> variants = new List<Move>();

	Move next;//The next move in the current level of variants null if this is the last move
	Move parent;//The move prior to this one.

	bool placed = false;
	int row = 0;
	int col = 0;

	public Move Next
	{
		get
		{
			return next;
		}

		set
		{
			next = value;
		}
	}

	public int Row
	{
		get
		{
			return row;
		}
		set
		{
			row = value;
		}
	}

	public int Column
	{
		get
		{
			return col;
		}
		set
		{
			col = value;
		}
	}

	public bool Placed
	{
		get
		{
			return placed;
		}
		set
		{
			placed = value;
		}
	}

	public Move Parent
	{
		get
		{
			return parent;
		}

		set
		{
			parent = value;
		}
	}

	public void addVariant(Move m)
	{
		variants.Add (m);
	}

	public List<Move> Variants
	{
		get
		{
			return variants;
		}
	}

	public Move()
	{
	}

	public Move(Color color, int column, int row)
	{
		moveColor = color;
		this.row = row;
		this.col = column;
		placed = true;
	}
}
