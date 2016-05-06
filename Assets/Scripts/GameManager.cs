using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject[] pieces;
	public float fallStep = 4.0f;
	public float minX = -5.0f;
	public float maxX = 5.0f;
	public float initialMoveWait; // delay between an individual piece's movements
	public float initialPieceWait; // delay between stopping of one piece to when the next piece spawns
	public float shiftDelay; // delay between horizontal shifts
	public Transform spawnTransform;

	private float width;
	private GameObject activePiece;
	private WaitForSeconds startWait;
	private WaitForSeconds moveWait;
	private WaitForSeconds pieceWait;
	private bool lost = false;
	private float shiftTimer = 0f;
	private bool attemptingShift = false;

	// Use this for initialization
	void Start () 
	{
		width = maxX - minX;
		moveWait = new WaitForSeconds(initialMoveWait);
		pieceWait = new WaitForSeconds (initialPieceWait);
		startWait = new WaitForSeconds (0f);
		StartCoroutine (GameLoop ());
	}

	GameObject RandomPiece () 
	{
		return pieces [Random.Range (0, pieces.Length)];
	}
	
	void Update () 
	{
		shiftTimer += Time.deltaTime;

		if (activePiece != null) {
			PieceManager activePieceManager = activePiece.GetComponent<PieceManager> ();
			if (activePieceManager.stationary) {
				return;
			}
			float rotation = 0;
			bool clockwiseRotate = Input.GetButtonDown ("Fire1");
			if (clockwiseRotate) {
				rotation += 1.0f;
			}

			if (rotation != 0) {
				activePieceManager.Rotate (rotation);
			}

			float horizontal = Input.GetAxisRaw ("Horizontal");
			if (horizontal != 0) {
				if (shiftTimer > shiftDelay || !attemptingShift) {
					activePieceManager.AttemptShift (horizontal);
					shiftTimer = 0f;
				}
				attemptingShift = true;
			} else {
				attemptingShift = false;
			}
		}
	}

	IEnumerator GameLoop() 
	{
	
		yield return StartCoroutine (RoundStarting ());
		yield return StartCoroutine (RoundPlaying ());
	
	}

	IEnumerator RoundStarting () 
	{
		yield return startWait;
	}

	IEnumerator RoundPlaying () 
	{
		while (!lost) {
			if (activePiece == null) {
				SpawnRandom ();
			}
			yield return null;
		}
	}

	void Spawn (GameObject piece) 
	{
		PieceManager pieceManager = piece.GetComponent<PieceManager>();
		float minXSpawn = minX + pieceManager.width / 2.0f;

		var spawnPointsLength = (int)(width - pieceManager.width) + 1;

		float[] spawnXPoints = new float[spawnPointsLength];

		for (float i = 0; i < spawnXPoints.Length; i++) {
			spawnXPoints [(int)i] = minXSpawn + i;
		}

		float spawnXPoint = spawnXPoints[Random.Range(0, spawnXPoints.Length)];

		float spawnYPoint = spawnTransform.position.y + pieceManager.height / 2.0f;

		Quaternion rotation = Quaternion.Euler (new Vector3 (0f, 90f, 0f));

		activePiece = Instantiate (piece, new Vector3 (spawnXPoint, spawnYPoint, 0f), rotation) as GameObject;
		pieceManager = activePiece.GetComponent<PieceManager>();
		pieceManager.gameManager = this;
		pieceManager.moveWait = moveWait;
		pieceManager.BeginMotion ();
	}

	void SpawnRandom ()
	{
		Spawn (RandomPiece ());
	}

	public IEnumerator HandlePieceStationary (GameObject piece) 
	{
		yield return pieceWait;
		SpawnRandom ();
	}
}
