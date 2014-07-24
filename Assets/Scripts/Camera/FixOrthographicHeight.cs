using UnityEngine;
using System.Collections;

public class FixOrthographicHeight : MonoBehaviour {

	void Awake()
	{
		GetComponent<Camera>().orthographicSize = Screen.width / 2;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Camera>().orthographicSize = Screen.width / 2;
	}
}
