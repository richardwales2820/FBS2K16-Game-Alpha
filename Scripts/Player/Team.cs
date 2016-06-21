using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Team : MonoBehaviour {
	
	// ids determine the actual team that this team is associated with 
    public int id;
    public string teamName;

	//Caps hold score value
    public int caps = 0;
    public bool win = false;

	// used to display the winner, displayed if the game quits and a winner was decided
    public GameObject winnerPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		// if there are 5 caps by this team, set their win bool to true if it isnt yet
		// Also set the units to stop moving and instantiate the winner text value

        if (caps == 5 && !win) {
            win = true;
            Controller.gameOver = true;
            Controller.GameOverButton();
            if (Player.unit) Player.unit.Walk(false);

            Controller.player.DestroyButtons();
            Destroy(Controller.title);

            // Instantiate the Winner object
            GameObject winner = (Instantiate(winnerPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 30, -5), Quaternion.identity) as GameObject);

            if (id == 0) winner.GetComponent<Animator>().SetBool("red_wins", true);
            else if (id == 1) winner.GetComponent<Animator>().SetBool("blue_wins", true);

            winner.transform.parent = Camera.main.transform;
        }
	}

    // Checks if two teams are equal (determined by their ID)
    public bool compare(Team team) {

        if (id == team.id) return true;
        return false;
    }
}
