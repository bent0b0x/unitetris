﻿using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour {

	public float width = 2.0f;
	public float height;
	public WaitForSeconds moveWait = new WaitForSeconds(1.0f);
	[HideInInspector] public bool stationary = true;


	public GameManager gameManager;

	[HideInInspector] public bool Stationary
	{
		get 
		{
			return stationary;
		}
		set 
		{
			bool previous = stationary;
			stationary = value;
			if (!previous && value && gameManager != null) {
				StartCoroutine (gameManager.HandlePieceStationary (gameObject));
			}
		}
	}


	private Rigidbody rb;
	private bool inverted = false;


	void Start () 
	{
		rb = gameObject.GetComponent<Rigidbody> ();
	}
		

	public void BeginMotion () 
	{
		Stationary = false;
		StartCoroutine (Move ());
	}

	// Recursively moves the piece down until it is stationary
	IEnumerator Move () 
	{
		yield return moveWait;
		if (!stationary) {
			transform.position = new Vector3 (transform.position.x, transform.position.y - 1, transform.position.z);
			yield return Move ();
		}
	}

	public void Rotate (float direction)
	{
		inverted = !inverted;
		transform.Rotate (90.0f * direction, 0, 0);
		if (height % 2 != width % 2) {
			Vector3 newPosition = new Vector3 (transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z);
			if (inverted) {
				newPosition.x += 1.0f;
			}
			transform.position = newPosition;
		}

	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.CompareTag ("Floor")) {
			Stationary = true;
			return;
		}
		if (other.CompareTag ("PieceComponent")) {

			// Generate Raycast from center of the other collider, downwards.
			// If we hit this game piece, then we know we need to stop moving.
			// This check is necessary to avoid false collision on corners.
			Vector3 rayOrigin = other.bounds.center;
			Vector3 direction = new Vector3 (0f, -1.0f, 0f);
			Ray pieceRay = new Ray (rayOrigin, direction);

			RaycastHit pieceHit;

			if (Physics.Raycast (pieceRay, out pieceHit, 1.0f)) {
				if (pieceHit.rigidbody != null && pieceHit.rigidbody.Equals(rb)) {
					Stationary = true;
					PieceManager otherManager = other.gameObject.GetComponentInParent<PieceManager> ();
					if (otherManager != null) {
						otherManager.Stationary = true;
					}
					return;
				}
			}
		}
	}

}
