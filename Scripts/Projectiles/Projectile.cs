using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	// projectile has an associated unit, its speed and distance values, as well as the origin of where it should spawn at
    public Unit unit;
    public float speed;
    public float distance;
    public Vector3 origin;

	// Use this for initialization
	void Start () {

        GetComponent<Renderer>().sortingOrder = (int)-transform.position.y + 5;
    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(speed, 0, 0);  // Move projectile

        // Check if it has exceeded its max distance
        if (Vector3.Distance(origin, transform.position) >= distance) {
            Destroy(gameObject);
        }
	}

	// if it collides, check what it hit
    void OnTriggerEnter2D(Collider2D collider) {

		// if the unit is an enemy, do damage and destroy the projectile

        Unit enemy = collider.gameObject.GetComponent<Unit>();
		RockyBalboa rockyBalboa = collider.gameObject.GetComponent<RockyBalboa> ();

        if (enemy) {  // If this is a Unit...
            if (enemy.team != unit.team) {
                if (enemy.alive) {
                    Destroy(gameObject);
                }

                enemy.ApplyDamage(unit, unit.attack);
            }
        }

		// if the collider is a rock, then only destroy the projectile
		if (rockyBalboa) {
			Destroy (gameObject);
		}
    }
}
