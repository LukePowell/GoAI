using UnityEngine;
using System.Collections;

public class BoardInfo : MonoBehaviour {

	public enum GameMode
	{
		E_1_V_1_LOCAL,
		E_1_V_1_NETWORKED,
		E_1_V_AI_RANDOM,
		E_1_V_AI_MONTE_CARLO,
		E_AI_V_AI_RANDOM,
		E_AI_V_AI_MONTE_CARLO,
		E_GTP
	}

	private static int size = 9;
	private static float unit = 1.0f;//Default to one unit is not really usable much of anywhere
	public static GameMode mode = GameMode.E_GTP;
	private static bool networked = false;
	private static bool host = false;
	private static int blackStonesCaptured = 0;
	private static int whiteStonesCaptured = 0;
	private static float komi = 6.5f;

	private static int pushedBlackStonesCaptured;
	private static int pushedWhiteStonesCaptured;

	public static void push()
	{
		pushedBlackStonesCaptured = blackStonesCaptured;
		pushedWhiteStonesCaptured = whiteStonesCaptured;
		blackStonesCaptured = 0;
		whiteStonesCaptured = 0;
	}
	public static void pop()
	{
		blackStonesCaptured = pushedBlackStonesCaptured;
		whiteStonesCaptured = pushedWhiteStonesCaptured;
	}

	public static int BlackStonesCaptured
	{
		get
		{
			return blackStonesCaptured;
		}

		set
		{
			blackStonesCaptured = value;
		}
	}

	public static int WhiteStonesCaptured
	{
		get
		{
			return whiteStonesCaptured;
		}
		
		set
		{
			whiteStonesCaptured = value;
		}
	}
	public static bool Networked
	{
		get
		{
			return networked;
		}
		set
		{
			networked = value;
		}
	}

	public static bool Host
	{
		get
		{
			return host;
		}
		set
		{
			host = value;
		}
	}

	public static int Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}
	public static GameMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			mode = value;
		}
	}

	public static float Unit
	{
		get
		{
			return unit;
		}

		set
		{
			unit = value;
		}
	}

	public static float Komi
	{
		get
		{
			return komi;
		}

		set
		{
			komi = value;
		}

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
