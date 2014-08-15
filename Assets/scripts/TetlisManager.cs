using UnityEngine;
using System.Collections;

public class TetlisManager : MonoBehaviour 
{
	public int SlideDownRaw = 10;

	[SerializeField]
	private float fallInterval = 5.0f;
	[SerializeField]
	private float fallIntervalMin = 2.0f;
	
	private GameObject gameManager;
	private bool startFlag;

	private GameObject[] field = new GameObject[10 * 18];
	private GameObject rightWall;
	private GameObject leftWall;
	private TetliMino mino;

	private AudioSource player;

	// Use this for initialization
	void Start () 
	{
		gameManager = GameObject.Find("GameManager");

		startFlag = false;

		rightWall = GameObject.Find("StageRightWall");
		leftWall = GameObject.Find("StageLeftWall");
		
		mino = new TetliMino ();
		
		GameObject normalBlockPrefab = (GameObject)Resources.Load ("NormalBlock");
		for (int i = 0; i < 10; i++)
		{
			field[17 * 10 + i] = (GameObject)MonoBehaviour.Instantiate(normalBlockPrefab,
			                                                           new Vector3(TetliMino.STEP * i, 
																		           0,
																		           0.0f),
			                                                           Quaternion.identity);
			field[17 * 10 + i].tag = "NormalBlock_Stop";
		}

		player = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void StartTetlis()
	{
		if(!startFlag)
		{
			mino.Dispose();
			mino.MinoCreate((MinoShape.Type)Random.Range(0, (int)MinoShape.Type.MinoNum),
			                new Vector3(1.8f, 10.8f, 0.0f));
			StartCoroutine ("MinoFalling");

			startFlag = true;
		}
	}

	void StopTetlis()
	{
		if(startFlag)
		{
			StopCoroutine("MinoFalling");
			startFlag = false;
		}
	}

	void MinoSlideRight()
	{
		if(startFlag)
		{
			mino.Translate(1, 0, 0);
			if(mino.CheckCollision(field, rightWall, leftWall))
				mino.Translate(-1, 0, 0);
		}
	}
	
	void MinoSlideLeft()
	{
		if(startFlag)
		{
			mino.Translate(-1, 0, 0);
			if(mino.CheckCollision(field, rightWall, leftWall))
				mino.Translate(1, 0, 0);
		}
	}
	
	void MinoSlideDown()
	{
		if(startFlag)
		{
			mino.Translate(0, -1, 0);
			if(mino.CheckCollision(field, rightWall, leftWall))
				mino.Translate(0, 1, 0);
		}
	}
	
	void MinoRightRotate()
	{
		if(startFlag)
		{
			mino.RightRotate();
			if(mino.CheckCollision(field, rightWall, leftWall))
				mino.LeftRotate();
		}
	}


	IEnumerator MinoFalling()
	{
		while (true)
		{
			mino.Translate(0, -1, 0);
			if(mino.CheckCollision(field, rightWall, leftWall))
			{
				mino.Translate(0, 1, 0);
				this.MinoStoping();
			}
			yield return new WaitForSeconds(fallInterval); 
		}
	}

	public void MinoStoping()
	{
		StopCoroutine("MinoFalling");

		// stack block into field array

		ArrayList rawList = new ArrayList();

		foreach (GameObject block in mino.Blocks)
		{
			float column = Mathf.Round(block.transform.position.x / TetliMino.STEP);
			float raw = Mathf.Round(17.0f - block.transform.position.y / TetliMino.STEP);
			int index = (int)column + (int)raw * 10;
			
			field[index] = block;
			field[index].tag = "NormalBlock_Stop";

			if(!rawList.Contains((int)raw))
				rawList.Add((int)raw);
		}

		// destroy line 
		bool destroy;

		foreach (int raw in rawList) 
		{
			destroy = true;
			for(int i = 0; i < 10; i++)
			{
				if(field[raw * 10 + i] == null)
				{
					destroy = false;
					break;
				}
			}

			if(destroy)
			{
				DeleteLine(raw);
				SlideDownField(raw);
			}
		}

		// if stack on raw of DownSlideRaw, slide down all block.
		bool loopEnd = false;
		while (!loopEnd)
		{
			loopEnd = true;
			for (int i = 0; i < 10; i++) 
			{
				if(field[SlideDownRaw * 10 + i] != null)
				{
					loopEnd = false;

					DeleteLine(17);
					SlideDownField(17);
					gameManager.SendMessage("ScoreUpdate", ScoreValue.STACK_SCORE);
					gameManager.SendMessage("SlideBack");

					if(fallInterval > fallIntervalMin)
						fallInterval -= 0.01f;

					break;
				}
			}
		}

		//player.Play ();
		
		mino.MinoCreate((MinoShape.Type)Random.Range(0, (int)MinoShape.Type.MinoNum),
		                new Vector3(1.8f, 10.8f, 0.0f));

		StartCoroutine("MinoFalling");
	}


	private void SlideDownField(int startRaw)
	{
		for (int i = startRaw; i > 0; i--) 
		{
			for(int j = 0; j < 10; j++)
			{
				field[i * 10 + j] = field[(i - 1) * 10 + j];
				if(field[i * 10 + j] != null)
					field[i * 10 + j].transform.Translate(0, -TetliMino.STEP, 0);
			}
		}

		for (int i = 0; i < 10; i++) 
		{
			field[i] = null;
		}
	}


	private void DeleteLine(int raw)
	{
		for (int i = 0; i < 10; i++) 
		{
			Destroy(field[raw * 10 + i]);
			field[raw * 10 + i] = null;
		}
	}

	void DestroyByBomb(GameObject target)
	{
		if(startFlag)
		{
			float column = Mathf.Round(target.transform.position.x / TetliMino.STEP);
			float raw = Mathf.Round(17.0f - target.transform.position.y / TetliMino.STEP);

			for(int i = (int)(column-1); i < (int)(column-1) + 3; i++)
			{
				for(int j = (int)(raw-1); j < (raw-1) + 3; j++)
				{
					if( (i >= 0 && i < 10) && (j >= 0 && j < 18)) 
					{
						MonoBehaviour.Destroy(field[i + j * 10]);
						field[i + j * 10] = null;
					}
				}
			}
		}
	}

	// **** properties **** //

	public float FallInterval
	{
		set{ if(value >= fallIntervalMin) fallInterval = value;}
		get{ return fallInterval; }
	}

	public float FallIntervalMin
	{
		set{ if(value <= fallInterval) fallIntervalMin = value;}
		get{ return fallIntervalMin; }
	}
}
