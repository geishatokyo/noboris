using UnityEngine;
using System.Collections;

public class TetliMino
{
	// *** const value *** //
	static public readonly float STEP = 0.6f; // mino movement const value.

	// *** class member value *** //
	private GameObject blockPrefab;
	private GameObject[] blocks;
	private int rotateAxisIndex;

	public TetliMino()
	{
		blockPrefab = (GameObject)Resources.Load ("NormalBlock");
	}

	// param is stepping num. not movement vector.
	public void Translate(int stepX = 0, int stepY = 0, int stepZ = 0)
	{
		foreach (GameObject block in blocks) 
		{
			block.transform.Translate(STEP * stepX, STEP * stepY, STEP * stepZ);
		}
	}
	
	public void RightRotate()
	{
		float axisX, axisY;

		axisX = blocks [rotateAxisIndex].transform.position.x;
		axisY = blocks [rotateAxisIndex].transform.position.y;

		// calc rotated position : x' = y, y' = -x
		foreach (GameObject block in blocks) 
		{
			block.transform.position = new Vector3((block.transform.position.y - axisY) + axisX,
			                                       -(block.transform.position.x - axisX) + axisY,
			                                       block.transform.position.z);
		}
	}

	public void LeftRotate()
	{
		float axisX, axisY;
		
		axisX = blocks [rotateAxisIndex].transform.position.x;
		axisY = blocks [rotateAxisIndex].transform.position.y;
		
		// calc rotated position : x' = -y, y' = x
		foreach (GameObject block in blocks) 
		{
			block.transform.position = new Vector3(-(block.transform.position.y - axisY) + axisX,
			                                       (block.transform.position.x - axisX) + axisY,
			                                       block.transform.position.z);
		}
	}

	// if this tetlimino crash object , return true
	public bool CheckCollision(GameObject[] field, GameObject rightWall, GameObject leftWall)
	{
		// check tetlimino pos is side wall inside, outside or ground.
		foreach (GameObject block in blocks)
		{
			if(block.transform.position.x <= leftWall.transform.position.x ||
			   block.transform.position.x >= rightWall.transform.position.x)
			{
				//Debug.Log("Side Collision");
				return true;
			}
				

			if(block.transform.position.y < 0)
			{
				//Debug.Log("Under Collision");
				return true;
			}
		}

		// field check
		foreach (GameObject block in blocks) 
		{
			float column = Mathf.Round(block.transform.position.x / STEP);
			float raw = Mathf.Round(17.0f - block.transform.position.y / STEP);
			int index = (int)column + (int)raw * 10;

			/*Debug.Log("column_f: " + column + "\tcolumn_i: " + (int)column +
			          "\nraw_f: " + raw + "\traw_i: " + (int)raw +
					  "\tindex: " + index);*/

			if(field[index] != null)
			{
				//Debug.Log("Block Collision");
				return true;
			}
		}

		//Debug.Log("Non Collision");
		return false;
	}

	public void MinoCreate(MinoShape.Type type, Vector3 position)
	{
		int length = MinoShape.SHAPE_WIDTH * MinoShape.SHAPE_HEIGHT;

		int blockNum = 0;
		for (int i = 0; i < length; i++) 
		{
			if(MinoShape.SHAPE[(int)type, i] == 1)
				blockNum++;	
		}

		blocks = new GameObject[blockNum];

		int index = 0;
		for (int i = 0; i < length; i++) 
		{
			if(MinoShape.SHAPE[(int)type, i] == 1)
			{
				// I will write new generating process.

				blocks[index] = (GameObject)MonoBehaviour.Instantiate(blockPrefab,
				                                                      new Vector3(position.x + (i % MinoShape.SHAPE_WIDTH) * STEP, 
				            													  position.y - (i / MinoShape.SHAPE_HEIGHT) * STEP,
				            													  position.z),
				                                                      Quaternion.identity);

				if(i == MinoShape.AXIS_INDEX)
					rotateAxisIndex = index;

				index++;
			}
		} 
	}

	public void Dispose()
	{
		if (blocks == null)
			return;

		foreach(GameObject block in blocks)
		{
			MonoBehaviour.Destroy(block);
		}
	}

	// ***** setter, getter **** //
	public GameObject[] Blocks
	{
		get{return blocks;}
	}
}
