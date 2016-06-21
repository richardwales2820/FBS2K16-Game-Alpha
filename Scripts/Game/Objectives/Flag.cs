using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

	// Animator for the flag's animations
    public Animator animator;

	// Use this for initialization
	void Start () {
		// Sets the sorting order depending on the y position to add depth 
        GetComponent<Renderer>().sortingOrder = (int)-transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	// Capture the flag and set the animation to trigger
    public void capture(Team team) {

        if (team == Controller.teams[0]) animator.SetTrigger("red_cap");
        else if (team == Controller.teams[1]) animator.SetTrigger("blue_cap");
    }

	// Set the burning to trigger
    public void relinquish(Team team) {

        if (team == Controller.teams[0]) animator.SetTrigger("red_loss");
        else if (team == Controller.teams[1]) animator.SetTrigger("blue_loss");
    }
}
