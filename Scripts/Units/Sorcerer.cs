using UnityEngine;
using System.Collections;

public class Sorcerer : Unit {

    public GameObject fireballPrefab;
    public float fireballSpeed = 2.0f;
    public float fireballDistance = 75.0f;

    // Use this for initialization
    void Start () {

        // Sorcerer Stats //

        speed = 0.7f;  // speed
        defense = 0;  // defense
        attack = 6;  // attack
        maxhp = 15;  // hp
        cooldownTime = 0.55f;  // Attack cooldown

        // Special Abilities: ignores defense on hit of projectile
        ignoreArmor = true;

        // AI Attributes
        attackDist = 60.0f;  // Max distance AI will attack from
        minDistance = 19.5f;  // Minimum distance AI is comfortable standing from you
        maxDistance = 110.0f;  // Distance at which the AI will give up chasing

        yvariant = 8.0f;  // Set the y-coordinate difference to attack

		hp = maxhp;StartCoroutine ("CheckHealth");
	}

	// Same code as the archer and fighter's for checking health, regen, attack, etc.
	public IEnumerator CheckHealth()
	{
		float oldHp = hp;

		yield return new WaitForSeconds (5f);

		if (oldHp == hp)
			StartCoroutine ("RegenHealth");

		StartCoroutine ("CheckHealth");
	}

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

	// Place a projectile and play the right sound
    public override void Attack() {

        base.Attack();
        StartCoroutine(SpawnProjectile());  // Spawn the attack projectile
		audioc.PlayShootSound ();
    }

	// spawn the projectile in the correctly specified vector position offset from the player
    public override IEnumerator SpawnProjectile() {

        yield return new WaitForSeconds(0.175f);  // wait three frames

        if (alive) {
            int xdir = transform.rotation.eulerAngles.y == 0 ? 1 : -1;
            Projectile proj = (Instantiate(fireballPrefab, new Vector3(transform.position.x + 16 * xdir, transform.position.y + 10, transform.position.z), transform.rotation) as GameObject).GetComponent<Projectile>();
            proj.unit = this;
            proj.speed = fireballSpeed;
            proj.distance = fireballDistance;
            proj.origin = transform.position;
        }
    }

	// Levels up and modifies the stats of the sorcerer. The unique part of the sorcerer is its added fireball distance and speed upon reaching max level
    public override void LevelUp(int level) {

        base.LevelUp(level);

        if (level == 1) {
            attack += 0.25f;
            maxhp += 1;
            hp += 1;
        } else if (level == 2) {
            defense += 0.125f;
            attack += 0.25f;
            maxhp += 1;
            hp += 1;
        } else if (level == 3) {
            speed += 0.1f;
            defense += 0.5f;
            attack += 0.5f;
            maxhp += 2;
            hp += 2;
            fireballSpeed += 1.5f;
            fireballDistance += 25.0f;
        }
    }
}
