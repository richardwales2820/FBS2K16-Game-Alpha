using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {

    // Prefab
    public GameObject tilePrefab;

    // Rocks and Trees
    public RockyBalboa scenery;

    // Tile Bitmask
    public int tileMask;
    public int pathTile;

    // Booleans
    public bool topLeft;
    public bool bottomRight;

    // Grid coordinates
    public int x;
    public int y;

    // G H and F values
    public float G;
    public float H;
    public float F;

    // Open
    public int open = 0;
    public bool obstructed = false;
    public bool noscenery = false;
    public bool tiled = false;
    public bool flagNode = false;

    // Parent
    public Node parent;

    // Use this for initialization
    void Start() {

        GetComponent<Renderer>().enabled = false;  // hide nodes
    }

    // Update is called once per frame
    void Update() {

    }

	// Get the adjacent nodes from the current node
    public List<Node> GetAdjacentNodes() {

        int width = Controller.nodeGrid.GetLength(0) - 1;
        int height = Controller.nodeGrid.GetLength(1) - 1;

        List<Node> nodes = new List<Node>();

        for (int i = -1; i <= 1; i++) {  // Loop through Nodes left, right and center
            for (int j = -1; j <= 1; j++) {  // Loop through Nodes up, down and center
                if (i != 0 || j != 0) {  // DeMorgan's: if i and j are both not zero; don't include the specified Node as an adjacent Node
                    if ((x + i >= 0 && x + i < width) && (y + j >= 0 && y + j < height)) {  // Ensure this coordinate is not off the map
                        if (!obstructed) nodes.Add(Controller.nodeGrid[x + i, y + j]);  // Get the Node at this location
                    }
                }
            }
        }

        return nodes;
    }

	// Use manhattan distance to detect nodes in a relative adjacency
    public List<Node> GetManhattanNodes() {

        List<Node> nodes = new List<Node>();

        Node left = GetRelative(-1, 0);
        Node above = GetRelative(0, 1);
        Node right = GetRelative(1, 0);
        Node below = GetRelative(0, -1);

        if (left) {
            if (!left.obstructed) nodes.Add(left);
        }

        if (above) {
            if (!above.obstructed) nodes.Add(above);
        }

        if (right) {
            if (!right.obstructed) nodes.Add(right);
        }

        if (below) {
            if (!below.obstructed) nodes.Add(below);
        }

        return nodes;
    }

	// Actually creates the tile of this class
    public void Tile() {

        if (tilePrefab) {
            Instantiate(tilePrefab, transform.position, Quaternion.identity);
        }
    }

	// Gets the node relative to te passed coordinates
    public Node GetRelative(int x, int y) {

        if (this.x + x >= Controller.nodeGrid.GetLength(0) || this.x + x < 0) return null;
        else if (this.y + y >= Controller.nodeGrid.GetLength(1) || this.y + y < 0) return null;

        return Controller.nodeGrid[this.x + x, this.y + y];
    }
}
