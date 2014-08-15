using UnityEngine;
using System.Collections;

public class UnitychanInsideColChild : MonoBehaviour 
{
	private GameObject parent;
	// Use this for initialization
	void Start () 
	{
		parent = gameObject.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider collider)
	{
		parent.SendMessage("RedirectedOnTriggerEnter", collider);
	}
}
