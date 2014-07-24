using UnityEngine;
using System.Collections;

public class Back : MonoBehaviour {

	public UIPanel containerPanel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnClick()
	{
		containerPanel.gameObject.SetActive(false);
		ShowPanel.previous.gameObject.SetActive(true);
	}
}
