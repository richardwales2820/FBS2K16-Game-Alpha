using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

    The Controller has the following functions:

    - Instantiate the map, trees, capture points, the title page and buttons, and the final Game Over button
    - Store global static variables which will need to be accessed by other classes frequently

*/

public class Controller : MonoBehaviour {

    // Prefabs
    public GameObject factoryPrefab;
    public GameObject playerPrefab;
    public GameObject nodePrefab;
    public GameObject titlePrefab;
    public GameObject winnerPrefab;

    // Global variables
    public static Player player;
    public static Factory factory;
    public static GameObject title;

    public static Button archerButton;
    public static Button fighterButton;
    public static Button sorcererButton;

    // Game over boolean
    public static bool gameOver = false;

    // CapturePoints
    public static List<CapturePoint> capturePoints = new List<CapturePoint>();

    // Nodes
    public Node topLeft;
    public Node bottomRight;

    public static Node[,] nodeGrid;

    // Teams
    public Team redTeam;
    public Team blueTeam;

    public static List<Team> teams = new List<Team>();

    // SpawnPoints
    public GameObject spawns;
    public static List<SpawnPoint> redSpawnPoints = new List<SpawnPoint>();
    public static List<SpawnPoint> blueSpawnPoints = new List<SpawnPoint>();

    // Player spawning
    public static void SpawnPlayer() {

        // Create the player's unit depending on the team and unit selected
        if (player.team.id == 0) {  // Red Team

            int rand = player.classSelect;
            if (rand == 0) factory.CreateSorcerer(player.team, redSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
            else if (rand == 1) factory.CreateArcher(player.team, redSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
            else factory.CreateFighter(player.team, redSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);

        } else {  // Blue Team

            int rand = player.classSelect;
            if (rand == 0) factory.CreateSorcerer(player.team, blueSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
            else if (rand == 1) factory.CreateArcher(player.team, blueSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
            else factory.CreateFighter(player.team, blueSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
        }

        // If the player has not yet spawned (the player is just now beginning the game)
        if (!player.spawnedPlayer) {

            // Make sure each team has 6 units; the player is one of them
            int red = player.team.id == 0 ? 5 : 6;
            int blue = player.team.id == 1 ? 5 : 6;

            // Create random units on each team and spawn them at random spawn locations
            for (int i = 0; i < red; i++) {

                // Red Team
                int type = Random.Range(0, 3);

                if (type == 0) factory.CreateArcher(teams[0], redSpawnPoints[Random.Range(0, 11)]);
                else if (type == 1) factory.CreateFighter(teams[0], redSpawnPoints[Random.Range(0, 11)]);
                else if (type == 2) factory.CreateSorcerer(teams[0], redSpawnPoints[Random.Range(0, 11)]);
            }

            for (int i = 0; i < blue; i++) {

                // Blue Team
                int type = Random.Range(0, 3);

                if (type == 0) factory.CreateArcher(teams[1], blueSpawnPoints[Random.Range(0, 11)]);
                else if (type == 1) factory.CreateFighter(teams[1], blueSpawnPoints[Random.Range(0, 11)]);
                else if (type == 2) factory.CreateSorcerer(teams[1], blueSpawnPoints[Random.Range(0, 11)]);
            }
        }

        // Create Kill and Death counter
        player.killsText = factory.CreateWord("Kills ", new Vector3(factory.screenCanvas.transform.position.x - 220,
            factory.screenCanvas.transform.position.y + 195));

        player.deathsText = factory.CreateWord("Deaths ", new Vector3(factory.screenCanvas.transform.position.x - 170,
            factory.screenCanvas.transform.position.y + 195));

        player.killsText.transform.parent = factory.screenCanvas.transform;
        player.deathsText.transform.parent = factory.screenCanvas.transform;

        // Create Flag Cap counter
        for (int i = 0; i < capturePoints.Count; i++) {
            capturePoints[i].uiCaps = (Instantiate(factory.flagCapsPrefab, new Vector3(Camera.main.transform.position.x + (24 * (i - 2)), Camera.main.transform.position.y - 98), Quaternion.identity) as GameObject);
            capturePoints[i].uiCaps.transform.parent = Camera.main.transform;
        }

		player.spawnedPlayer = true;  // The player has begun the game
	}

	// Use this for initialization
	void Start () {

        // Create Factory
        factory = (Instantiate(factoryPrefab) as GameObject).GetComponent<Factory>();

		factory.worldCanvas = GameObject.Find ("World Canvas");
		factory.screenCanvas = GameObject.Find ("Screen Canvas");

        factory.topLeft = topLeft;
        factory.bottomRight = bottomRight;

        // Create Teams
        teams.Add(redTeam);
        teams.Add(blueTeam);

        // Build Node Map
        int xcount = 0;
        int ycount = 0;

        nodeGrid = new Node[(int)Mathf.Ceil((bottomRight.transform.position.x - topLeft.transform.position.x) / 16) + 1, (int)Mathf.Ceil((topLeft.transform.position.y - bottomRight.transform.position.y) / 16) + 1];

        // Place nodes on the grid
        for (float x = topLeft.transform.position.x; x <= bottomRight.transform.position.x; x += 16) {
            for (float y = bottomRight.transform.position.y; y <= topLeft.transform.position.y; y += 16) {

                // Create Node
                Node node = (Instantiate(nodePrefab, new Vector3(x, y), Quaternion.identity) as GameObject).GetComponent<Node>();

                // Assign the x and y array values to the Node
                node.x = xcount;
                node.y = ycount;

                // Place the Node into the Grid
                nodeGrid[xcount, ycount] = node;
                ycount++;
            }

            ycount = 0;
            xcount++;
        }

        // Spawn Points
        for (int i = 0; i < spawns.transform.childCount - 1; i++) {

            SpawnPoint sp = spawns.transform.GetChild(i).GetComponent<SpawnPoint>();

            if (sp.team == teams[0]) redSpawnPoints.Add(sp);
            else blueSpawnPoints.Add(sp);
        }

        // Title
        title = (Instantiate(titlePrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + 40, -9), Quaternion.identity) as GameObject);
        title.transform.parent = Camera.main.transform;

        // Create Player
        player = factory.CreatePlayer(teams[Random.Range(0, teams.Count)]); // Put player on a random team
        player.transform.position = Camera.main.transform.position;
        player.factory = factory;

        // PCG CapturePoints
        capturePoints = factory.GenerateAllCapturePoints(teams, nodeGrid);

        // PCG Map tiles, rocks, trees, and weeds
		factory.GenerateMap(nodeGrid, 12);
	}

    // Creates the Quit button when the game ends
    public static void GameOverButton() {

        // Instantiate the Button
        Button gameOverButton = (Instantiate(player.buttonPrefab, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -5), Quaternion.identity) as GameObject).GetComponent<Button>();

        // Assign Button attributes
        gameOverButton.player = player;
        gameOverButton.action = "gameover";
        Word quitWord = factory.CreateWord("Quit", new Vector3(gameOverButton.transform.position.x, gameOverButton.transform.position.y, -6));
        quitWord.transform.parent = gameOverButton.transform;
        gameOverButton.text = quitWord;

        // Set the Transform parent to that of the main Camera
        gameOverButton.transform.parent = Camera.main.transform;
    }
}
