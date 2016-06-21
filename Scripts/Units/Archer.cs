using UnityEngine;
using System.Collections;

public class Archer : Unit {

    public GameObject arrowPrefab;
    public float arrowSpeed = 4.5f;
    public float arrowDistance = 200;

    // Use this for initialization
    void Start () {

        // Archer Stats //

        speed = 0.75f;  // speed
        defense = 2;  // defense
        attack = 5;  // attack
        maxhp = 13;  // hp
        cooldownTime = 0.75f;  // Attack cooldown

        // AI Attributes
        attackDist = 175.0f;  // Max distance AI will attack from
        minDistance = 60.0f;  // Minimum distance AI is comfortable standing from you
        maxDistance = 225.0f;  // Distance at which the AI will give up chasing

        yvariant = 8.0f;  // Set the y-coordinate difference to attack

        hp = maxhp;

		StartCoroutine ("CheckHealth");
	}

	// Coroutine checks the health of the unit and regens it if it has not decreased in the past 5 seconds
	public IEnumerator CheckHealth()
	{
		float oldHp = hp;

		yield return new WaitForSeconds (5f);

		if (oldHp == hp)
			StartCoroutine ("RegenHealth");

		StartCoroutine ("CheckHealth");
	}

	// This coroutine regens the health every 100 milliseconds if no damage has been done since the beginning of the coroutine
	public IEnumerator RegenHealth()
	{
		while (hp < maxhp) {
			hp++;
			float oldHp = hp;
			yield return new WaitForSeconds (0.1f);

			if (oldHp != hp)
				break;
		}
	}

	// Overrides the unit attack to spawn a projectile and play the correct sound
    public override void Attack() {

        base.Attack();
        StartCoroutine(SpawnProjectile());  // Spawn the attack projectile
		audioc.PlayArrowSound();
    }

	// Spawns a projectile after waiting enough time for the animation to reach the correct point of projectile spawning 
    public override IEnumerator SpawnProjectile() {

        yield return new WaitForSeconds(0.15f);  // wait three frames

		// If the unit is alive at this point of waiting, spawn the projectile and set its speed to the units projectile stats
        if (alive) {
            int xdir = transform.rotation.eulerAngles.y == 0 ? 1 : -1;
            Projectile proj = (Instantiate(arrowPrefab, new Vector3(transform.position.x + 8 * xdir, transform.position.y + 8, transform.position.z), transform.rotation) as GameObject).GetComponent<Projectile>();
            proj.unit = this;
            proj.speed = arrowSpeed;
            proj.distance = arrowDistance;
            proj.origin = transform.position;
        }
    }

	// Calls this method on level up to increase stats according to the correct level they have reached
    public override void LevelUp(int level) {

		// in addition to the base level up of the unit class, 
        base.LevelUp(level);

		// at each level, increment the stats accordingly, modified to add balance to the game at developer discretion
        if (level == 1) {
            attack += 0.25f;
            defense += 0.125f;
            arrowDistance += 30;
            attackDist += 30;
        } else if (level == 2) {
            defense += 0.125f;
            attack += 0.5f;
            maxhp += 1;
            hp += 1;
            arrowDistance += 40;
            attackDist += 40;
        } else if (level == 3) {
            speed += 0.1f;
            defense += 0.5f;
            attack += 2.0f;
            maxhp += 2;
            hp += 2;
            arrowDistance += 50;
            attackDist += 50;
        }
    }
}
