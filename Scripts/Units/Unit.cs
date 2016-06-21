// Unit

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

/*

    The Unit class serves the following purposes:

    - Serves as a superclass for the Fighter, Archer, and Sorcerer classes
    - Stores all information regarding a Unit such as health, animations, level, stats, and AI handling

*/

public abstract class Unit : MonoBehaviour {

    // Prefab
    public GameObject prefab;

    // Team
    public Team team;

    // Class
    public string className;

    // Animator and Animation
    public Animator animator;
    public Animation anim;

    // Animation Triggers
    public string trigger_walk;
    public string trigger_attack1;

    // Level
    public int level = 0;
	public GameObject levelupAnim;

    // Kills
    public int kills = 0;

	///////////
	// Stats //
	///////////
	
	// Health
	public float maxhp;  // Maximum HP
	public float hp;  // Current HP
	
	// Attack and Defense
	public float attack;  // Damage
	public float defense;  // Damage reduction

    // Special Attributes
    public bool ignoreArmor;

    // AI Attributes
    public float attackDist;  // Distance AI will attack from
    public float minDistance;  // Minimum distance AI is comfortable standing from another Unit
    public float maxDistance;  // Distance at which an AI will give up chasing
    public float yvariant;  // Vertical range at which the AI will attempt to attack
	
	// Speed
	public float speed;  // Movespeed

    // Special Stats
    public float controlRate = 0.025f;  // Rate at which this Unit will influence the control percentage of a Control Point
    public float cooldownTime;  // Cooldown

	// Player Controller
	public Player player;  // Remains null if this is an AI

    // AI
    public Unit target;  // AI Target Unit


    // Booleans
    public bool controllable = true;
    public bool alive = true;
    public bool canAttack = true;
    public bool blocked = false;

    // Path
    public Node curNode;
    public Path path;

    // Capture Point Target
    public CapturePoint capturePoint;

    // UI Elements
    public Text health;
    public Word screenWord;
	public Word levelWord;
	public GameObject healthbar;

	// Audio
	public AudioController audioc;

    // Called when this Unit is instantiated
    void Start() {

        // Get Animator and Animation components
        animator = GetComponent<Animator>();
        anim = GetComponent<Animation>();

        // Healthbar
        health = GameObject.Find("Health").GetComponent<Text>();

        // Audio Controller
		audioc = GetComponent<AudioController> ();
    }

    // Called on each frame
    void Update() {

        // Update the healthbar location
		if (!healthbar) {
			healthbar = Instantiate (Controller.factory.unitHealthbarPrefab, transform.position, Quaternion.identity) as GameObject;
			healthbar.transform.parent = Controller.factory.worldCanvas.transform;
		}

        // Update healthbar fill value
		if (healthbar) {
			Image[] images = healthbar.GetComponentsInChildren<Image> ();
			foreach (Image i in images)
				i.fillAmount = hp / maxhp;
			
			healthbar.transform.position = new Vector3(transform.position.x, transform.position.y + 14, transform.position.z);
		}

        // Update unit sorting order for depth purposes
        GetComponent<Renderer>().sortingOrder = (int)-transform.position.y;

        // Allow the unit's name to follow the Unit
        if (screenWord) {
            screenWord.transform.position = new Vector3(transform.position.x, transform.position.y + 23, transform.position.z);
        }

        // Allow the unit's level to follow the Unit
		if (levelWord) {
			levelWord.transform.position = new Vector3(transform.position.x - 12, transform.position.y + 17, transform.position.z);
		}

        // If the game is over, disable the Unit's controls
        if (Controller.gameOver) {
            DisableControls();
        }
    }

    // Allows and disallows a Player to control this Unit
    public void PlayerControl(bool toggle) {

        if (toggle) {
            player = Controller.player;
            Player.unit = this;
            EnableControls();
        } else {
            player = null;
        }
    }

    // Disallows this Unit to be controlled by the AI or the Player
    public void DisableControls() {
        controllable = false;
    }

    // Allows this Unit to be controlled by the AI or the Player
    public void EnableControls() {
        controllable = true;
    }

    // Plays the animation for the attack and sets the cooldown for the AI
    // Attack is overriden by Unit subclasses
    public virtual void Attack() {

        canAttack = false;
        animator.SetTrigger(trigger_attack1);
        StartCoroutine(Cooldown());
    }

