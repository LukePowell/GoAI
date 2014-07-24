using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;


public class GTPListener : MonoBehaviour {

	//This should start up in the main menu and continue working throughout the game
	Thread listenThread;

	bool quit = false;
	bool clear = false;

	ASCIIEncoding encoder = new ASCIIEncoding();
	TcpClient client;
	Board b;

	Move m = null;
	MonteCarloGTP gtpAI;

	public string fromBytes(byte[] bytes, int size)
	{
		return encoder.GetString(bytes,0,size);
	}

	public byte[] toBytes(string convert)
	{
		return encoder.GetBytes(convert);
	}

	char[] toCol = {'A','B','C','D','E','F','G','H','J','K','L','M','N','O','P','Q','R','S','T'};

	public int fromCharToCol(char c)
	{
		c = Char.ToUpper(c);
		if(c <= 'A' && c <= 'H')
			return c - 'A';
		else
			return (c  - 'A') - 1;
	}

	public void runGTP()
	{
		client = new TcpClient();
		while(!client.Connected)
		{
			client.Connect(IPAddress.Loopback,8000);
		}
		if(client.Connected)
		{
			UnityEngine.Debug.Log("Connected to server");
		}
	
		byte[] buffer = new byte[4096];
		while(!quit)
		{
			int size = client.GetStream().Read(buffer,0,4096);
			string s = fromBytes(buffer,size);
			string[] command = s.Split(' ');
			if(command[0] == "protocol_version")
			{
				byte[] bytes = toBytes("= 2\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else if(command[0] == "name")
			{
				byte[] bytes = toBytes("= LPBot\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else if(command[0] == "version")
			{
				byte[] bytes = toBytes("= 1.0\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else if(command[0] == "quit")
			{
				Application.Quit();
			}
			else if(command[0] == "boardsize")
			{
				BoardInfo.Size = int.Parse(command[1]);
				byte[] bytes = toBytes("=\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else if(command[0] == "komi")
			{
				BoardInfo.Komi = float.Parse(command[1]);
				byte[] bytes = toBytes("=\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);	
			}
			else if(command[0] == "play")
			{
				int color = 0;
				int col = fromCharToCol(command[2][0]);
				int row = 19 - int.Parse("" + command[2][1] + command[2][2]);
				if(command[1][0] == 'w')//handles both w and white
				{
					color = 0;
				}
				else if(command[1][0] == 'b')//handles both b and black
				{
					color = 1;
				}

				bool pass = false;
				if(command[2] == "pass")
				{
					b.pass(b.CurrentMoveColor);
				}
				else
				{
					m = new Move(color == 0 ? Move.Color.E_WHITE : Move.Color.E_BLACK,col,row);					
				}
				byte[] bytes = toBytes("=\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else if(command[0] == "genmove")
			{
				Vector2 move = new Vector2();
				char color = command[1][0];

				if(command[1][0] == 'w' || command[1][0] == 'W')
				{
					//Run until we get a move generated we are already in seperate thread
					while(!gtpAI.genMove(ref move, Move.Color.E_WHITE));
				}
				else if(command[1][0] == 'b' || command[1][0] == 'B')
				{
					//Play a black move
					while(!gtpAI.genMove(ref move, Move.Color.E_BLACK));
				}


				char col = toCol[(int)move.x];
				byte[] bytes = toBytes("= "+ color + " " + toCol + (int)move.y + "\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else if(command[0] == "clear_board")
			{
				BoardInfo.WhiteStonesCaptured = 0;
				BoardInfo.BlackStonesCaptured = 0;
				b.clear();
				clear = true;
				byte[] bytes = toBytes("=\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			else 
			{
				byte[] bytes = toBytes("=\n\n");
				client.GetStream().Write(bytes,0,bytes.Length);
			}
			UnityEngine.Debug.Log(s);
		}
	}

	void OnDestroy()
	{
		client.Close();
		listenThread.Abort();
	}

	// Use this for initialization
	void Start () {
		//If we started up in GTP mode do GTP stuff
		if(BoardInfo.Mode == BoardInfo.GameMode.E_GTP)
		{
			b = GetComponent<Board>();
			UnityEngine.Debug.Log ("I have started.");
			listenThread = new Thread(this.runGTP);
			listenThread.Start();
			gtpAI = gameObject.AddComponent<MonteCarloGTP>();
		}
	}

	// Update is called once per frame
	void Update () {
		if(m != null)
		{
			b.makeMove(m.Row,m.Column);
			m = null;
		}
		if(clear)
		{
			GameObject[] boardDraw = GameObject.FindGameObjectsWithTag("boardDraw");
			GameObject[] stones = GameObject.FindGameObjectsWithTag("stone");
			foreach(GameObject o in boardDraw)
			{
				GameObject.Destroy(o);
			}

			foreach(GameObject o in stones)
			{
				GameObject.Destroy(o);
			}
			GetComponent<CreateBoard>().Start();
			clear = false;
		}
	}
}
