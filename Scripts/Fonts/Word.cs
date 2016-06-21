using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Word contains the letters generated from the letter class
public class Word : MonoBehaviour {

    public List<Letter> letters = new List<Letter>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// destroys the letters associated with the word then destroys the word object
    public void DestroyWord() {

        foreach (Letter letter in letters) {
            Destroy(letter.gameObject);
        }

        Destroy(gameObject);
    }
}
