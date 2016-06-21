using UnityEngine;
using System.Collections;

public class Fighter : Unit {

    public GameObject swordDamagePrefab;

	// Use this for initialization
	void Start () {

        // Fighter Stats //

        speed = 0.65f;  // speed
        defense = 3;  // defense
        attack = 7;  // attack
        maxhp = 17;  // hp
        cooldownTime = 0.55f;  // Attack cooldown

        // AI Attributes
        attackDist = 22.0f;  // Max distance AI will attack from
        minDistance = 21.5f;  // Minimum distance AI is comfortable standing from you
        maxDistance = 75.0f;  // Distance at which the AI will give up chasing

        yvariant = 12.0f;  // Set the y-coordinate difference to attack

        hp = maxhp;
		StartCoroutine ("CheckHealth");
	}

	// Following code follows same process as Archer for checking and regen of health, and attacking
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

    public override void Attack() {

        if (canAttack) {
            base.Attack();
            transform.Translate(5, 0, 0);
			audioc.PlaySwordSound ();
            StartCoroutine(SwordDamage());
        }
    }

	// knights have their own sword damage collider that is instantiated on attack to deal AoE damage to units in its range
    public IEnumerator SwordDamage() {

        yield return new WaitForSeconds(0.1333f);

        if (alive) {
            int xdir = transform.rotation.eulerAngles.y == 0 ? 1 : -1;
            DamageBox box = (Instantiate(swordDamagePrefab, new Vector3(transform.position.x + 15 * xdir, transform.position.y), Quaternion.identity) as GameObject).GetComponent<DamageBox>();
            box.unit = this;

            StartCoroutine(DestroyDamageBox(box));
        }
    }

	// destroys the damage collider after the attack is complete
    public IEnumerator DestroyDamageBox(DamageBox box) {

        yield return new WaitForSeconds(0.05f);
        if (box) Destroy(box.gameObject);
    }

	// Follows the same process as the archer unit except it has different stats specific to the knight
    public override void LevelUp(int level) {

        base.LevelUp(level);

        if (level == 1) {
            defense += 0.125f;
            maxhp += 1;
            hp += 1;
        } else if (level == 2) {
            defense += 0.125f;
            attack += 0.25f;
            maxhp += 2;
            hp += 2;
        } else if (level == 3) {
            speed += 0.15f;
            defense += 0.25f;
            attack += 1;
            maxhp += 6;
            hp += 6;
        }
    }
}
