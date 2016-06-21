using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CapturePoint : MonoBehaviour {

    // Prefabs
    public GameObject flagPrefab;

    // UI
    public GameObject uiCaps;

    // Objects
    public Flag flag;  // Store the Flag for animation purposes
    public Team team;  // Controlling team

    // LayerMask
    public LayerMask layerMask;  // The layer the CapturePoint will search for in FixedUpdate
	public LayerMask nodeMask;

    // Attributes
    public float radius;  // The radius within which a Unit is considered within the CapturePoint
    public float control = 0;  // Current control percentage of controlling team

    // Booleans
    public bool captured = false;
	public bool midFlag = false;
    public bool scored = false;
    public bool neutral = false;

    // Node Information
    public Node node;
    public List<Node> nodes = new List<Node>();

	//Text
	Text display;

	// Capture bar
	GameObject captureBar;

	// Audio
	public AudioController audioc;

	// Use this for initialization
	void Start () {

        // Create a Flag if there isn't one created yet
        if (!flag) {
            flag = (Instantiate(flagPrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<Flag>();
            flag.capture(team);
        }

		audioc = GetComponent<AudioController> ();

		if (control == 0)
			midFlag = true;
    }
	
	// FixedUpdate is called once 1/30 of a second
	void FixedUpdate () {

		// if the status bar for the flag has not been instantiated yet, create it for the respective team
		if (!captureBar) {
			if (team.id == 0)
				captureBar = Instantiate (Controller.factory.redCapBarPrefab, transform.position, Quaternion.identity) as GameObject;
			else if (team.id == 1)
				captureBar = Instantiate (Controller.factory.blueCapBarPrefab, transform.position, Quaternion.identity) as GameObject;

			//If there is a capture bar created now, set its parent to the Unity GUI canvas in world space
			if (captureBar)
				captureBar.transform.parent = Controller.factory.worldCanvas.transform;
		}

		// If the capture bar is created now, fill up its status bar depending on the current control
		if (captureBar) {
			Image[] images = captureBar.GetComponentsInChildren<Image> ();
			foreach (Image i in images)
				i.fillAmount = control;

			// Also update its position based on the flags position
			captureBar.transform.position = new Vector3(transform.position.x, transform.position.y + 32, transform.position.z);
		}


		// Set variables for if the flag is taken or not
        scored = false;
        neutral = false;

        if (control == 1) scored = true;
        else if (control == 0) neutral = true;

		// Adjacent agent sensor detects what agents are within the flag's range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);  // Get Collider2D objects within the radius of the Capture Point

		// Iterate through the local agents nearby and change the control score based on their team if they are alive
        for (int i = 0; i < colliders.Length; i++) {

            Unit unit = colliders[i].GetComponent<Unit>();

            if (unit) {
                if (unit.alive) {  // Ensure dead Units don't get counted
						
                    // Add or subtract from the current control rate
                    if (unit.team == team) control += unit.controlRate / 30;
                    else control -= unit.controlRate / 30;
                }
            }

			// Keep the control value between these numbers to ensure they stay in the correct range
            control = Mathf.Clamp(control, -1, 1);
        }

        // If control goes below zero, the opposing team is beginning to capture this point
		if (control < 0) {

			// Give the capture point to the other team and BURN BABY BURN
            if (captured) 
				relinquish(team);

			// Sets the UI below that indicates what is being captured
            if (uiCaps) {
                if (team.id == 0) uiCaps.GetComponent<Animator>().SetBool("red_contest", false);
                if (team.id == 1) uiCaps.GetComponent<Animator>().SetBool("blue_contest", false);
            }

            team = Controller.teams[1 - team.id];  // change the controlling Team
            control = Mathf.Abs(control);

            if (uiCaps) {
                if (team.id == 0) uiCaps.GetComponent<Animator>().SetBool("red_contest", true);
                if (team.id == 1) uiCaps.GetComponent<Animator>().SetBool("blue_contest", true);
            }

			// If the point is being captured, destroy the capture bar for that flag with the previous team's color
			// And create a new one in the same place that has the correct color associated with it

			Destroy (captureBar);
			if (team.id == 0) {
			captureBar = Instantiate (Controller.factory.redCapBarPrefab, new Vector3(transform.position.x, transform.position.y + 32, transform.position.z), 
				Quaternion.identity) as GameObject;
			
				captureBar.transform.parent = Controller.factory.worldCanvas.transform;	

				Image[] images = captureBar.GetComponentsInChildren<Image> ();
				foreach (Image i in images)
					i.fillAmount = 0;
				
			} else if (team.id == 1) {
			captureBar = Instantiate (Controller.factory.blueCapBarPrefab, new Vector3(transform.position.x, transform.position.y + 32, transform.position.z), 
				Quaternion.identity) as GameObject;
			
				captureBar.transform.parent = Controller.factory.worldCanvas.transform;

				Image[] images = captureBar.GetComponentsInChildren<Image> ();
				foreach (Image i in images)
					i.fillAmount = 0;
			}
			// If the control has reached 1 and nobody has captured it yet, capture it for the controlling team
        } else if (control == 1) {

            if (!captured) capture(team);

        }

		// Conditionals below change the flag UI score at the bottom of the screen to correctly display the scores
        if (control < 1 && scored) {
            if (uiCaps) {
                if (team.id == 0) uiCaps.GetComponent<Animator>().SetBool("red_capture", false);
                else if (team.id == 1) uiCaps.GetComponent<Animator>().SetBool("blue_capture", false);
            }
        }

		// if the flag is neutral but it is being captured, dislay the flashing flag icon
        if (control > 0 && neutral) {
            if (uiCaps) {
                if (team.id == 0) uiCaps.GetComponent<Animator>().SetBool("red_contest", true);
                else if (team.id == 1) uiCaps.GetComponent<Animator>().SetBool("blue_contest", true);
            }
        }

		// Reflect the current changing status of the flag for the UI
        if (control == 1) {
            if (uiCaps) {
                if (team.id == 0) {
                    uiCaps.GetComponent<Animator>().SetBool("red_contest", true);
                    uiCaps.GetComponent<Animator>().SetBool("red_capture", true);
                } else if (team.id == 1) {
                    uiCaps.GetComponent<Animator>().SetBool("blue_contest", true);
                    uiCaps.GetComponent<Animator>().SetBool("blue_capture", true);
                }
            }
        }
	}

	// Capture the flag for the passed team to indicate it is scored and captured
	//Increment the score for that team and play the sound

    public void capture(Team team) {

        flag.capture(team);
        captured = true;
        scored = true;
        control = 1;

        team.caps++;

		audioc.PlayFlagCapSound ();

    }

	// Give the flag to the right team and decrease the score
	// Show the BURNINGGGGG SIMULATION
    public void relinquish(Team team) {

        flag.relinquish(team);
        captured = false;

        team.caps--;

		audioc.PlayBurnSound ();
    }

	// Mark the A* nodes surrounded by the adjacent agent sensor as being occupied by the flag's radius
	public void MarkAdjacentNodes()
	{
		Collider2D[] nodesInRage = Physics2D.OverlapCircleAll (transform.position, radius, nodeMask); 

		for (int i = 0; i < nodesInRage.Length; i++) {
			Node node = nodesInRage [i].GetComponent<Node> ();
			if (node) {
				node.flagNode = true;
                nodes.Add(node);
            }
		}
	}

	// Use A* to create the paths between points

    public Path AStar(Node start, Node end) {

        // Initialize G, H and F values for the starting Node
        start.G = 0;
        start.H = Vector3.Distance(start.transform.position, end.transform.position);
        start.F = start.H;

        Node cur = start;  // The current Node is initalized as the start Node

        Stack<Node> openStack = new Stack<Node>();
        Stack<Node> closedStack = new Stack<Node>();

        while (cur != end) {

            List<Node> nodes = cur.GetManhattanNodes();  // Get the Nodes adjacent to the current Node

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
        Path path = Controller.factory.CreatePath(nodePath);

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
}
