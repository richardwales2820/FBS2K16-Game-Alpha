using UnityEngine;
using System.Collections;

public class BlazeIt : MonoBehaviour {

	// This class is simply used to associate the weeds with a class to be referenced from other objects
	// To check if the game object is a weed

	// Use this for initialization
	void Start () {

		// Change its sorting order to add correct depth
        GetComponent<SpriteRenderer>().sortingOrder = (int)-transform.position.y + 2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
