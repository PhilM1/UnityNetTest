using UnityEngine;
using System.Collections;


public class ConnectionHandler : MonoBehaviour {
	
	
	
	
	public Transform playerPrefab_Net;
	public ArrayList playerScripts = new ArrayList();
	
	void Awake()
	{
		//when this scene has loaded, try setting up a connection based on the type selected from the menu
		if(GlobalGameState.Instance.connectionType == ConnectionType.Client)
		{
			Network.Connect(GlobalGameState.Instance.ipAddress, GlobalGameState.Instance.port);
		}
		else if(GlobalGameState.Instance.connectionType == ConnectionType.Host)
		{
			Network.InitializeServer(GlobalGameState.Instance.maxPlayers, GlobalGameState.Instance.port, !Network.HavePublicAddress());
		}
		else if(GlobalGameState.Instance.connectionType == ConnectionType.Dedicated)
		{
			Network.InitializeServer(GlobalGameState.Instance.maxPlayers, GlobalGameState.Instance.port, !Network.HavePublicAddress());
		}
	}
	
	void OnFailedToConnect()
	{
		//when the network connection fails for the client then switch back to the main menu
		Application.LoadLevel("Main");
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		//Switch back to main menu if disconnected from the server.
		Application.LoadLevel("Main");
	}
	
	void OnServerInitialized()
	{
		SpawnPlayer(Network.player);
	}

	void OnConnectedToServer()
	{
		//SpawnPlayer(Network.player);
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		SpawnPlayer(player);
		Debug.Log(player.ToString() + " Connected to the server From: " + player.externalIP + " " + player.externalPort + " " + player.ipAddress + " " + player.port);
	}
	
	void SpawnPlayer(NetworkPlayer player)
	{
		string tempPlayerString = player.ToString();
		int playerNumber = System.Convert.ToInt32(tempPlayerString);
		
		Transform newPlayerTransform = (Transform)Network.Instantiate(playerPrefab_Net, transform.position, transform.rotation, playerNumber);
		//newPlayerTransform.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
		playerScripts.Add(newPlayerTransform.GetComponent("NetworkedPlayerControls"));
		
		
		NetworkView theNetworkView = newPlayerTransform.networkView;
		theNetworkView.RPC("SetPlayer", RPCMode.AllBuffered, player);
		
		//tell the client to create the model for this player
		//theNetworkView.RPC("CreateModel", RPCMode.OthersBuffered, player); //networkplayer
	}
}
