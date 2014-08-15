using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public enum UpdateMode
	{
		Increment,
		Decrement
	}

	// ****** const value ****** //
	private readonly int MAX_LIFE = 5;
	private readonly int FIRST_LIFE = 3;

	private readonly int MAX_BOMB = 3;
	private readonly int FIRST_BOMB = 1;

	private readonly int MAX_SCORE = 99999999;

	// ****** game control valiable ****** //
	private GameObject unitychan;
	private GameObject tetlisManager;
	private GameObject itemSpawner;
	private GameObject result;
	private GameObject backGround;
	private GameObject detonator;
	private bool gameStart = false;
	private bool showResult = false;

	private int pressFlame;
	private Vector3 lastMousePos;
	private Vector3 slideDistance;
	private readonly int slideTriggerValue = 20;
	
	// ****** count down valiable ****** //
	private readonly float strWidth = 25.0f;
	private readonly float strTop = 35.0f;
	private readonly float strLeft = 40.0f;

	private bool showCountDown = true;
	private int count = 4;

	public GUIStyle countDownStrStyle;
	
	// ****** variable ****** //
	private GameObject lifeGUIPrefab;
	private int lifeNum;
	private GameObject[] lifeArray;
	
	private GameObject bombGUIPrefab;
	private int bombNum;
	private GameObject[] bombArray;
	
	private int score;
	public GUIStyle scoreStyle;

	// audio
	AudioSource player;
	AudioClip finishSE;
	
	// Use this for initialization
	void Start ()
	{
		unitychan = GameObject.Find ("unitychan");
		tetlisManager = GameObject.Find("TetlisManager");
		itemSpawner = GameObject.Find("ItemSpawner");
		result = GameObject.Find("ResultObject");
		backGround = GameObject.Find("Background");
		detonator = GameObject.Find ("Detonator-Sounds");

		player = this.GetComponent<AudioSource> ();
		finishSE = (AudioClip)Resources.Load("se/finish");
		
		pressFlame = 0;
		slideDistance = Vector3.zero;

		lifeArray = new GameObject[MAX_LIFE];
		bombArray = new GameObject[MAX_BOMB];

		lifeGUIPrefab = (GameObject)Resources.Load ("GUILife");
		bombGUIPrefab = (GameObject)Resources.Load ("GUIBomb");

		score = 0;

		lifeNum = FIRST_LIFE;
		CleanUpGameObjectArray (lifeArray);
		for (int i = 0; i < FIRST_LIFE; i++) 
		{
			lifeArray[i] = (GameObject)MonoBehaviour.Instantiate(lifeGUIPrefab,
			                                                     new Vector3(0.08f + i * 0.6f, 9.0f, 0f),
			                                                     new Quaternion(0, 0, 180.0f, 1.0f));
		}
		
		bombNum = FIRST_BOMB;
		CleanUpGameObjectArray (bombArray);
		for(int i = 0; i < FIRST_BOMB; i++)
		{
			bombArray[i] = (GameObject)MonoBehaviour.Instantiate(bombGUIPrefab,
			                                                     new Vector3(0.09f + i * 0.6f, 8.4f, 0f),
			                                                     new Quaternion(0, 0, 180.0f, 1.0f));
		}

		StartCoroutine ("StartCountDown");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (lifeNum == 0) 
		{
			StopGame();

			if(!showResult)
			{
				showResult = true;

				Rigidbody[] obj = result.GetComponents<Rigidbody>();
				obj[0].useGravity = true;

				TextMesh[] text = result.transform.Find("ResultText_Score").GetComponents<TextMesh>();
				text[0].text = "score: " + score.ToString("D8");

				player.PlayOneShot(finishSE);
			}

			if(Input.GetMouseButtonUp(0) && result.transform.position.y < 4.8f)
			{
				Application.LoadLevel ("Title");
			}
				
		}

		if (!gameStart)
			return;
		
		if(Input.GetMouseButtonDown(0))
		{
			lastMousePos = Input.mousePosition;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			
			if (Physics.Raycast(ray, out hit))
			{
				GameObject obj = hit.collider.gameObject;

				if(obj.tag == "NormalBlock_Stop" && bombNum > 0)
				{
					pressFlame = 1000; // lock rotate

					GameObject explosion = (GameObject)GameObject.Instantiate(detonator,
					                                                          obj.transform.position,
					                                                          Quaternion.identity);
					explosion.SendMessage("Explode");

					UpdateBomb(UpdateMode.Decrement);

					tetlisManager.SendMessage("DestroyByBomb", obj);
				}
				else if(obj.tag == "unitychan")
				{
					pressFlame = 1000; // lock rotate
					unitychan.SendMessage("ChangeDirection");
				}
			}
		}
		else if(Input.GetMouseButton(0)) 
		{
			pressFlame = pressFlame < 1000 ? pressFlame + 1 : 1000;
			slideDistance = slideDistance + Input.mousePosition -lastMousePos;
			lastMousePos = Input.mousePosition;
		}

		if(Input.GetMouseButtonUp(0))
		{
			if(pressFlame < 10)
				tetlisManager.SendMessage("MinoRightRotate");
			
			pressFlame = 0;
			slideDistance = Vector3.zero;
		}

		// ***** mino slide control ***** //
		if(slideDistance.x >= slideTriggerValue)
		{
			tetlisManager.SendMessage("MinoSlideRight");
			slideDistance.x = 0;
		}
		else if(slideDistance.x <= -slideTriggerValue)
		{
			tetlisManager.SendMessage("MinoSlideLeft");
			slideDistance.x = 0;
		}
		
		if(slideDistance.y <= -slideTriggerValue)
		{
			tetlisManager.SendMessage("MinoSlideDown");
			slideDistance.y = 0;
		}


		// for debug
		if(Input.GetKeyDown(KeyCode.H))
		{
			this.UpdateLife(UpdateMode.Increment);
		}
		else if(Input.GetKeyDown(KeyCode.J))
		{
			this.UpdateBomb(UpdateMode.Increment);
		}
	}

	void OnGUI()
	{
		if(showCountDown)
		{
			countDownStrStyle.fontSize = (int)(Screen.width * strWidth / 100);
			GUILayout.BeginArea(new Rect(count > 0 ? Screen.width * strLeft / 100 : Screen.width * strLeft / 400, 
			                             Screen.height * strTop / 100,
			                             countDownStrStyle.CalcSize(new GUIContent(count > 0 ? count.ToString() : "START")).x,
			                             countDownStrStyle.CalcSize(new GUIContent(count > 0 ? count.ToString() : "START")).y));
			
			GUILayout.Label(count > 0 ? count.ToString() : "START", countDownStrStyle);
			GUILayout.EndArea();
		}

		string text = "score: " + score.ToString("D8");
		scoreStyle.fontSize = (int)(Screen.width * 7 / 100);
		GUILayout.BeginArea(new Rect(Screen.width * 44 / 100, 
		                             Screen.height * 0 / 100,
		                             scoreStyle.CalcSize(new GUIContent(text)).x,
		                             scoreStyle.CalcSize(new GUIContent(text)).y));
		
		GUILayout.Label(text, scoreStyle);
		GUILayout.EndArea();
	}
	
	void StartGame()
	{
		if(!gameStart)
		{
			gameStart = true;
			unitychan.SendMessage("StartMoving");
			tetlisManager.SendMessage("StartTetlis");
			itemSpawner.SendMessage("StartItemSpawn");
		}
	}

	void StopGame()
	{
		if(gameStart)
		{
			gameStart = false;
			unitychan.SendMessage("StopMoving");
			tetlisManager.SendMessage("StopTetlis");
			itemSpawner.SendMessage("StopItemSpawn");
		}
	}

	IEnumerator StartCountDown()
	{
		while(count > 0)
		{
			count--;
			yield return new WaitForSeconds(1); 
		}

		GameObject.Find ("BGM").GetComponent<AudioSource> ().Play ();

		showCountDown = false;
		StartGame ();
	}

	void SlideBack()
	{
		if (backGround.transform.position.y > -10.0f)
			backGround.transform.Translate (0, 0.3f, 0);
	}
	
	void UpdateLife(UpdateMode mode)
	{
		if (!gameStart)
			return;
		
		if (mode == UpdateMode.Increment) 
		{
			if(lifeNum + 1 <= MAX_LIFE)
			{
				lifeArray[lifeNum] = (GameObject)MonoBehaviour.Instantiate(lifeGUIPrefab,
				                                                           new Vector3(0.08f + lifeNum * 0.6f, 9.0f, 0f),
				                                                           new Quaternion(0, 0, 180.0f, 1.0f));
				lifeNum++;
			}
		}
		else if(mode == UpdateMode.Decrement)
		{
			if(lifeNum - 1 >= 0)
			{
				lifeNum--;
				this.Destroy(lifeArray[lifeNum]);
				lifeArray[lifeNum] = null;
			}
		}
	}

	void UpdateBomb(UpdateMode mode)
	{
		if (!gameStart)
			return;

		if (mode == UpdateMode.Increment) 
		{
			if(bombNum + 1 <= MAX_BOMB)
			{
				bombArray[bombNum] = (GameObject)MonoBehaviour.Instantiate(bombGUIPrefab,
				                                                           new Vector3(0.09f + bombNum * 0.6f, 8.4f, 0f),
				                                                           new Quaternion(0, 0, 180.0f, 1.0f));
				bombNum++;
			}
		}
		else if(mode == UpdateMode.Decrement)
		{
			if(bombNum - 1 >= 0)
			{
				bombNum--;
				this.Destroy(bombArray[bombNum]);
				bombArray[bombNum] = null;
			}
		}
	}

	void ScoreUpdate(int addScore)
	{
		if (!gameStart)
			return;
		
		score = score + addScore < MAX_SCORE ?  score + addScore : MAX_SCORE;
	}
	
	private void CleanUpGameObjectArray(GameObject[] array)
	{
		for (int i = 0; i < array.Length; i++) 
		{
			if(array[i] != null)
			{
				this.Destroy(array[i]);
				array[i] = null;
			}
		}
	}
}
