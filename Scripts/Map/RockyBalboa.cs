using UnityEngine;
using System.Collections;

public class RockyBalboa : MonoBehaviour {

	// Unity checks for this property on collision with projectiles to destroy if the object is a Rock(y balboa)

	// Use this for initialization
	void Start () {

        GetComponent<Renderer>().sortingOrder = (int)-transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
