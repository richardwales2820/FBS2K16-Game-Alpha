using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*

    The Factory's main function is to act as a constructor for all other classes.
    It stores the prefabs for each class object as well as instantiation methods.
    Unity does not support constructors for classes which extend MonoBehavior.

*/

public class Factory : MonoBehaviour {

    /////////////
    // Prefabs //
    /////////////

    // Teams
    public GameObject teamPrefab;

    // Player
    public GameObject playerPrefab;

	// Levelup
	public GameObject levelupPrefab;

	// Canvas
	public GameObject worldCanvas;
	public GameObject screenCanvas;

    // Units
    public GameObject redFighterPrefab;
    public GameObject redArcherPrefab;
    public GameObject redSorcererPrefab;

    public GameObject blueFighterPrefab;
    public GameObject blueArcherPrefab;
    public GameObject blueSorcererPrefab;

    // UI
    public GameObject nameplatePrefab;
    public GameObject healthbarPrefab;
	public GameObject unitHealthbarPrefab;
	public GameObject redCapBarPrefab;
	public GameObject blueCapBarPrefab;
    public GameObject flagCapsPrefab;

    // Paths
    public GameObject pathPrefab;

    // Nodes
    public Node topLeft;
    public Node bottomRight;


    // Projectiles
    public GameObject redArrowPrefab;
    public GameObject blueArrowPrefab;

    public GameObject redFireballPrefab;
    public GameObject blueFireballPrefab;


    // Objectives
    public GameObject flagPrefab;
    public GameObject capturePointPrefab;

    // Fonts
    public GameObject wordPrefab, letterPrefab;

    // Map Tiles
    public GameObject grassTile;
    public GameObject pathTile;
    public GameObject pathBottomLeftTile;
    public GameObject pathBottomRightTile;
    public GameObject pathHorizontalBottomTile;
    public GameObject pathHorizontalTopTile;
    public GameObject pathInnerBottomLeftTile;
    public GameObject pathInnerBottomRightTile;
    public GameObject pathInnerTopLeftTile;
    public GameObject pathInnerTopRightTile;
    public GameObject pathTopLeftTile;
    public GameObject pathTopRightTile;
    public GameObject pathVerticalRightTile;
    public GameObject pathVerticalLeftTile;
    public GameObject pathSlash;
    public GameObject pathBackslash;

    // Rock Objects
    public GameObject rock1, rock3, rock4;
    public GameObject evergreen, birch, weed1, weed2;

	// Soundclips
	public AudioClip shootSound, hitSound,powerUpSound, flagCapSound, swordHitSound, burnSound, deathSound,
					 archerAttack, levelupSound, huhSound;


    ///////////////////////////
    // Instantiation Methods //
    ///////////////////////////

    // Player
    public Player CreatePlayer(Team team) {

        Player player = (Instantiate(playerPrefab) as GameObject).GetComponent<Player>();

        player.team = team;

        return player;   
    }

    // Teams
    public Team CreateTeam(int id, string name) {

        Team team = (Instantiate(teamPrefab) as GameObject).GetComponent<Team>();

        // Apply Team attributes
        team.id = id;
        team.teamName = name;

        return team;
    }
    

    // Units
    private Unit UnitInit(Unit unit, Team team) {

        // Team Information
        unit.team = team;

        // Animation Triggers
        unit.trigger_walk = unit.team.teamName + "_" + unit.className + "_walk";
        unit.trigger_attack1 = unit.team.teamName + "_" + unit.className + "_attack1";

		unit.levelWord = CreateWord ("1", new Vector3 (unit.transform.position.x - 12, unit.transform.position.y + 17, unit.transform.position.z)); 

        return unit;
    }