    // Toggle the walking animation
    public void Walk(bool walking) {

        animator.SetBool(trigger_walk, walking);
    }

    // Move an AI to a Vector2 location
    public void MoveTo(Vector2 pos) {

        if (controllable) {

            if (pos.x > transform.position.x) {
                Turn(Quaternion.Euler(0, 0, 0));
            } else if (pos.x < transform.position.x) {
                Turn(Quaternion.Euler(0, -180, 0));
            }

            transform.position = Vector2.MoveTowards(transform.position, pos, speed);
            Walk(true);
        }
    }

    // Force an AI to face a certain direction
    public void Turn(Quaternion rotation) {
        
        if (controllable) {
            transform.rotation = rotation;
        }
    }

    // Delayed projectile spawn
    // Overriden by by Unit subclasses
    public virtual IEnumerator SpawnProjectile() {

        yield break;
    }

    // Apply Damage to this Unit from a source Unit
    public void ApplyDamage(Unit source,  float damage) {

        float def = defense;

        // Ignore Armor check
        if (source.ignoreArmor) def = 0;

        if (alive) {

            // Turn this unit red for a short period of time to show they were hit
            StartCoroutine(HitColor());

            // Set this Unit's HP to a minimum of zero
            hp = Mathf.Clamp(hp - (damage - def), 0, Mathf.Infinity);

            // If this Unit's HP is zero...
            if (hp == 0) {

                // ... Kill this unit
                Kill();

                source.kills++;  // Update the source's kill counter for leveling purposes

                if (source.alive) {
                    source.CheckLevelUp ();  // Check to see if this Unit has leveled up
                }
			
                // Player stats
				if (source.player) {

					source.player.kills++;  // Increment the source's kills as a player

                    // Update Kill counter
					source.player.killsText.DestroyWord ();
					string killsString = "Kills " + source.player.kills;
					source.player.killsText = Controller.factory.CreateWord (killsString, new Vector3 (Controller.factory.screenCanvas.transform.position.x - 220, 
						Controller.factory.screenCanvas.transform.position.y + 195));
					source.player.killsText.transform.parent = Controller.factory.screenCanvas.transform;
				}
            }
        }

        if (player) {
            CameraFollow.ShakeCamera((damage * damage) / 10);  // Camera shake
        }

        // Aggro
        if (!player) {  // If this is an AI
            if (target) {  // If this AI has a target already
                
                // If this AI Unit was just hit by a source Unit which is closer than its current target...
                if (Vector2.Distance(transform.position, source.transform.position) < Vector2.Distance(transform.position, target.transform.position)) {
                    target = source;  // ... make this new Unit its current target
                }
            } else {

                target = source;  // If this Unit doesn't already have a target, make the source its target

                // Play "Huh?" sound when unit is not pursuing someone
				audioc.PlayHuhSound();
            }
        }
    }

    // Changes color to red temporarily when taking damage
    public IEnumerator HitColor() {
		audioc.PlayHitSound ();
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<Renderer>().material.color = Color.white;
    }

    // Called when this Unit gets killed
    public void Kill() {

        // Disable controls and rotate the body
        canAttack = false;
        DisableControls();
        transform.Rotate(Vector3.forward * 90);
        animator.speed = 0;
        alive = false;

		healthbar.SetActive (false);

        // Destroy the name and level
        screenWord.DestroyWord();
        screenWord = null;

		levelWord.DestroyWord ();
		levelWord = null;

		Destroy (levelWord);

        // If this Unit was leveling up, destroy its level up animation immediately
		if (levelupAnim)
			Destroy (levelupAnim);

        // Turn this Unit into a skeleton and play the death sound
        animator.Play("dead");
		audioc.PlayDeathSound ();

        // Start the respawn timer and the body destruction timer
        StartCoroutine(RespawnDelay());
        StartCoroutine(BodyDespawn());

        // Destroy this dead Unit's box collider
		GetComponent<BoxCollider2D> ().enabled = false;

		if (player) {  // If this is a player...

			player.deaths++;  // Increment their deaths

            // Update the death counter
			player.deathsText.DestroyWord ();
			string deathString = "Deaths " + player.deaths;
			player.deathsText = Controller.factory.CreateWord (deathString, new Vector3(Controller.factory.screenCanvas.transform.position.x - 170, 
				Controller.factory.screenCanvas.transform.position.y + 195));

			player.deathsText.transform.parent = Controller.factory.screenCanvas.transform;
		}

    }

