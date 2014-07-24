using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Board.
/// </summary>
/// 
public class Board : MonoBehaviour {
	public GameObject whiteStone;
	public GameObject blackStone;
	public GameObject debugThing;
	public GameObject lastMove;

	public bool debug = false;
	public UILabel whiteCapturesDisplay;
	public UILabel blackCapturesDisplay;

	public GameObject blackScoreCube;
	public GameObject whiteScoreCube;

	public int blackCaptures;
	public int whiteCaptures;

	private bool showGameTree;
	private Move.Color currentMoveColor = Move.Color.E_BLACK;//Black always goes first

	public delegate void onOverlayChange (bool overlay);
	public onOverlayChange overlayChange;

	public Move.Color CurrentMoveColor
	{
		get
		{
			return currentMoveColor;
		}
	}

	Vector2 gameViewScroll = new Vector2();

	int[,] stones;

	Move root = null;
	Move currentMoveRef = null;

	Vector3 ko = new Vector3(-1,-1);

	void clearStones()
	{
		for(int i = 0; i < stones.GetLength(0); ++i)
		{
			for(int j = 0; j < stones.GetLength(1); ++j)
			{
				stones[i,j] =  0;//Simply set all stones to zero will be replacing the board state shortly
			}
		}
	}

	void playTilMove(Move m)
	{
		BoardInfo.BlackStonesCaptured = 0;
		BoardInfo.WhiteStonesCaptured = 0;

		List<Move> movesTilMove = new List<Move> ();

		while(m.Parent != null)
		{
			movesTilMove.Add(m);
			m = m.Parent;
		}
		movesTilMove.Add (m);
	
		for(int i = movesTilMove.Count-1; i >= 0; --i)
		{
			CheckBoard.makeMove(movesTilMove[i].Column,movesTilMove[i].Row,stones,movesTilMove[i].MoveColor);
			movesTilMove[i].Visual.SetActive(true);
			removeRemovedStonesVisuals();
		}
	}

	void gotoMove(Move m)
	{
		currentMoveRef = m;
		
		GameObject[] gameObj = GameObject.FindGameObjectsWithTag("stone");
		
		foreach(GameObject obj in gameObj)
		{
			obj.SetActive(false);
		}
		
		Move p = currentMoveRef;
		while(p != null)
		{
			p.Visual.SetActive(true);
			p = p.Parent;
		}
		
		currentMoveColor = currentMoveRef.MoveColor == Move.Color.E_WHITE ? Move.Color.E_BLACK : Move.Color.E_WHITE;
		clearStones();
		playTilMove(currentMoveRef);
	}
	
	void gameTreeWindow(int message)
	{
		int height = (int)(0.20f * Screen.height);
		gameViewScroll = GUI.BeginScrollView(new Rect(0,20,Screen.width,Screen.height-(height + 20)),gameViewScroll,new Rect(0,0,moves.Count * 100,Screen.height));
		GUILayout.BeginHorizontal();

		Move iter = root;

		int i = 0;
		float buttonWidth = 100.0f;
		float buttonHeight = 50.0f;
		while(iter.Next != null)
		{
			if(iter == currentMoveRef)
			{
				int j = 0;
				foreach(Move v in currentMoveRef.Variants)
				{
					if(GUI.Button (new Rect((i+1)*buttonWidth+15,buttonHeight+buttonHeight*j+10,buttonWidth,buttonHeight),(v.MoveColor == Move.Color.E_BLACK ? "Black: " : "White: ") + v.Row + " " + v.Column))
					{
						gotoMove(v);
					}
				}
			}

			if(GUI.Button(new Rect(i*buttonWidth + 10,0,buttonWidth,buttonHeight),(iter.MoveColor == Move.Color.E_BLACK ? "Black: " : "White: ") + iter.Row + " " + iter.Column))
			{		
				gotoMove(iter);
			}
			iter = iter.Next;
			++i;
		}


		GUILayout.EndHorizontal();
		GUI.EndScrollView();

		if(GUI.Button(new Rect(0,Screen.height - height,Screen.width,height),"Dismiss"))
		{
			overlayChange(false);
			showGameTree = false;
		}
	}

	public void saveToSGFFile(string filename)
	{
		SGFParser.saveSGFFile(filename,moves);
	}

	/// <summary>
	/// The size of the board assumes square boards.
	/// </summary>
	int size;

	/// <summary>
	/// The moves.
	/// </summary>
	List<Move> moves = new List<Move>();

	int currentMove = 0;

	bool scoring = false;

	public bool Scoring 
	{
		get
		{
			return scoring;
		}
	}

