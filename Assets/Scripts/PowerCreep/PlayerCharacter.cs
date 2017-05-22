using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {
    [SerializeField]
    float speed = 15.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector3.up;
        }
        transform.position += direction * speed * Time.deltaTime;
        Camera.main.transform.position = transform.position - 10 * Vector3.forward;
	}
}
