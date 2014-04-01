using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public string connectionIP = "127.0.0.1";
	public int connectionPort = 25001;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnGUI()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUI.Label(new Rect(10, 10, 200, 20), "Status: Disconnected");

			//The Connection IP TextFields
			GUI.Label(new Rect(10,90,200,20), "IP to connect:");
			connectionIP = GUI.TextField(new Rect(100,90,200,20), connectionIP);
			
			if (GUI.Button(new Rect(10, 30, 120, 20), "Join"))
			{
				GlobalGameState.Instance.connectionType = ConnectionType.Client;
				SwitchToGame();
				
				//Network.Connect(connectionIP, connectionPort);
			}
			
			if (GUI.Button(new Rect(10, 50, 120, 20), "Host & Play"))
			{
				GlobalGameState.Instance.connectionType = ConnectionType.Host;
				SwitchToGame();
				
				//Network.InitializeServer(32, connectionPort, false);
			}
			
			if (GUI.Button(new Rect(10, 70, 120, 20), "Test Dedicated"))
			{
				GlobalGameState.Instance.connectionType = ConnectionType.Dedicated;
				SwitchToGame();
				
				//Network.InitializeServer(32, connectionPort, false);
			}
			
		}
	}
	
	void SwitchToGame()
	{
		GlobalGameState.Instance.ipAddress = connectionIP;
		GlobalGameState.Instance.port = connectionPort;
		
		Application.LoadLevel("Game");
	}
}
