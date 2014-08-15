using UnityEngine;
using System.Collections;

public class BlockAction : MonoBehaviour 
{
	private GameObject unitychan;
	void Start()
	{
	}

	void Update()
	{
	}

	void DestroyBlock()
	{
		Debug.Log("hogehoge");
	}

	void OnCollisionEnter(Collision collisionInfo)
	{

		//Debug.Log ("hit inside block: " + collisionInfo.gameObject.tag);
	}
}
