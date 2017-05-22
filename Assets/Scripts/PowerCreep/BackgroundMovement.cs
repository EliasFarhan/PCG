using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour {

    GameObject[] bgs;

	// Use this for initialization
	void Start () {
        bgs = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount;i++)
        {
            bgs[i] = transform.GetChild(i).gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
		foreach(var bg in bgs)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            if(cameraPos.x - bg.transform.position.x > 2.0f * Camera.main.aspect * Camera.main.orthographicSize)
            {
                bg.transform.position += 4.0f * Vector3.right * Camera.main.aspect * Camera.main.orthographicSize;
            }
            if (cameraPos.y - bg.transform.position.y > 2.0f * Camera.main.orthographicSize)
            {
                bg.transform.position += 4.0f * Vector3.up * Camera.main.orthographicSize;
            }
            if (cameraPos.x - bg.transform.position.x < -2.0f * Camera.main.aspect * Camera.main.orthographicSize)
            {
                bg.transform.position += 4.0f * Vector3.left * Camera.main.aspect * Camera.main.orthographicSize;
            }
            if (cameraPos.y - bg.transform.position.y < -2.0f * Camera.main.orthographicSize)
            {
                bg.transform.position += 4.0f * Vector3.down * Camera.main.orthographicSize;
            }
        }
	}
}
