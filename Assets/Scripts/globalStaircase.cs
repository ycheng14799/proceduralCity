using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalStaircase : MonoBehaviour {

    public Transform[] blocks;

	// Use this for initialization
	void Start () {
        float i = 0.0f;
        float h = 0.5f; 
		foreach(Transform block in blocks)
        {
            block.transform.position = new Vector3(i, h, 0.0f);
            i++;
            h += 1.0f;
        }
	}
	
	// Update is called once per frame
	void Update () {
        
		foreach(Transform block in blocks)
        {
            block.transform.position += Time.deltaTime * new Vector3(-0.5f, -0.5f, 0.0f);
            if(block.transform.position.y <= -0.5f)
            {
                block.transform.position = new Vector3(4.0f, 4.5f, 0.0f);
            } 
        }
	}
}
