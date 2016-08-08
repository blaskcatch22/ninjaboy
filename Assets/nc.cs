using UnityEngine;
using System.Collections;

public class nc : MonoBehaviour {

    public float speed = 30f;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        rb2d.AddForce((Vector2.right * speed) * h);
	}
}
