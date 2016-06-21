using UnityEngine;
using System.Collections;

// The letter class takes in a character, and depending on the character, returns the sprite associated to that passed char
public class Letter : MonoBehaviour {

    // Letters
    public Sprite A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z;

    // Numbers
    public Sprite num0, num1, num2, num3, num4, num5, num6, num7, num8, num9;

    // Symbols
    public Sprite sym_exc, sym_que, sym_per;

    // Sprite Renderer
    public SpriteRenderer spriteRenderer;

    // Word
    public Word word;

    // Word position offset
    public float offset = -Mathf.Infinity;

	// Use this for initialization
	void Start () {

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
	
        if (offset > -Mathf.Infinity) {
            transform.position = new Vector3(word.transform.position.x + offset, word.transform.position.y, word.transform.position.z);
        }

        GetComponent<SpriteRenderer>().sortingOrder = 1000;
	}

    public void GetSprite(char letter) {

        Sprite sprite = sym_que;

        if (letter == 'A') sprite = A;
        else if (letter == 'B') sprite = B;
        else if (letter == 'C') sprite = C;
        else if (letter == 'D') sprite = D;
        else if (letter == 'E') sprite = E;
        else if (letter == 'F') sprite = F;
        else if (letter == 'G') sprite = G;
        else if (letter == 'H') sprite = H;
        else if (letter == 'I') sprite = I;
        else if (letter == 'J') sprite = J;
        else if (letter == 'K') sprite = K;
        else if (letter == 'L') sprite = L;
        else if (letter == 'M') sprite = M;
        else if (letter == 'N') sprite = N;
        else if (letter == 'O') sprite = O;
        else if (letter == 'P') sprite = P;
        else if (letter == 'Q') sprite = Q;
        else if (letter == 'R') sprite = R;
        else if (letter == 'S') sprite = S;
        else if (letter == 'T') sprite = T;
        else if (letter == 'U') sprite = U;
        else if (letter == 'V') sprite = V;
        else if (letter == 'W') sprite = W;
        else if (letter == 'X') sprite = X;
        else if (letter == 'Y') sprite = Y;
        else if (letter == 'Z') sprite = Z;
        else if (letter == 'a') sprite = a;
        else if (letter == 'b') sprite = b;
        else if (letter == 'c') sprite = c;
        else if (letter == 'd') sprite = d;
        else if (letter == 'e') sprite = e;
        else if (letter == 'f') sprite = f;
        else if (letter == 'g') sprite = g;
        else if (letter == 'h') sprite = h;
        else if (letter == 'i') sprite = i;
        else if (letter == 'j') sprite = j;
        else if (letter == 'k') sprite = k;
        else if (letter == 'l') sprite = l;
        else if (letter == 'm') sprite = m;
        else if (letter == 'n') sprite = n;
        else if (letter == 'o') sprite = o;
        else if (letter == 'p') sprite = p;
        else if (letter == 'q') sprite = q;
        else if (letter == 'r') sprite = r;
        else if (letter == 's') sprite = s;
        else if (letter == 't') sprite = t;
        else if (letter == 'u') sprite = u;
        else if (letter == 'v') sprite = v;
        else if (letter == 'w') sprite = w;
        else if (letter == 'x') sprite = x;
        else if (letter == 'y') sprite = y;
        else if (letter == 'z') sprite = z;
        else if (letter == '0') sprite = num0;
        else if (letter == '1') sprite = num1;
        else if (letter == '2') sprite = num2;
        else if (letter == '3') sprite = num3;
        else if (letter == '4') sprite = num4;
        else if (letter == '5') sprite = num5;
        else if (letter == '6') sprite = num6;
        else if (letter == '7') sprite = num7;
        else if (letter == '8') sprite = num8;
        else if (letter == '9') sprite = num9;
        else if (letter == '.') sprite = sym_per;
        else if (letter == '!') sprite = sym_exc;
        else if (letter == '?') sprite = sym_que;
		else if (letter == ' ') sprite = null;

        spriteRenderer.sprite = sprite;
    }
}
