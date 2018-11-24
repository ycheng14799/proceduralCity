using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cityScript : MonoBehaviour {
    // Ground plane parameters 
    public float groundPlaneLength; 
    public float groundPlaneWidth;
    // Ground plane object 
    private GameObject groundPlane;

    // Ground plane initialization function 
    private void initializeGroundPlane(float length, float width)
    {
        // Initialize groundPlane 
        groundPlane = new GameObject();
        groundPlane.name = "GroundPlane";
        // Set parent of groundPlane to City object
        groundPlane.transform.SetParent(this.transform);

        // Initialize geometry of groundPlane 
        GameObject groundPlaneGeometry = GameObject.CreatePrimitive(PrimitiveType.Quad);
        groundPlaneGeometry.name = "GroundPlaneGeometry";
        // Correct orientation 
        groundPlaneGeometry.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
        // Set parent of groundPlaneGeometry to GroundPlane object
        groundPlaneGeometry.transform.SetParent(groundPlane.transform);

        // Set groundPlane to specified length and width 
        groundPlane.transform.localScale = new Vector3(width, 1.0f, length);
    }


	// Use this for initialization
	void Start () {
        // Initialize the ground plane
        initializeGroundPlane(groundPlaneLength, groundPlaneWidth);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
