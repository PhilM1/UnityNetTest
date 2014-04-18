using UnityEngine;
using System.Collections;

public class SyncPositionOverNetwork : MonoBehaviour {

	float lastSyncTime = 0.0f; //The time of the last syncPosition recieved.
	float syncDelay = 0.0f; //The time difference between the current and last syncPosition.
	float syncTime = 0.0f; //A counter for how much time has elapsed since the last syncPosition.
	Vector3 syncStartPosition = Vector3.zero; //The position to start Lerping from.
	Vector3 syncEndPosition = Vector3.zero; //The position to Lerp toward.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(networkView.isMine == false)
		{
			InterpolateMovement();
		}


	
	}

	void Awake()
	{
		lastSyncTime = Time.time;
	}


	//Sends or Recieves Serialized variables over the network.
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		//Variable to be used over the network.
		Vector3 syncPosition = Vector3.zero;

		//If you're the owner of the object you are sending the objects position over the network.
		if(stream.isWriting)
		{
			//Set syncPosition to this objects current position.
			syncPosition = transform.position;
			//Send the position over the network.
			stream.Serialize(ref syncPosition);
		}
		//You're recieving the position from the Owner.
		else
		{
			//Serialize the new incoming position.
			stream.Serialize(ref syncPosition);

			syncTime = 0.0f;
			syncDelay = Time.time - lastSyncTime;
			lastSyncTime = Time.time;

			syncStartPosition = transform.position;
			syncEndPosition = syncPosition;
		}
	
	}

	void InterpolateMovement()
	{
		syncTime += Time.deltaTime;
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}
}
