using UnityEngine;
using System.Collections;

public class Levelup : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer>().sortingOrder = (int)-transform.position.y;
	}

	public void StopLevelup()
	{
		GameObject.Destroy (gameObject);
	}
}
