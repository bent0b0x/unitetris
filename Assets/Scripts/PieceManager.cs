using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour {

	public float width = 2.0f;
	public float height;
	public WaitForSeconds moveWait = new WaitForSeconds(1.0f);

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
	private bool stationary = true;


	void Start () 
	{
		rb = gameObject.GetComponent<Rigidbody> ();}

		void Update () {
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
