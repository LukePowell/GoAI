using UnityEngine;
using System.Collections;

public class Position : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.transform.position = new Vector3 (BoardInfo.Size / 2, BoardInfo.Size / 2, this.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