    // Check if this Unit can level up
    public void CheckLevelUp() {

        if (kills == 2) {
            LevelUp(1);
        } else if (kills == 5) {
            LevelUp(2);
        } else if (kills == 10) {
            LevelUp(3);
        }
    }

    // Level up
    // Overriden by Unit subclasses
    public virtual void LevelUp(int level) {

        // Play Level Up animation
		levelupAnim = Instantiate (Controller.factory.levelupPrefab, transform.position, Quaternion.identity) as GameObject;
		levelupAnim.transform.parent = transform;

        // Update level Word
		if (levelWord)
			levelWord.DestroyWord ();
		
		string levelString = "" + (level + 1);
		levelWord = Controller.factory.CreateWord (levelString, new Vector3 (transform.position.x - 12, transform.position.y + 17, transform.position.z)); 

        // Increase capture rate
        controlRate += 0.0125f;

		// Play level up sound
		audioc.PlayLevelupSound();
    }

    // Respawn Delay
    public IEnumerator RespawnDelay() {

        // Wait for 7 seconds
        yield return new WaitForSeconds(7.0f);
        
        // If this is a Player, create the Unit which this player has selected as their class
        if (player) {
            if (team.id == 0) {  // Red Team
				int rand = player.classSelect;
                if (rand == 0) Controller.factory.CreateSorcerer(team, Controller.redSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
                else if (rand == 1) Controller.factory.CreateArcher(team, Controller.redSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
                else Controller.factory.CreateFighter(team, Controller.redSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
            } else {  // Blue Team
				int rand = player.classSelect;
                if (rand == 0) Controller.factory.CreateSorcerer(team, Controller.blueSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
                else if (rand == 1) Controller.factory.CreateArcher(team, Controller.blueSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
                else Controller.factory.CreateFighter(team, Controller.blueSpawnPoints[Random.Range(0, 11)]).PlayerControl(true);
            }

            // Play spawn noise and no longer allow the Player to control this Unit
			audioc.PlayPowerUpSound ();
            PlayerControl(false);

        } else {

            if (team.id == 0) {  // Red Team
                int rand = Random.Range(0, 3);
                if (rand == 0) Controller.factory.CreateSorcerer(team, Controller.redSpawnPoints[Random.Range(0, 11)]);
                else if (rand == 1) Controller.factory.CreateArcher(team, Controller.redSpawnPoints[Random.Range(0, 11)]);
                else Controller.factory.CreateFighter(team, Controller.redSpawnPoints[Random.Range(0, 11)]);
            } else {  // Blue Team
                int rand = Random.Range(0, 3);
                if (rand == 0) Controller.factory.CreateSorcerer(team, Controller.blueSpawnPoints[Random.Range(0, 11)]);
                else if (rand == 1) Controller.factory.CreateArcher(team, Controller.blueSpawnPoints[Random.Range(0, 11)]);
                else Controller.factory.CreateFighter(team, Controller.blueSpawnPoints[Random.Range(0, 11)]);
            }
        }
    }

    // Delayed dead body destruction
    public IEnumerator BodyDespawn() {

        yield return new WaitForSeconds(180.0f);  // Destroy body after 3 minutes
        Destroy(gameObject);
    }

    // Cooldown for AI to attack
    public IEnumerator Cooldown() {

        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }

    // A* pathfinding for AI
    public Path AStar(Node start, Node end) {

        // Initialize G, H and F values for the starting Node
        start.G = 0;
        start.H = Vector3.Distance(start.transform.position, end.transform.position);
        start.F = start.H;

        Node cur = start;  // The current Node is initalized as the start Node

        Stack<Node> openStack = new Stack<Node>();
        Stack<Node> closedStack = new Stack<Node>();

        while (cur != end) {

            List<Node> nodes = cur.GetAdjacentNodes();  // Get the Nodes adjacent to the current Node

            for (int i = 0; i < nodes.Count; i++) {

                Node adj = nodes[i];  // Get this adjacent Node

                if (adj.open == 0) {  // If this Node has yet to be opened...

                    // Open the Node and set the parent
                    adj.open = 1;
                    adj.parent = cur;

                    // Calculate G, H, and F values
                    adj.G = cur.G + Vector3.Distance(cur.transform.position, adj.transform.position);
                    adj.H = Vector3.Distance(adj.transform.position, end.transform.position);
                    adj.F = adj.G + adj.H;

                    // Push to the Open Stack
                    openStack.Push(adj);

                } else if (adj.open == 1) {  // If this Node has already been opened...

                    // Check if it is more efficient to travel by this Node from the cur Node
                    float G = cur.G + Vector3.Distance(cur.transform.position, adj.transform.position);

                    if (G < adj.G) {  // If this path is more efficient...
                        adj.G = G;  // Update the G value
                        adj.F = G + adj.H;  // Update the F value
                        adj.parent = cur;  // Update parent
                    }
                }

                if (adj == end) break;  // If this is the end Node, end the search
            }

            // Sort the openStack by F value
            openStack = new Stack<Node>(openStack.OrderByDescending(node => node.F));

            // Close the current Node
            closedStack.Push(cur);  // Push the current Node onto the closed Stack
            cur.open = -1;  // Make sure the Node knows it is closed

            // If there are no more open Nodes, there is no possible Path
            if (openStack.Count == 0) {
                return null;  // No Path is possible
            } else {
                // Make the lowest F value Node the next Node to check
                cur = openStack.Pop();
            }
        }

        // Generate the List of Node in the correct Path order
        Node n = end;
        List<Node> nodePath = new List<Node>();

        // Starting with the end Node, find the parent until the parent is null (start Node)
        while (n != null) {
            nodePath.Add(n);
            n = n.parent;
        }

        nodePath.Reverse();  // Reverse the List

        // Generate the Path
        Path path = Controller.factory.CreatePath(nodePath, this);

        // Get the width and height of the Node Grid
        int width = Controller.nodeGrid.GetLength(0) - 1;
        int height = Controller.nodeGrid.GetLength(1) - 1;

        // Once the Path has been generated, reset the A* temporary variables for each Node
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Controller.nodeGrid[i, j].open = 0;
                Controller.nodeGrid[i, j].parent = null;
            }
        }

        return path;
    }

    // Check for Node collision
    void OnTriggerStay2D(Collider2D coll) {

        Node node = coll.GetComponent<Node>();

        if (node) {  // If this is a Node...
            if (path) {
                if (node == path.curNode) {  // If this is the Node this Unit's Path is looking for...
                    if (path.nodeIndex + 1 < path.nodes.Count) {  // If this isn't the end of the Path
                        path.nodeIndex++;  // Increment the Path's curernt Node
                        path.curNode = path.nodes[path.nodeIndex];
                    } else path = null;  // If this is the end of the Path, nullify the Path

                    CapturePoint cap = FindCapturePoint();  // Find the closest capture point

                    if (cap && cap != capturePoint) {  // If this isn't the capture point the AI is currently targeting..
                        if (curNode) {
                            capturePoint = cap;
                            path = AStar(curNode, cap.node);  // Set this as the new target via A*
                        }
                    }
                }
            }

            // Set the currently triggered Node as the Unit's current Node as long as it is unobstructed
            if (!node.obstructed) {
                curNode = node;
            }
        }
    }

    // Finds the closest capture point to the Unit
    public CapturePoint FindCapturePoint() {

        float min = Mathf.Infinity;
        CapturePoint cap = null;
        for (int i = 0; i < Controller.capturePoints.Count; i++) {

            CapturePoint point = Controller.capturePoints[i];

            // If this capture point is controlled by the other team or if it is not completely controlled by either team...
            if (!point.team.compare(team) || point.control < 1) {

                // Consider it for minimum distance checking
                float dist = Vector2.Distance(transform.position, point.transform.position);
                if (dist < min) {  // If this is closer than the minimum previously found, then set the new minimum and capture point
                    min = dist;
                    cap = point;
                }
            }
        }

        return cap;  // Return the capture point
    }
}