    // Knight
    public Fighter CreateFighter(Team team, SpawnPoint spawn) {

        GameObject prefab = team.id == 0 ? redFighterPrefab : blueFighterPrefab;
        Vector3 pos = spawn.transform.position;

        // Instantiation
        Fighter unit = (Instantiate(prefab, pos, Quaternion.identity) as GameObject).GetComponent<Fighter>();

        // Class-specific properties
        unit.prefab = prefab;
        unit.className = "fighter";

        unit.screenWord = CreateWord("Knight", new Vector3(unit.transform.position.x, unit.transform.position.y + 23, unit.transform.position.z));

        return (Fighter)UnitInit(unit, team);
    }

    // Archer
    public Archer CreateArcher(Team team, SpawnPoint spawn) {

        GameObject prefab = team.id == 0 ? redArcherPrefab : blueArcherPrefab;
        Vector3 pos = spawn.transform.position;

        // Instantiation
        Archer unit = (Instantiate(prefab, pos, Quaternion.identity) as GameObject).GetComponent<Archer>();

        // Prefabs
        unit.arrowPrefab = team.id == 0 ? redArrowPrefab : blueArrowPrefab;

        // Class-specific properties
        unit.prefab = prefab;
        unit.className = "archer";

        unit.screenWord = CreateWord("Archer", new Vector3(unit.transform.position.x, unit.transform.position.y + 23, unit.transform.position.z));

        return (Archer)UnitInit(unit, team);
    }

    // Sorcerer
    public Sorcerer CreateSorcerer(Team team, SpawnPoint spawn) {

        GameObject prefab = team.id == 0 ? redSorcererPrefab : blueSorcererPrefab;
        Vector3 pos = spawn.transform.position;

        // Instantiation
        Sorcerer unit = (Instantiate(prefab, pos, Quaternion.identity) as GameObject).GetComponent<Sorcerer>();

        // Prefabs
        unit.fireballPrefab = team.id == 0 ? redFireballPrefab : blueFireballPrefab;

        // Class-specific properties
        unit.prefab = prefab;
        unit.className = "sorcerer";

        unit.screenWord = CreateWord("Sorcerer", new Vector3(unit.transform.position.x, unit.transform.position.y + 23, unit.transform.position.z));

        return (Sorcerer)UnitInit(unit, team);
    }


    // CapturePoints
    public CapturePoint CreateCapturePoint(Team team, Node node) {

        Vector3 loc = node.transform.position;

        CapturePoint cap = (Instantiate(capturePointPrefab, loc, Quaternion.identity) as GameObject).GetComponent<CapturePoint>();

        // Give the CapturePoint a Flag
        cap.team = team ? team : Controller.teams[0];
        cap.node = node;
        cap.flag = (Instantiate(flagPrefab, loc, Quaternion.identity) as GameObject).GetComponent<Flag>();

        if (team) cap.capture(team);

        Controller.capturePoints.Add(cap);  // Add this to the list of CapturePoints

        return cap;
    }

    // Paths
    public Path CreatePath(List<Node> nodes, Unit unit) {

        Path path = (Instantiate(pathPrefab) as GameObject).GetComponent<Path>();

        // Path attributes
        path.nodes = nodes;
        path.unit = unit;
        path.curNode = nodes[0];

        return path;
    }

    // Path overload for map generation
    public Path CreatePath(List<Node> nodes) {

        Path path = (Instantiate(pathPrefab) as GameObject).GetComponent<Path>();

        // Path attributes
        path.nodes = nodes;

        return path;
    }

    // PCG Capture Point locations
	public List<CapturePoint> GenerateAllCapturePoints(List<Team> teams, Node[,] nodeGrid) {

		List<CapturePoint> capPoints = new List<CapturePoint> ();

		int grid1x = Random.Range(4, 13);
		int grid1y = Random.Range(3, 10);

		int grid2x = Random.Range (18, 27);
		int grid2y = Random.Range(3, 10);

		int grid3x = Random.Range (32, 41);
		int grid3y = Random.Range(3, 10);

		int grid4x = Random.Range (46, 55);
		int grid4y = Random.Range(3, 10);

		int grid5x = Random.Range (60, 69);
		int grid5y = Random.Range(3, 10);

		capPoints.Add (CreateCapturePoint (teams [1], nodeGrid [grid1x, grid1y]));
		capPoints.Add (CreateCapturePoint (teams [1], nodeGrid [grid2x, grid2y]));
		capPoints.Add (CreateCapturePoint (null, nodeGrid [grid3x, grid3y]));
		capPoints.Add (CreateCapturePoint (teams [0], nodeGrid [grid4x, grid4y]));
		capPoints.Add (CreateCapturePoint (teams [0], nodeGrid [grid5x, grid5y]));

		foreach (CapturePoint cp in capPoints) {
			cp.MarkAdjacentNodes ();
		}

		return capPoints;
	}