	// Use this for initialization
	void Start () {
		currentMoveRef = new Move ();
		moves.Add(currentMoveRef);
		root = currentMoveRef;
		stones = new int[BoardInfo.Size,BoardInfo.Size];
	}

	/// <summary>
	/// Resets the board to a clean state, with the current board size
	/// this can come after the clear_board gtp command
	/// </summary>
	public void clear()
	{
		moves.Clear();
		Start();
	}

	// Update is called once per frame
	void Update () {
		//Check for user input,
		if(whitePassed && blackPassed)//We can go into scoring the game now
		{
			scoring = true;
		}

		if(scoring)
		{
			Score s = score (stones);

			GameObject whiteScore = GameObject.FindWithTag("whitescore");
			GameObject blackScore = GameObject.FindWithTag("blackscore");

			whiteScore.GetComponent<UILabel>().text = s.white.ToString();
			blackScore.GetComponent<UILabel>().text = s.black.ToString();
		}

		blackCapturesDisplay.text = BoardInfo.BlackStonesCaptured.ToString();
		whiteCapturesDisplay.text = BoardInfo.WhiteStonesCaptured.ToString();
	}
	
	//The key here is to display enough data to demonstrate that the AI is in working order load and save
	//to an SGF file in a convinent manner for diagnostic reasons. And to allow the display of a tree view of the 
	//game and all the variant moves that the AI has made assuming that only the moves in the array list stored
	//here are the actual game and all lower nodes are only a variant.
	void OnGUI()
	{
		return;
		float width = (int)(0.30f * Screen.width);
		float height = (int)(0.20f * Screen.height);

		if(scoring)
		{
			Score s = score (stones);
			GUI.TextField(new Rect(Screen.width / 2 - 100, 20,300,20),"Black: " + s.black + "White: " + s.white);		  
		}

		if(GUI.Button(new Rect(Screen.width - width,0,width,height),"Save to SGF File"))
		{
			//TODO: Actually figure out the SGF file enough to make this work correctly.
		}

		///TODO: Set this to only be in a special view SGF mode. Save should always be enabled.
		if(GUI.Button(new Rect(Screen.width - width,height,width,height),"Load SGF File"))
		{
			//TODO: Replace with a custom browser 
			#region SGGLoadingEditorUtility
#if UNITY_EDITOR
			string file = EditorUtility.OpenFilePanel("Open SGF File",EditorApplication.applicationPath,"sgf");
			if(file != null)
			{
				List<Move> newMoves = SGFParser.readSGFFile(file);
				if(newMoves != null)
				{
					//Delete all curent GameObjects that have tag stone
					GameObject[] objects = GameObject.FindGameObjectsWithTag("stone");
					foreach(GameObject obj in objects)
					{
						GameObject.Destroy(obj);
					}
					moves = newMoves;
					currentMove = moves.Count-1;
					for(int i = 0; i <= currentMove; ++i)
					{
						GameObject newStone;
						if(moves[i].MoveColor == Move.Color.E_BLACK)
						{
							newStone = GameObject.Instantiate(blackStone,new Vector3(moves[i].Column,moves[i].Row,0.0f),Quaternion.identity) as GameObject;
						}
						else
						{
							newStone = GameObject.Instantiate(whiteStone,new Vector3(moves[i].Column,moves[i].Row,0.0f),Quaternion.identity) as GameObject;
						}
						moves[i].Visual = newStone;
					}
				}
			}
#endif
			#endregion
		}
		if(GUI.Button(new Rect(Screen.width - width,2 * height,width,height),"Show Game Tree"))
		{
			showGameTree = !showGameTree;
		}

		GUI.BeginScrollView(new Rect(Screen.width - width,3 * height,width,height),new Vector2(),new Rect(0,0,width,height * 2));
		//Show all comments of the current move
		foreach(string comment in currentMoveRef.Comments)
		{
			GUILayout.Label(comment);
		}
		GUI.EndScrollView();

		if(showGameTree)
		{
			overlayChange.Invoke(true);
			GUI.Window(1,new Rect(0,0,Screen.width,Screen.height),gameTreeWindow,"Game Tree");
		}
		if(GUI.Button(new Rect(Screen.width - width,4 * height,width / 2,height/2),"<"))
		{
			if(currentMoveRef.Parent != null)
			{
				gotoMove(currentMoveRef.Parent);
			}
		}
		if(GUI.Button(new Rect(Screen.width - width /2 ,4 * height,width / 2,height/2),">"))
		{
			if(currentMoveRef.Next != null && currentMoveRef.Next.Next != null)
			{
				gotoMove(currentMoveRef.Next);
			}
		}
		if(GUI.Button (new Rect(Screen.width - width, 4 * height + height / 2,width,height/2), "Return to Menu"))
		{
			Application.LoadLevel(0);
		}
	
		// GUI.TextField(new Rect(Screen.width - width,2 * height, (3 * width) / 4,height/ 2),userComment);
	}

