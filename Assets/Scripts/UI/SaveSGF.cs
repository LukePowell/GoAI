using UnityEngine;
using System.Collections;

public class SaveSGF : MonoBehaviour {

	public GameObject board;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		board.GetComponent<Board>().saveToSGFFile("wee.sgf");
	}
}
