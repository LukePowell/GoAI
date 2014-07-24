using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SGFParser
{
	public static List<Move> readSGFFile(string filename)
	{
		List<Move> moves = new List<Move>();
		FileStream sgfFile = File.Open(filename,FileMode.Open);
		StreamReader reader = new StreamReader(sgfFile);
		string line;
		int lineNum = 0;
		while((line = reader.ReadLine()) != null)
		{
			//If we are on a line > 2 we are reading moves
			if(lineNum > 2)
			{
				Move.Color color;

				if(line[1] == 'B')
				{
					color = Move.Color.E_BLACK;
				}
				else
				{
					color = Move.Color.E_WHITE;
				}

				int row = (line[3] - 'a') - (BoardInfo.Size / 2);
				int col = (BoardInfo.Size / 2) - (line[4] - 'a');

				Debug.Log("" + row + " " + col + "");
				Move addMove = new Move(color,col,row);//Worry about comments each
				moves.Add(addMove);
			}
			Debug.Log(line);
			lineNum++;
		}
		return moves.Count != 0 ? moves : null;
	}

	public static void saveSGFFile(string filename, List<Move> move)
	{
		Debug.Log(Application.persistentDataPath + "/" + filename);
		FileStream sgfFile = File.Open(Application.persistentDataPath + "/" + filename,FileMode.Create);
		StreamWriter writer = new StreamWriter(sgfFile);

		writer.WriteLine("(;GN[Luke Powell GO AI]");
		writer.WriteLine("PB[Black]");
		writer.WriteLine("HA[0]");
		writer.WriteLine("PW[White]");
		writer.WriteLine("KM[6.5]");
		writer.WriteLine("RU[Japenese]");
		writer.WriteLine("SZ["+BoardInfo.Size+"]");

		for(int i = 0; i < move.Count - 1; i += 2)
		{
			char rowBlack = (char)(((BoardInfo.Size - 1) - move[i].Row) + 'a');
			char colBlack = (char)(move[i].Column + 'a');

			char rowWhite = (char)(((BoardInfo.Size - 1) - move[i+1].Row) + 'a');
			char colWhite = (char)(move[i+1].Column + 'a');

			writer.WriteLine(";B["+colBlack+rowBlack+"]");
			writer.WriteLine(";W["+colWhite+rowWhite+"]");
		}
		writer.WriteLine(")");
		writer.Close();
	}
}
