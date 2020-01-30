using UnityEngine;
using System.Collections;

public class bullet : MonoBehaviour {

	public GameObject explo;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}
	
	
	void OnCollisionEnter(Collision col) {
	
		GameObject.Instantiate(explo, col.contacts[0].point, Quaternion.identity);
	
		Destroy(gameObject);
	}
	
	
}
