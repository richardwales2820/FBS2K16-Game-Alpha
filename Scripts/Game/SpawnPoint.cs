using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	// The team associated with the spawn point
    public Team team;

	// Use this for initialization
	void Start () {

		// Hide it from view from the player
        GetComponent<Renderer>().enabled = false;  // hide spawnpoints
    }
	
	// Update is called once per frame
	void Update () {
	
	}


}
