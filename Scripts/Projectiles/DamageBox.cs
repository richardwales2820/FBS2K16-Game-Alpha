using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageBox : MonoBehaviour {

    public Unit unit;

    // Enemy units already hit by this collision box
    public List<Unit> units = new List<Unit>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// On trigger enter checks if an enemy enters the swords damage box
    void OnTriggerEnter2D(Collider2D coll) {

		// get the enemy unit hit from the sword 
        Unit enemy = coll.GetComponent<Unit>();

		// if the enemy is not null and is on the opposing team , apply the damage to them
        if (enemy) {
            if (!enemy.team.compare(unit.team)) {
                if (!units.Find(e => e.GetInstanceID() == enemy.GetInstanceID())) {
                    enemy.ApplyDamage(unit, unit.attack);
                    units.Add(enemy);
                }
            }
        }
    }
}
