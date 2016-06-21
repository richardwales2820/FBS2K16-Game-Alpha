using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    // Prefabs
    public GameObject buttonPrefab;

    // Factory
    public Factory factory;

    // Unit being controlled by Player
    public static Unit unit;

    // Player's team
    public Team team;

    // Player ScreenName
    public string screenName;

    // Button
    public bool buttons = false;

    // K/D
    public int kills;
    public int deaths;

	// UI for K/D
	public Word killsText;
	public Word deathsText;

    // Booleans
    private bool walking;

	// bools for if the player has a class selected and spawned
	public bool selectedPlayer = false;
	public bool spawnedPlayer = false;

	// int detects the class
	public int classSelect = 0;

	// Use this for initialization
	void Start () {

		// Create the start buttons for choosing classes
        CreateButtons();
	}

    public void CreateButtons() {

		// if no buttons have been placed
        if (!buttons) {
            // Archer Button

			// Instantiate each class button. The code below for each class button is identical except for the identifiers for specific classes
            Button archerButton = (Instantiate(buttonPrefab, new Vector3(Camera.main.transform.position.x - 40, Camera.main.transform.position.y - 25, -5), Quaternion.identity) as GameObject).GetComponent<Button>();

			// Set the archer button's player reference to this player class
            archerButton.player = this;

			// set the words for the class using the Word library created by Ty
			// Has a word sprite generated from words passed to it

            archerButton.action = "archer";
            Word archerWord = factory.CreateWord("Archer", new Vector3(archerButton.transform.position.x, archerButton.transform.position.y, -6));
            archerWord.transform.parent = archerButton.transform;
            archerButton.text = archerWord;

            archerButton.transform.parent = Camera.main.transform;

            Controller.archerButton = archerButton;

            // Fighter Button
            Button fighterButton = (Instantiate(buttonPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 25, -5), Quaternion.identity) as GameObject).GetComponent<Button>();

            fighterButton.player = this;
            fighterButton.action = "fighter";
            Word fighterWord = factory.CreateWord("Knight", new Vector3(fighterButton.transform.position.x, fighterButton.transform.position.y, -6));
            fighterWord.transform.parent = fighterButton.transform;
            fighterButton.text = fighterWord;

            fighterButton.transform.parent = Camera.main.transform;

            Controller.fighterButton = fighterButton;

            // Sorcerer Button
            Button sorcererButton = (Instantiate(buttonPrefab, new Vector3(Camera.main.transform.position.x + 40, Camera.main.transform.position.y - 25, -5), Quaternion.identity) as GameObject).GetComponent<Button>();

            sorcererButton.player = this;
            sorcererButton.action = "sorcerer";
            Word sorcererWord = factory.CreateWord("Sorcerer", new Vector3(sorcererButton.transform.position.x, sorcererButton.transform.position.y, -6));
            sorcererWord.transform.parent = sorcererButton.transform;
            sorcererButton.text = sorcererWord;

            sorcererButton.transform.parent = Camera.main.transform;

            Controller.sorcererButton = sorcererButton;

            buttons = true;
        }
    }

    public void DestroyButtons() {

		// If there are buttons available, destroy them and set the bool value to false
        if (buttons) {
            Controller.archerButton.text.DestroyWord();
            Controller.fighterButton.text.DestroyWord();
            Controller.sorcererButton.text.DestroyWord();

            Destroy(Controller.archerButton.gameObject);
            Destroy(Controller.fighterButton.gameObject);
            Destroy(Controller.sorcererButton.gameObject);

            buttons = false;
        }
    }

	// Update is called once per frame
	void Update () {

        if (unit) {  // Ensure the Player is associated with a Unit

			// Set the position of the prefab to the associated units position
			transform.position = unit.transform.position;
          
			// If the unit is controllable, then get input from the player and move to the right place
            if (unit.controllable) {

                // Get movement input
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                // Facing Direction
                if (horizontal > 0) {
                    unit.transform.localRotation = Quaternion.Euler(0, 0, 0);  // Face right
                } else if (horizontal < 0) {
                    unit.transform.localRotation = Quaternion.Euler(0, 180, 0);  // Face left
                    horizontal = -horizontal;
                }

                Vector3 move = new Vector3(horizontal, vertical, 0) * unit.speed;

                // Fixes diagonal movement
                Vector3.ClampMagnitude(move, unit.speed);

                // Determine if the player is walking
                if (horizontal == 0 && vertical == 0) {
                    walking = false;
                } else {
                    walking = true;
                }

                unit.transform.Translate(move);  // Move unit
                // Walking animation
                unit.Walk(walking);

                // Spawn projectiles for archers and sorcerers
                if (Input.GetKeyDown(KeyCode.Space)) {
                    unit.Attack();
                }

				if (Input.GetKeyDown (KeyCode.P)) //Force respawn
					unit.Kill ();

            } else {
				// Dont move if no movement. Bring up the selection movement if period is pressed then
                unit.transform.Translate(Vector3.zero);
            }

            if (Input.GetKeyDown(KeyCode.Period)) {  //Brings up selection menu
                if (!buttons) CreateButtons();
                else if (buttons) DestroyButtons();
            }
        }

		// Stop the game and show the quit button to exit

        if (Input.GetKeyDown(KeyCode.Q)) {

            Controller.gameOver = true;
            Controller.GameOverButton();
            if (unit) unit.Walk(false);

            Controller.player.DestroyButtons();
            Destroy(Controller.title);

            // Instantiate the Winner object
            GameObject winner = (Instantiate(team.winnerPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 30, -5), Quaternion.identity) as GameObject);

            if (team.id == 0) winner.GetComponent<Animator>().SetBool("blue_wins", true);
            else if (team.id == 1) winner.GetComponent<Animator>().SetBool("red_wins", true);

            winner.transform.parent = Camera.main.transform;
        }
	}
}
