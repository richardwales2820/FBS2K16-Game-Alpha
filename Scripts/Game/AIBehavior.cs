using UnityEngine;
using System.Collections;

public class AIBehavior : MonoBehaviour {

    public Unit unit;  // This Unit

    public bool combat = false;  // Toggles whether the Unit is in combat

    private float radius = 75.0f;  // AI detection radius

	public LayerMask layerMask;

	// Use this for initialization
	void Start () {

        unit = GetComponent<Unit>();
	}
	
	// Update is called once per frame
	void Update () {

        if (!unit.player && unit.alive) {

            unit.Walk(false);

            // Check if an enemy is within the detection radius
            if (!unit.target) {

                // Get Colliders within radius
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

                float min = Mathf.Infinity;

                // Loop through colliders and find the closest target
                for (int i = 0; i < colliders.Length; i++) {

                    Unit enemy = colliders[i].GetComponent<Unit>();  // Get the Unit component of this Collider2D
                    
                    if (enemy) {
                        if (!unit.team.compare(enemy.team) && enemy.alive) {  // If the enemy Unit and this Unit are on different teams and the enemy is alive...

                            // Get the distance between this Unit and the enemy Unit
                            float dist = Vector2.Distance(enemy.transform.position, transform.position);

                            // If this is the closest Unit...
                            if (dist < min) {
                                min = dist;  // Set the new minimum
                                unit.target = enemy;  // Set the new target
                            }
                        }
                    }
                }
            }

            // If this AI now has a target, move it towards that target or shoot the target
            if (unit.target) {
                if (unit.target.alive) {

                    // If the distance is greater than the minDistance + 10 of this Unit, move towards the target
                    if (Vector2.Distance(transform.position, unit.target.transform.position) > unit.minDistance) {

                        // Move towards the target
                        unit.MoveTo(new Vector2(unit.target.transform.position.x, unit.target.transform.position.y));

                    // If the distance is within the minDistance, move away from the target
                    } else {

                        int xdir;
                        if (unit.target.transform.position.x > transform.position.x) xdir = -1;
                        else xdir = 1;

                        unit.MoveTo(new Vector2(unit.target.transform.position.x + unit.minDistance * xdir, unit.target.transform.position.y));
                    }

                    // If the target is within attack range
                    if (Vector2.Distance(transform.position, unit.target.transform.position) <= unit.attackDist && Mathf.Abs(unit.target.transform.position.y - transform.position.y) < unit.yvariant) {
                        if (unit.canAttack) {

                            // Face the target
                            if (unit.target.transform.position.x > transform.position.x) unit.Turn(Quaternion.Euler(0, 0, 0));
                            else unit.Turn(Quaternion.Euler(0, -180, 0));

                            unit.Attack();
                            unit.Cooldown();
                        }
                    }

                } else {

                    CapturePoint cap = unit.FindCapturePoint();

                    if (cap && unit.curNode) {
                        unit.capturePoint = cap;
                        unit.path = unit.AStar(unit.curNode, cap.node);
                    }

                    unit.target = null;
                }
                 
            } else {

                // If this Unit has a Path, move towards the next Node of the Path
                if (unit.path) {

                    //if (!unit.path.curNode.occupied && unit.path.curNode != unit.curNode) {
                        unit.MoveTo(unit.path.curNode.transform.position);
                    //}

                } else {  // Otherwise...

                    // If this Unit has a CapturePoint and the control percentage is less than 100%...
                    if (unit.capturePoint && unit.capturePoint.control < 1) {

                        unit.Walk(false);  // Stop walking

                    } else {  // Otherwise...
                        
                        // Find a new CapturePoint
                        CapturePoint cap = unit.FindCapturePoint();

                        if (cap && unit.curNode) {
                            unit.capturePoint = cap;
                            unit.path = unit.AStar(unit.curNode, cap.node);
                        }
                    }
                }
            }
        }
	}
}
