using UnityEngine;
using System.Collections;

public class NetworkedPlayerControls : MonoBehaviour {

	public NetworkPlayer theOwner;
	float lastClientXInput = 0f;
	float lastClientYInput = 0f;
	float serverCurrentXInput = 0f;
	float serverCurrentYInput = 0f;


	void Awake()
	{
		if(Network.isClient)
		{
			enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(theOwner != null && Network.player == theOwner)
		{
			float xInput = 0f;
			float yInput = 0f;

			if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
			{
				xInput = 0;
			}
			else if(Input.GetKey(KeyCode.A))
			{
				xInput = -1;
			}
			else if(Input.GetKey(KeyCode.D))
			{
				xInput = 1;
			}
			else
			{
				xInput = 0;
			}

			if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
			{
				yInput = 0;
			}
			else if(Input.GetKey(KeyCode.W))
			{
				yInput = 1;
			}
			else if(Input.GetKey(KeyCode.S))
			{
				yInput = -1;
			}
			else
			{
				yInput = 0;
			}


			if(lastClientXInput != xInput || lastClientYInput != yInput)
			{
				lastClientXInput = xInput;
				lastClientYInput = yInput;

				if(Network.isServer)
				{
					SendMovementInput(xInput, yInput);
				}
				else if(Network.isClient)
				{
					networkView.RPC("SendMovementInput", RPCMode.Server, xInput, yInput);
				}
			}
		}
		
		
		if(Network.isServer)
		{
			float speed = 3;
			transform.position = new Vector2(transform.position.x + (serverCurrentXInput * speed * Time.deltaTime), transform.position.y + (serverCurrentYInput * speed * Time.deltaTime));
		}

	}


	[RPC]
	void SetPlayer(NetworkPlayer player)
	{
		theOwner = player;
		if(player == Network.player)
		{
			enabled = true;
		}
	}

	[RPC]
	void SendMovementInput(float xInput, float yInput)
	{
		serverCurrentXInput = xInput;
		serverCurrentYInput = yInput;
	}


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = transform.position;
			stream.Serialize(ref pos);
		}
		else
		{
			Vector3 posReceive = Vector3.zero;
			stream.Serialize(ref posReceive);
			transform.position = new Vector2(posReceive.x, posReceive.y);
		}
	}
}