    // PCG map
    public void GenerateMap(Node[,] nodeGrid, int rocksPerChunk) {

        // Get the width and height of the Node Grid
        int width = nodeGrid.GetLength(0);
        int height = nodeGrid.GetLength(1);

        // Create rocks and trees
        for (int i = 7; i < width - 7; i += width / 10) {  // Generate rocks and trees 7 units from the left and right edges and increment by 10 units per chunk
            for (int j = 0; j < rocksPerChunk; j++) {  // Use "rocksPerChunk" to define how many rocks and trees can exist in a given chunk

                // Get random Node Grid coordinates
                int x = Random.Range(Mathf.Clamp(i, 0, width), Mathf.Clamp(i + width / 10, 0, width));
                int y = Random.Range(0, height);

                if (nodeGrid[x, y]) {  // If this Node exists...
                    if (!nodeGrid[x, y].noscenery) {  // If this Node has not been flagged for no scenery...
                        if (!nodeGrid[x, y].obstructed & !nodeGrid[x, y].flagNode) {  // If this Node is not already obstructed or been flagged as a flagNode...

                            // Randomly select a rock or tree
                            GameObject obj = rock1;
                            int type = Random.Range(0, 10);

                            if (type == 0) {
                                obj = rock1;
                            } else if (type == 1) {
                                obj = rock3;
                            } else if (type == 2) {
                                obj = rock4;

                            // Make trees more common than rocks
                            } else if (type >= 3 && type < 7) {
                                obj = evergreen;
                            } else if (type >= 7 && type < 10) {
                                obj = birch;
                            }

                            // Set this Node's scenery object and flag it as obstructed
                            nodeGrid[x, y].scenery = (Instantiate(obj, nodeGrid[x, y].transform.position, Quaternion.identity) as GameObject).GetComponent<RockyBalboa>();
                            nodeGrid[x, y].obstructed = true;

                            // Do not allow scenery objects to spawn on diagonals from other scenery objects
                            if (x + 1 < width && y + 1 < height) {
                                nodeGrid[x + 1, y + 1].noscenery = true;
                            }

                            if (x - 1 >= 0 && y + 1 < height) {
                                nodeGrid[x - 1, y + 1].noscenery = true;
                            }

                            if (x - 1 >= 0 && y - 1 >= 0) {
                                nodeGrid[x - 1, y - 1].noscenery = true;
                            }

                            if (x + 1 < width && y - 1 >= 0) {
                                nodeGrid[x + 1, y - 1].noscenery = true;
                            }
                        }
                    }
                }
            }
        }

        // Create Paths
        for (int i = 0; i < Controller.capturePoints.Count - 1; i++) {

            // Find the A* path between this capture point and the next one to the right
            CapturePoint cp = Controller.capturePoints[i];
            Path path = cp.AStar(cp.node, Controller.capturePoints[i + 1].node);

            // Thicken the path for tiling purposes
            foreach (Node node in path.nodes) {

                node.tilePrefab = pathTile;
                node.pathTile = 1;
                node.tiled = true;

                Node above = node.GetRelative(0, 1);
                Node left = node.GetRelative(-1, 0);

                // The Nodes above and to the left of this Node should also be pathed
                if (above) {
                    above.tilePrefab = pathTile;
                    above.pathTile = 1;
                    above.tiled = true;

                    if (above.scenery) Destroy(above.scenery.gameObject);
                }

                if (left) {
                    left.tilePrefab = pathTile;
                    left.pathTile = 1;
                    left.tiled = true;

                    if (left.scenery) Destroy(left.scenery.gameObject);
                }
            }
        }

        // Set remaining tiles
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (nodeGrid[i, j]) {
                    if (!nodeGrid[i, j].tiled) {  // If this Node hasn't already been tiled

                        // If this is within the range of a capture point, set it to a path tile
                        if (nodeGrid[i, j].flagNode) {
                            nodeGrid[i, j].tilePrefab = pathTile;
                            nodeGrid[i, j].pathTile = 1;
                        } else {  // ... otherwise, set this to a grass tile
                            nodeGrid[i, j].tilePrefab = grassTile;
                            nodeGrid[i, j].pathTile = 0;
                        }

                        nodeGrid[i, j].tiled = true;  // This Node is now tiled
                    }
                }
            }
        }

        // Use bitmasks to blend the path tiles and tile edges
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {

                Node node = nodeGrid[i, j];

                if (node) {

                    if (node.pathTile == 1) {  // If this is a path tile...

                        // Get relative Nodes; set each direction to 1 if it is a path tile, 0 if it is not
                        int NW = !node.GetRelative(-1, 1) ? 0 : node.GetRelative(-1, 1).pathTile;
                        int N = !node.GetRelative(0, 1) ? 0 : node.GetRelative(0, 1).pathTile;
                        int NE = !node.GetRelative(1, 1) ? 0 : node.GetRelative(1, 1).pathTile;
                        int W = !node.GetRelative(-1, 0) ? 0 : node.GetRelative(-1, 0).pathTile;
                        int E = !node.GetRelative(1, 0) ? 0 : node.GetRelative(1, 0).pathTile;
                        int SW = !node.GetRelative(-1, -1) ? 0 : node.GetRelative(-1, -1).pathTile;
                        int S = !node.GetRelative(0, -1) ? 0 : node.GetRelative(0, -1).pathTile;
                        int SE = !node.GetRelative(1, -1) ? 0 : node.GetRelative(1, -1).pathTile;

                        // Use 8-bit bitmask to give this Node a bit value
                        int mask = 0;

                        // Calculate mask
                        mask = NE + E * 32 + SE * 2 + S * 64 + SW * 4 + W * 128 + NW * 8 + N * 16;

                        // Obtain the correct tile depending on the bitmask
                        node.tilePrefab = GetTile(node, mask);

                    } else {
                        node.tilePrefab = grassTile;  // If this isn't a path tile, it's a grass tile
                    }

                    node.Tile();  // Instantiate the tile object on top of this Node
                }   
            }
        }

        // Create weeds
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (nodeGrid[i, j]) {
                    if (nodeGrid[i, j].pathTile == 0) {
                        if (Random.Range(0.0f, 1.0f) < 0.12f) {  // Allow a 12% chance for weeds to be created

                            int weed = Random.Range(0, 2);  // Choose one of the two random weeds

                            // Instantiate the weeds
                            if (weed == 0) (Instantiate(weed1, nodeGrid[i, j].transform.position, Quaternion.identity) as GameObject).GetComponent<BlazeIt>();
                            else if (weed == 1) (Instantiate(weed2, nodeGrid[i, j].transform.position, Quaternion.identity) as GameObject).GetComponent<BlazeIt>();
                        }
                    }
                }
            }
        }

        // Create grass and scenery beyond the map
		for (int i = (int)topLeft.transform.position.x - 16 * 15 - 1; i < (int)bottomRight.transform.position.x + 16 * 15; i += 16) {
			for (int j = (int)topLeft.transform.position.y + 16 * 14; j > (int)bottomRight.transform.position.y - 15 * 16; j -= 16) {

                // Create grass tile
				(Instantiate(grassTile, new Vector3(i, j), Quaternion.identity) as GameObject).GetComponent<Tile>();

                // Compare the position of this location with the topLeft and bottomRight extreme Nodes of the map
				if (i < (int)topLeft.transform.position.x || i > (int)bottomRight.transform.position.x ||
				    j > (int)topLeft.transform.position.y || j < (int)bottomRight.transform.position.y) {

                    // Randomly choose a rock or tree
					GameObject obj = rock1;
					int type = Random.Range(0, 10);

					if (type == 0) {
						obj = rock1;
					} else if (type == 1) {
						obj = rock3;
					} else if (type == 2) {
						obj = rock4;
					} else if (type >= 3 && type < 7) {
						obj = evergreen;
					} else if (type >= 7 && type < 10) {
						obj = birch;
					}
					
                    // Create the scenery object and destroy its collision and rigidbody components for enhanced performance
					GameObject newObj = Instantiate (obj, new Vector3 (i, j), Quaternion.identity) as GameObject;
					Destroy (newObj.GetComponent<CircleCollider2D> ());
					Destroy (newObj.GetComponent<Rigidbody2D> ());
				}
			}
		}
    }

    // Returns the appropriate tile given the bitmask
    public GameObject GetTile(Node node, int bitmask) {

        /* These bitmasks were calculated using an 8-bit mask with the following setup:
        
             8 ___16___1
              |__|__|__|
           128|__|__|__|32
              |__|__|__|
             4    64    2

        */
        if (bitmask == 0) {
            return grassTile;
        } else if (bitmask == 255) {
            return pathTile;
        } else if (bitmask == 196 || bitmask == 198 || bitmask == 204 || bitmask == 206) {
            return pathBottomLeftTile;
        } else if (bitmask == 98 || bitmask == 99 || bitmask == 102 || bitmask == 103) {
            return pathBottomRightTile;
        } else if (bitmask == 185 || bitmask == 189 || bitmask == 187 || bitmask == 191) {
            return pathHorizontalBottomTile;
        } else if (bitmask == 230 || bitmask == 238 || bitmask == 231 || bitmask == 239) {
            return pathHorizontalTopTile;
        } else if (bitmask == 254) {
            return pathInnerBottomLeftTile;
        } else if (bitmask == 247) {
            return pathInnerBottomRightTile;
        } else if (bitmask == 253) {
            return pathInnerTopLeftTile;
        } else if (bitmask == 251) {
            return pathInnerTopRightTile;
        } else if (bitmask == 152 || bitmask == 153 || bitmask == 156 || bitmask == 157) {
            return pathTopLeftTile;
        } else if (bitmask == 49 || bitmask == 51 || bitmask == 57 || bitmask == 59) {
            return pathTopRightTile;
        } else if (bitmask == 115 || bitmask == 123 || bitmask == 119) {
            return pathVerticalRightTile;
        } else if (bitmask == 220 || bitmask == 221 || bitmask == 222) {
            return pathVerticalLeftTile;
        } else if (bitmask == 245) {
            return pathSlash;
        } else if (bitmask == 250) {
            return pathBackslash;
        } else {
            return grassTile;
        }
    }

    // Creation of Text using custom font
    public Word CreateWord(string chars, Vector3 position) {

        // Create new Word object
        Word word = (Instantiate(wordPrefab, position, Quaternion.identity) as GameObject).GetComponent<Word>();

        // Center the word depending on the amount of characters in the string
        float offset = -chars.Length * 2 + 2.5f;

        foreach (char c in chars) {

            // Instantiate word objects for each letter
            Letter letter = (Instantiate(letterPrefab, new Vector3(position.x + offset, position.y, position.z), Quaternion.identity) as GameObject).GetComponent<Letter>();

            // Set letter attributes
            letter.GetSprite(c);
            letter.offset = offset;
            letter.word = word;
            word.letters.Add(letter);

            offset += 4;  // character spacing
        }

        // Return resulting Word
        return word;
    }
}
