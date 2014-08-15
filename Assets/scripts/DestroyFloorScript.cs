using UnityEngine;
using System.Collections;

public class DestroyFloorScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		Destroy (collisionInfo.gameObject);
	}

	void OnTriggerEnter(Collider collisionInfo)
	{
		Destroy (collisionInfo.gameObject);
	}
}
