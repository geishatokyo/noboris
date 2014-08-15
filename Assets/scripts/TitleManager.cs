using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour
{
	public float minoSpawnInterval = 1.0f;

	public float anounceStrTop;
	public float anounceStrLeft;
	public float anounceStrWidth;
	public string anounceStr = "please touch!";
	public GUIStyle anounceStrStyle;
	public float flashSpeed = 1.0f;
	private float flashTime;

	private GameObject[] MinoObj;

	// Use this for initialization
	void Start () 
	{
		flashTime = 0;

		MinoObj = new GameObject[6];
		MinoObj[0] = (GameObject)Resources.Load ("Falling_I_Mino");
		MinoObj[1] = (GameObject)Resources.Load ("Falling_L_Mino");
		MinoObj[2] = (GameObject)Resources.Load ("Falling_J_Mino");
		MinoObj[3] = (GameObject)Resources.Load ("Falling_Z_Mino");
		MinoObj[4] = (GameObject)Resources.Load ("Falling_S_Mino");
		MinoObj[5] = (GameObject)Resources.Load ("Falling_T_Mino");

		GUITexture tex = GameObject.Find ("titleLogo").GetComponent<GUITexture> ();
		tex.pixelInset = new Rect (-0.23f * Screen.width, -40, Screen.width * 0.45f, 0.25f * Screen.width);

		StartCoroutine ("SpawnTetliMino");
	}
	
	// Update is called once per frame
	void Update () 
	{
		flashTime += Time.deltaTime;

		if (Input.GetMouseButtonUp(0))
		{
			Application.LoadLevel ("Main");
		}
	}

	void OnGUI()
	{
		if(flashTime > flashSpeed)
		{
			anounceStrStyle.fontSize = (int)(Screen.width * anounceStrWidth / 100);
			GUILayout.BeginArea(new Rect(Screen.width * anounceStrLeft / 100, 
			                             Screen.height * anounceStrTop / 100,
			                    		 anounceStrStyle.CalcSize(new GUIContent(anounceStr)).x,
			                             anounceStrStyle.CalcSize(new GUIContent(anounceStr)).y));
			
			GUILayout.Label(anounceStr, anounceStrStyle);
			GUILayout.EndArea();
			
			if (flashTime > flashSpeed * 2)
				flashTime = 0;
		}
	}

	IEnumerator SpawnTetliMino()
	{
		while(true)
		{
			GameObject.Instantiate(MinoObj[Random.Range(0, 5)],
			                       new Vector3(Random.Range(-3, 3), 4.0f, Random.Range(-3, 3)),
			                       Quaternion.identity);

			yield return new WaitForSeconds(minoSpawnInterval); 
		}
	}
}