	public void drawValidMoves(List<Vector3> moves)
	{
		foreach(Vector3 v in moves)
		{
			GameObject.Instantiate(debugThing,v,Quaternion.identity);
		}
	}

	public void drawValidMoves()
	{
		List<Vector3> moves = getValidMoves();
		foreach(Vector3 v in moves)
		{
			GameObject.Instantiate(debugThing,v,Quaternion.identity);
		}
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
						foreach(Vector2 v in scoredPoints)
						{
							GameObject.Instantiate(blackScoreCube,new Vector3(v.x,v.y),Quaternion.identity);
						}
					}
					else if(value == 2)//white
					{
						s.white += score;
						foreach(Vector2 v in scoredPoints)
						{
							GameObject.Instantiate(whiteScoreCube,new Vector3(v.x,v.y),Quaternion.identity);
						}
					}
				}
			}
		}

		clearMarkers(stonesToCount);
		s.black += BoardInfo.BlackStonesCaptured;
		s.white += BoardInfo.WhiteStonesCaptured + BoardInfo.Komi;//TOOD: Store komi elsewhere
		return s;
	}

	public float testMove(int row, int col)
	{
		int[,] stonesCopy = new int[BoardInfo.Size, BoardInfo.Size];

		for(int i = 0; i < BoardInfo.Size; ++i)
		{
			for(int j = 0; j < BoardInfo.Size; ++j)
			{
				stonesCopy[i,j] = stones[i,j];
			}
		}

		stonesCopy[row,col] = currentMoveColor == Move.Color.E_BLACK ? 1 : 2;
		Score s = score (stonesCopy);
		return currentMoveColor == Move.Color.E_BLACK ? s.black : s.white;
	}

	public float estimateScore()
	{
		Score s = score (stones);
		return currentMoveColor == Move.Color.E_BLACK ? s.black : s.white;
	}

	private bool whitePassed = false;
	private bool blackPassed = false;

	public bool WhitePassed 
	{
		get
		{
			return whitePassed;
		}
	}

	public bool BlackPassed
	{
		get
		{
			return blackPassed;
		}
	}

	public bool opponentPassed()
	{
		if(currentMoveColor == Move.Color.E_BLACK)
		{
			return whitePassed;
		}
		else if(currentMoveColor == Move.Color.E_WHITE)
		{
			return blackPassed;
		}
		return false;
	}

	public void pass(Move.Color passColor)
	{
		//Can only pass on you move
		if(passColor != currentMoveColor)
			return;

		if(currentMoveColor == Move.Color.E_BLACK)
		{
			Debug.Log ("Black Passed");
			blackPassed = true;
		}
		else if(currentMoveColor == Move.Color.E_WHITE)
		{
			Debug.Log ("White Passed");
			whitePassed = true;
		}
		currentMoveColor = currentMoveColor == Move.Color.E_BLACK ? Move.Color.E_WHITE : Move.Color.E_BLACK;
	}

	[RPC]
	public void passInt(int color)
	{
		if(color == 1)
		{
			pass (Move.Color.E_BLACK);
		}
		else if(color == 2)
		{
			pass(Move.Color.E_WHITE);
		}
	}

	private void killGroup(int row, int col, int target, int value)
	{
		if (row < 0 || row >= BoardInfo.Size || col < 0 || col >= BoardInfo.Size)
			return;

		if(stones[row,col] == target)
		{
			stones[row,col] = value;

			foreach(Move m in moves)
			{
				int rowPrime = row - BoardInfo.Size / 2;
				int colPrime = col - BoardInfo.Size / 2;
				if(m.Column == colPrime && m.Row == rowPrime)
				{
					m.Visual.SetActive(false);
				}
			}
		}
		else if(stones[row,col] == 0)
		{
			stones[row,col] = 3;
		}
		else if(stones[row,col] != target)
		{
			return;
		}
		else
		{
			return;
		}

		killGroup (row + 1, col, value, target);
		killGroup (row - 1, col, value, target);
		killGroup (row, col + 1, value, target);
		killGroup (row, col - 1, value, target);
	}

    /// <summary>
    /// Marks a group as dead. At the moment this simply kills the group with no means to retrieve the information.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
	public void markGroupDead(int row, int col)
	{
		if (stones [row, col] == 0)
						return;

		CheckBoard.removeGroup (col, row, stones [row, col], stones);

        removeRemovedStonesVisuals();
	}

    public void removeRemovedStonesVisuals()
    {
        foreach (Move m in moves)
        {
            if (stones[m.Row, m.Column] == 0)
            {
                if (m != null && m.Visual != null)
                {
                    m.Visual.SetActive(false);
                }
            }
        }
    }

	public bool occupied(int row, int col)
	{
		return stones[row, col] != 0;
	}


	bool isGroupSurroundedHelper(int x, int y, Move.Color color, int[,] board)
	{
		if (y < 0 || y >= BoardInfo.Size || x < 0 || x >= BoardInfo.Size)
			return true;

		if(board[y,x] != (color == Move.Color.E_BLACK ? 1 : 2))
		{
			//TODO: Should really have a define for these colors
			if(board[y,x] == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		board [y, x] = 3;

		return isGroupSurrounded(x - 1, y, color, board) && isGroupSurrounded(x + 1, y, color, board) && isGroupSurrounded (x, y - 1, color, board) && isGroupSurrounded(x,y+1,color,board);
	}

	/// <summary>
	/// Is the move valid.
	/// </summary>
	/// <returns><c>true</c>, if move valid was vaild, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="color">Color.</param>
	public bool isMoveValid(int x, int y, Move.Color color)
	{
		return CheckBoard.isMoveValid (x, y, stones, color);
	}

	/// <summary>
	/// Determines if a group in a given board is surronded. 
	/// Is not the most efficent method 
	/// </summary>
	/// <returns><c>true</c>, if gropu surronded was ised, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="color">Color.</param>
	/// <param name="board">Board.</param>
	bool isGroupSurrounded(int x, int y, Move.Color color, int[,] board)
	{
		bool value = isGroupSurroundedHelper (x, y, color, board);
		clearMarkers (board);
		return value;
	}

	[RPC]
	public void makeMove(int row, int col)
	{
        //Final catach against illegal moves
        if(!CheckBoard.makeMove(col,row,stones,currentMoveColor))
        {
            return;
        }

		blackPassed = false;
		whitePassed = false;

        removeRemovedStonesVisuals();

        //The move we made was valid need to spawn the actual visual representation of that move.
		GameObject newStone;
		if(currentMoveColor == Move.Color.E_BLACK)
		{
			if(!BoardInfo.Networked)
			{
				newStone = GameObject.Instantiate(blackStone,new Vector3(col,row,0.0f),Quaternion.identity) as GameObject;
			}
			else
			{
				newStone = Network.Instantiate(blackStone,new Vector3(col,row,0.0f),Quaternion.identity,0) as GameObject;
			}
		}
		else
		{
			if(!BoardInfo.Networked)
			{
				newStone = GameObject.Instantiate(whiteStone,new Vector3(col,row,0.0f),Quaternion.identity) as GameObject;
			}
			else
			{
				newStone = Network.Instantiate(whiteStone,new Vector3(col,row,0.0f),Quaternion.identity,0) as GameObject;
			}
		}

		if(lastMove != null)
			lastMove.GetComponent<Transform>().position = new Vector3(col,row);

		if (currentMoveRef.Next == null) 
		{
			currentMoveRef.Visual = newStone;
			currentMoveRef.Row = row;
			currentMoveRef.Column = col;
			currentMoveRef.Next = new Move ();
			currentMoveRef.Next.Parent = currentMoveRef;
			currentMoveRef.MoveColor = currentMoveColor;
			currentMoveRef = currentMoveRef.Next;
			moves.Add (currentMoveRef);
		}
		else if(currentMoveRef.Next.Row == row && currentMoveRef.Next.Column == col)
		{
			currentMoveRef = currentMoveRef.Next;
		}
		else
		{
			//We are adding a variant this can lead to code getting complicated.
			Move variant = new Move ();
			variant.Visual = newStone;
			variant.Row = row;
			variant.Column = col; 
			variant.Parent = currentMoveRef;
			currentMoveRef.addVariant(variant);
			currentMoveRef = variant;
			currentMoveRef.MoveColor = currentMoveColor;
		}

		/*
		moves[currentMove].Visual = newStone;
		moves[currentMove].Row = row;
		moves[currentMove].Column = col;
		moves.Add(new Move());
		*/

		currentMove++;
		currentMoveColor = currentMoveColor == Move.Color.E_BLACK ? Move.Color.E_WHITE : Move.Color.E_BLACK;
	}

	public int[,] Stones
	{
		get
		{
			return stones;
		}
	}

	public void networkMakeMove(int row, int col)
	{
		networkView.RPC ("makeMove", RPCMode.All, row, col);
	}

	public Board()
	{
	}
}
