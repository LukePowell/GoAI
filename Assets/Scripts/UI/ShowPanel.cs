using UnityEngine;
using System.Collections;

public class ShowPanel : MonoBehaviour {

	public UIPanel panel;
	public UIPanel containerPanel;
	public static UIPanel previous;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		containerPanel.gameObject.SetActive(false);
		panel.gameObject.SetActive(true);
		previous = containerPanel;
	}
}
