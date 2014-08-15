using UnityEngine;
using System.Collections;

public class UnitychanController : MonoBehaviour 
{
	private readonly float STEP_RANGE_MIN = 0.6f;
	private readonly float STEP_OFFSET = 0.6f;
	private readonly float MAX_STEP_GRADE = 2;
	private readonly float MIN_STEP_GRADE = 0;

	private readonly float SPEED_RANGE_MIN = 0.02f;
	private readonly float SPEED_RANGE_OFFSET = 0.015f;
	private readonly float MAX_SPEED_GRADE = 2;
	private readonly float MIN_SPEED_GRADE = 0;

	private readonly float gravity = 0.0005f;
	private float horizonMovement = 0.005f;
	
	private GameObject gameManager;
	private CharacterController characterController;
	private Animator animator;
	private bool movingFlag;
	private bool noDamageTime = false;

	private Vector3 vVelocity;
	private Vector3 hVelocity;

	private float maxSpeed;
	private int speedGrade;
	private int stepGrade;

	// audio 
	private readonly int VOICE_NUM = 3;
	private readonly int DAMAGE_VOICE = 0;
	private readonly int ITEM_GET_VOICE = 1;
	private readonly int BADITEM_GET_VOICE = 2;
	private AudioSource player;
	private AudioClip[] se;

	// Use this for initialization
	void Start () 
	{
		gameManager = GameObject.Find("GameManager");
 		characterController = GetComponent<CharacterController> ();
		animator = GetComponent<Animator>();

		movingFlag = false;

		vVelocity = Vector3.zero;
		hVelocity = Vector3.zero;
		
		characterController.stepOffset = STEP_RANGE_MIN;
		
		maxSpeed = SPEED_RANGE_MIN;
		speedGrade = 0;
		stepGrade = 0;
		
		horizonMovement = 0.005f;

		player = this.GetComponent<AudioSource> ();
		se = new AudioClip[VOICE_NUM];
		se[DAMAGE_VOICE] = (AudioClip)Resources.Load("se/damage");
		se[ITEM_GET_VOICE] = (AudioClip)Resources.Load("se/item_get");
		se[BADITEM_GET_VOICE] = (AudioClip)Resources.Load("se/baditem_get");
	}

	// Update is called once per frame
	void Update () 
	{
		if (!characterController.isGrounded)
		{
			vVelocity.y -= gravity;
			characterController.Move (vVelocity + hVelocity);
		} 
		else 
		{
			if(movingFlag)
			{
				if(vVelocity.y != 0)
				{
					vVelocity.y = 0;
				}

				if(Mathf.Abs(hVelocity.x) < maxSpeed)
				{
					hVelocity.x += horizonMovement;
				}

				CollisionFlags flag = characterController.Move(hVelocity);

				if((flag & CollisionFlags.Sides) == CollisionFlags.Sides)
				{
					hVelocity = Vector3.zero;
					ChangeDirection();
				}

				this.transform.LookAt (hVelocity + this.transform.position);
			}
		}

		animator.SetFloat ("hVelocity", Mathf.Abs(hVelocity.x));
		animator.SetFloat ("vVelocity", Mathf.Abs (vVelocity.y));

		if (characterController.isGrounded)
			noDamageTime = false;
	}

	void ChangeDirection()
	{
		hVelocity.x = 0;
		horizonMovement = -horizonMovement;
	}

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		//Debug.Log ("hit: " + hit.gameObject.tag);
		if (hit.gameObject.tag == "DeleteObjectArea") 
		{
			if(!noDamageTime && movingFlag)
				Respawn();

			// this write gamemanager send process
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		//Debug.Log ("hit: " + hit.gameObject.tag);
		if(hit.gameObject.tag == "Item_StepUp")	// ***** item collision ***** //
		{
			player.PlayOneShot (se[ITEM_GET_VOICE]);

			MonoBehaviour.Destroy(hit.gameObject);
			if(stepGrade < MAX_STEP_GRADE)
				stepGrade++;
			
			characterController.stepOffset = STEP_RANGE_MIN + STEP_OFFSET * stepGrade;
		}
		else if(hit.gameObject.tag == "Item_StepDown")
		{
			player.PlayOneShot (se[BADITEM_GET_VOICE]);

			MonoBehaviour.Destroy(hit.gameObject);
			if(stepGrade > MIN_STEP_GRADE)
				stepGrade--;
			
			characterController.stepOffset = STEP_RANGE_MIN + STEP_OFFSET * stepGrade;
		}
		else if(hit.gameObject.tag == "Item_SpeedUp")
		{
			player.PlayOneShot (se[ITEM_GET_VOICE]);

			MonoBehaviour.Destroy(hit.gameObject);
			if(speedGrade < MAX_SPEED_GRADE)
				speedGrade++;
			
			maxSpeed = SPEED_RANGE_MIN + SPEED_RANGE_OFFSET * speedGrade;
		}
		else if(hit.gameObject.tag == "Item_SpeedDown")
		{
			player.PlayOneShot (se[BADITEM_GET_VOICE]);

			MonoBehaviour.Destroy(hit.gameObject);
			if(speedGrade > MIN_SPEED_GRADE)
				speedGrade--;
			
			maxSpeed = SPEED_RANGE_MIN + SPEED_RANGE_OFFSET * speedGrade;
		}
		else if(hit.gameObject.tag == "Item_Heart")
		{
			player.PlayOneShot (se[ITEM_GET_VOICE]);

			MonoBehaviour.Destroy(hit.gameObject);
			gameManager.SendMessage("UpdateLife", GameManager.UpdateMode.Increment);
		}
		else if(hit.gameObject.tag == "Item_Bomb")
		{
			MonoBehaviour.Destroy(hit.gameObject);
			gameManager.SendMessage("UpdateBomb", GameManager.UpdateMode.Increment);
		}
		else if(hit.gameObject.tag == "Item_ScoreUp")
		{
			player.PlayOneShot (se[ITEM_GET_VOICE]);

			MonoBehaviour.Destroy(hit.gameObject);
			gameManager.SendMessage("ScoreUpdate", ScoreValue.ITEM_SCORE);
		}
	}

	void StartMoving()
	{
		movingFlag = true;
		player.volume = 1;
	}

	void StopMoving()
	{
		movingFlag = false;
		player.volume = 0;
	}

	void Respawn()
	{
		noDamageTime = true;

		player.PlayOneShot (se[DAMAGE_VOICE]);

		gameManager.SendMessage ("UpdateLife", GameManager.UpdateMode.Decrement);
		stepGrade = 0;
		speedGrade = 0;

		characterController.stepOffset = STEP_RANGE_MIN + STEP_OFFSET * stepGrade;
		maxSpeed = SPEED_RANGE_MIN + SPEED_RANGE_OFFSET * speedGrade;
		
		hVelocity = Vector3.zero;
		vVelocity = Vector3.zero;

		this.transform.LookAt (this.transform.position + new Vector3(0, 0, -1.0f));

		this.transform.position = new Vector3(2.6f, 5.4f, 0.0f);
	}
}
