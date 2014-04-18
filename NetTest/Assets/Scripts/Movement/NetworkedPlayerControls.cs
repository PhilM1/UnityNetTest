using UnityEngine;
using System.Collections;

public class NetworkedPlayerControls : MonoBehaviour {

	public GameObject debugNetObj;
	GameObject newDebugObj;

	public NetworkPlayer theOwner;
	float lastClientXInput = 0f;
	float lastClientYInput = 0f;
	float serverCurrentXInput = 0f;
	float serverCurrentYInput = 0f;

	Vector3 posOnServer = Vector3.zero;

	float speed = 5;
	float lastInputTime = 0.0f;


	void Awake()
	{
		//For Debuging.
		newDebugObj = (GameObject)Instantiate(debugNetObj, transform.position, transform.rotation) as GameObject;
		newDebugObj.SetActive(false);

		if(Network.isClient)
		{
			//enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
		{
			newDebugObj.SetActive(!newDebugObj.activeSelf);
		}

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
					lastInputTime = Time.time;
				}
			}

			if(Network.isClient)
			{
				float distance = Vector3.Distance(transform.position, posOnServer);
				float ping = Network.GetAveragePing(Network.connections[0]);
				float margin = 0.05f;

				if (distance > 1.0f)
				{
					Vector3 positionDifference = posOnServer - transform.position;
					transform.position += positionDifference * 0.1f;
						//float lerpTime = (( 1 / distance) * (speed)) / 100;
						//transform.position = Vector3.Lerp(transform.position, posOnServer, lerpTime);
				}
				else
				{
					Vector3 direction = Vector3.Normalize(new Vector3(xInput, yInput, 0));
					transform.position += direction * speed * Time.deltaTime;

					//transform.position = new Vector2(transform.position.x + (xInput * speed * Time.deltaTime), transform.position.y + (yInput * speed * Time.deltaTime));
				}

			}

		}
		
		
		if(Network.isServer)
		{
			Vector3 direction = Vector3.Normalize(new Vector3(serverCurrentXInput, serverCurrentYInput, 0));
			transform.position += direction * speed * Time.deltaTime;

			//transform.position = new Vector2(transform.position.x + (serverCurrentXInput * speed * Time.deltaTime), transform.position.y + (serverCurrentYInput * speed * Time.deltaTime));
		}


		//Interpolation for objects that the client does not own.
		if(Network.isClient)
		{
			if(theOwner == null || theOwner != Network.player)
			{
				Vector3 positionDifference = posOnServer - transform.position;
				float distanceAppart = Vector3.Distance(posOnServer, transform.position);

				if(distanceAppart * 0.1f  < speed * Time.deltaTime)
				{
					if(Vector3.Distance(posOnServer, transform.position) < speed * Time.deltaTime)
					{
						//Snap to location
						transform.position = posOnServer;
					}
					else
					{
						Vector3 direction = Vector3.Normalize(positionDifference);
						transform.position += direction * speed * Time.deltaTime;
					}
				}
				else
				{
					transform.position += positionDifference * 0.1f;
				}
				

			}
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
		Vector3 position = Vector3.zero;

		if (stream.isWriting)
		{
			position = transform.position;
			stream.Serialize(ref position);
		}
		else
		{
			stream.Serialize(ref position);
			if(posOnServer == Vector3.zero)
			{
				transform.position = posOnServer;
			}

			posOnServer = position;

			//Do Correction Here.
			if(theOwner != null && Network.player == theOwner)
			{
				float distance = Vector3.Distance(transform.position, position);
				float ping = Network.GetAveragePing(Network.connections[0]);
				float margin = 0.05f;
				float maxDist = 1 * speed * (ping / 1000 + margin);
				float positionThreshold = Vector2.Distance(Vector2.zero, new Vector2(maxDist, maxDist));
				if(distance >= positionThreshold)
				{
					float lerpTime = (( 1 / distance) * speed) / 100;
					transform.position = Vector3.Lerp(transform.position, position, lerpTime);
				}
			}
			else
			{
				//transform.position = position;
			}
		}
		
		newDebugObj.transform.position = position;
	}


}
