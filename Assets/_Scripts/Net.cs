using UnityEngine;
using System.Collections;

public class Net : MonoBehaviour {


	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log(other.gameObject.name + " hit net");
		other.GetComponent<Rigidbody2D>().gravityScale = 0f;
		other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

}
