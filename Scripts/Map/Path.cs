using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour {

	// This class identifies a path object and contains its node properties

    // Nodes
    public List<Node> nodes;
    public Node curNode;
    public int nodeIndex = 1;

    // Unit
    public Unit unit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
