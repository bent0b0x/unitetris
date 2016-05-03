using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour {

	public float width = 2.0f;
	public WaitForSeconds moveWait = new WaitForSeconds(1.0f);

	// Use this for initialization

		

	
	// Update is called once per frame
	void Update () {
	}

	public void BeginMotion () {
		StartCoroutine (Move ());
	}

	IEnumerator Move () {
		yield return moveWait;
		transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
		yield return Move ();
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log (other.ToString());
	}
}
