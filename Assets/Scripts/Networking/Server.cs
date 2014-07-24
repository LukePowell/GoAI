using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Network.InitializeServer (2, 25002, !Network.HavePublicAddress ());
		MasterServer.RegisterHost ("GoMatch", + BoardInfo.Size + " x " + BoardInfo.Size);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
