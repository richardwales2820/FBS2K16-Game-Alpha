using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	// A button has an associated animator, player, and what the text is that it displays
    public Animator animator;
    public Player player;
    public string action;
    public Word text;

    // Use this for initialization
    void Start () {
		// sets its coordinates for display and checks if anyone clicked it, set its sorting to the highest priority
        transform.position = new Vector3(transform.position.x, transform.position.y, -5);
        Physics.queriesHitTriggers = true;
        GetComponent<SpriteRenderer>().sortingOrder = 1000;
    }
	
	// Update is called once per frame
	void Update () {


    }

	// if the player mouses over the button, show the pressed animation
    void OnMouseOver() {

        if (Input.GetMouseButton(0)) {
            animator.SetBool("click", true);
        } else {
            animator.SetBool("click", false);
        }

        if (Input.GetMouseButtonUp(0)) {
            Action(player);
        }
    }

	// shows the hover animation
    void OnMouseEnter() {
       
        animator.SetBool("hover", true);
    }

	// On mouse exit, go back to the previous animation of un-hovered
    void OnMouseExit() {

        animator.SetBool("hover", false);
    }

	// Action applies to correct properties to the player depending on what button was pressed
    public void Action(Player player) {


		// If the action was a class value, set the players class to that selection
        if (action == "archer") {

            Destroy(Controller.title);

            player.selectedPlayer = true;
            player.classSelect = 1;

            if (!player.spawnedPlayer)
                Controller.SpawnPlayer();

            player.DestroyButtons();

        } else if (action == "fighter") {

            Destroy(Controller.title);

            player.selectedPlayer = true;
            player.classSelect = 2;

            if (!player.spawnedPlayer)
                Controller.SpawnPlayer();

            player.DestroyButtons();

        } else if (action == "sorcerer") {

            Destroy(Controller.title);

            player.selectedPlayer = true;
            player.classSelect = 0;

            if (!player.spawnedPlayer)
                Controller.SpawnPlayer();

            player.DestroyButtons();

        } else if (action == "gameover") { // Exits the game if the quit button was pressed

            Application.Quit();
        }
    }
}
