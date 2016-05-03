﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject[] pieces;
	public float fallStep = 4.0f;
	public float minX = -5.0f;
	public float maxX = 5.0f;
	public float initialWait;
	public Transform spawnTransform;

	private float width;
	private GameObject activePiece;
	private WaitForSeconds startWait;
	private WaitForSeconds moveWait;
	private bool lost = false;


	// Use this for initialization
	void Start () {
		width = maxX - minX;
		moveWait = new WaitForSeconds(initialWait);
		startWait = new WaitForSeconds (0f);
		StartCoroutine (GameLoop ());
	}

	GameObject RandomPiece () {
		return pieces [Random.Range (0, pieces.Length)];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator GameLoop() {
	
		yield return StartCoroutine (RoundStarting ());
		yield return StartCoroutine (RoundPlaying ());
	
	}

	IEnumerator RoundStarting () {
		yield return startWait;
	}

	IEnumerator RoundPlaying () {
		while (!lost) {
			if (activePiece == null) {
				Spawn (RandomPiece ());
			}
			yield return null;
		}
	}

	void Spawn (GameObject piece) {
		PieceManager pieceManager = piece.GetComponent<PieceManager>();
		float minXSpawn = minX + pieceManager.width / 2.0f;

		var spawnPointsLength = (int)(width - pieceManager.width) + 1;

		float[] spawnXPoints = new float[spawnPointsLength];

		for (float i = 0; i < spawnXPoints.Length; i++) {
			spawnXPoints [(int)i] = minXSpawn + i;
		}

		float spawnXPoint = spawnXPoints[Random.Range(0, spawnXPoints.Length)];

		Quaternion rotation = Quaternion.Euler (new Vector3 (0, 90, 0));

		activePiece = Instantiate (piece, new Vector3 (spawnXPoint, spawnTransform.position.y, 0f), rotation) as GameObject;
		pieceManager = activePiece.GetComponent<PieceManager>();
		pieceManager.moveWait = moveWait;
		pieceManager.BeginMotion ();
	}
}