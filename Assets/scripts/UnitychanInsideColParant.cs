using UnityEngine;
using System.Collections;

public class UnitychanInsideColParant : MonoBehaviour 
{
	private GameObject unitychan;
	// Use this for initialization
	void Start () 
	{
		unitychan = GameObject.Find("unitychan");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void RedirectedOnTriggerEnter (Collider collider)
	{
		if(collider.gameObject.tag == "NormalBlock" || collider.gameObject.tag == "NormalBlock_Stop")
			unitychan.SendMessage("Respawn");
	}
}
