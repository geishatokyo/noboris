using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
	public float ItemSpawnInterval = 6.0f;
	private bool spawnFlag = false;

	private readonly int itemKindNum = 7;
	private GameObject[] ItemPrefabs;
	// Use this for initialization
	void Start () 
	{
		ItemPrefabs = new GameObject[itemKindNum];
		ItemPrefabs [0] = (GameObject)Resources.Load ("ItemBomb");
		ItemPrefabs [1] = (GameObject)Resources.Load ("ItemHeart");
		ItemPrefabs [2] = (GameObject)Resources.Load ("ItemScoreUp");
		ItemPrefabs [3] = (GameObject)Resources.Load ("ItemStepUp");
		ItemPrefabs [4] = (GameObject)Resources.Load ("ItemStepDown");
		ItemPrefabs [5] = (GameObject)Resources.Load ("ItemSpeedUp");
		ItemPrefabs [6] = (GameObject)Resources.Load ("ItemSpeedDown");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void StartItemSpawn()
	{
		if(!spawnFlag)
		{
			spawnFlag = false;
			StartCoroutine("ItemSpawn");
		}
	}

	void StopItemSpawn()
	{
		if(spawnFlag)
		{
			spawnFlag = false;
			StopCoroutine("ItemSpawn");
		}
	}

	IEnumerator ItemSpawn()
	{
		while(true)
		{
			GameObject.Instantiate(ItemPrefabs[Random.Range(0, itemKindNum)],
			               		   new Vector3(0.6f * Random.Range(0, 9), 10.8f, 0.0f),
			       				   new Quaternion(0.0f, 0.0f, 180.0f, 1.0f));
			yield return new WaitForSeconds(ItemSpawnInterval);
		}
	}
}
