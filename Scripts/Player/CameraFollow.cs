using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	// This class is used to have the camera follow the player with added smoothing so that the camera is not always directly overhead

	public float smoothTime = 0.3f; // How quickly should the camera move to the player
	private Vector3 velocity = Vector3.zero; // The speed of the camera
    public static float cameraShake = 0; // The camera shake
	private Player player; // The player that the camera should follow
    private Node node; // The node the camera will follow on the menu

	void Start()
	{
        StartCoroutine(ChooseNode()); //At the beginning of the game, this coroutine begins to have the camera move randomly to nodes in the map
    }

    void Update () 
	{
        player = Controller.player;

        if (player) {
			
			// If the player is spawned, stop the moving function and move to the player 
            if (player.spawnedPlayer) {
                node = null;
		        Vector3 goalPos = player.transform.position;
		        goalPos.z = transform.position.z;
		        transform.position = Vector3.SmoothDamp (transform.position, goalPos, ref velocity, smoothTime);
            } else {

				// if the player is not spawned and there is a selected node, move to the node in a dampened fashion
                if (node) {
                    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(node.transform.position.x, node.transform.position.y, transform.position.z), ref velocity, 10.0f);
                }
            }
        }

		// If there is camera shake hapening, shake that thing. 
        if (cameraShake > 0) {
            transform.position = new Vector3(transform.position.x + Random.Range(-cameraShake, cameraShake), transform.position.y + Random.Range(-cameraShake, cameraShake), transform.position.z);
            cameraShake -= Time.deltaTime * 15.0f;
        } else {
            cameraShake = 0.0f;
        }
    }

	// Sets the camera shake value to the passed amount. This amount will be dependent on the damage being done to the player
    public static void ShakeCamera(float amount) {

        cameraShake = amount;
    }

	// Choose node Coroutine to pick a map node within a range that stays within the map bounds
    public IEnumerator ChooseNode() {

        float curx = 37;
        float cury = 5;

        if (node) {
            curx = node.x;
            cury = node.y;
        }

		// Gets a random node from the node grid and if it is valid, move to it and wait 5 seconds to check again
		// Else if it was not a node that is valid

        int x = (int)Random.Range(Mathf.Clamp(curx - 25.0f, 0, Controller.nodeGrid.GetLength(0)), Mathf.Clamp(curx + 25.0f, 0, Controller.nodeGrid.GetLength(0)));
        int y = (int)Random.Range(Mathf.Clamp(cury - 10.0f, 0, Controller.nodeGrid.GetLength(1)), Mathf.Clamp(cury + 10.0f, 0, Controller.nodeGrid.GetLength(1)));

        node = Controller.nodeGrid[x, y];

        if (!node) {
            yield return new WaitForSeconds(0.5f);
        } else {
            yield return new WaitForSeconds(5.0f);
        }

        StartCoroutine(ChooseNode());
    }
}
