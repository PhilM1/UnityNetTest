using UnityEngine;
using System.Collections;

public class WASDMovementScript : MonoBehaviour {


	float speed = 2.0f;

	// Use this for initialization
	void Start () {
		//If the player doesn't own this object disably movement.
		if(networkView.isMine == false)
		{
			GetComponent<WASDMovementScript>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKey(KeyCode.W))
		{
			//Move up
			this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + speed * Time.deltaTime, this.gameObject.transform.position.z);
		}

		if(Input.GetKey(KeyCode.S))
		{
			//Move up
			this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - speed * Time.deltaTime, this.gameObject.transform.position.z);
		}

		if(Input.GetKey(KeyCode.A))
		{
			//Move up
			this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x - speed * Time.deltaTime, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
		}

		if(Input.GetKey(KeyCode.D))
		{
			//Move up
			this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x + speed * Time.deltaTime, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
		}
	
	}
}
