using UnityEngine;
using System.Collections;

public class SelectBoardSize : MonoBehaviour {

	public int size = 9;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnClick()
	{
		BoardInfo.Size = size;
	}
}
