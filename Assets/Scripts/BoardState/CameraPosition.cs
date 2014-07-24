using UnityEngine;
using System.Collections;

public class CameraPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.transform.position = new Vector3 (BoardInfo.Size, BoardInfo.Size / 2, this.transform.position.z);
		Debug.Log(GetComponent<Camera>().aspect);

		float targetAspect = 1.957399f;//Wee

		float windowAspect = (float)Screen.width / (float)Screen.height;

		float scale = windowAspect / targetAspect;

		Camera c = GetComponent<Camera>();

		if(scale < 1.0f)
		{
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = scale;
			rect.x = 0;
			rect.y = (1.0f - scale) / 2.0f;

			camera.rect = rect;
		}
		else
		{
			float scaleWidth = 1.0f / scale;

			Rect rect = camera.rect;

			rect.width = scaleWidth;
			rect.height	= 1.0f;
			rect.x = (1.0f - scaleWidth) / 2.0f;
			rect.y = 0;

			camera.rect = rect;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
