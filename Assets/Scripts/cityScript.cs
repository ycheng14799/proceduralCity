using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cityScript : MonoBehaviour {
    // Ground plane parameters 
    public float groundPlaneLength; 
    public float groundPlaneWidth;
    // Ground plane object 
    private GameObject groundPlane;

    // Plane subdivision function 
    // Parameters: 
    // GameObject originPlane: Plane for subdivision
    private void subdivide(GameObject originPlane)
    {

    }

    // Quad mesh generation function 
    // Parameters:
    // Array of four vertices
    private GameObject quadMesh(Vector3[] vertices)
    {
        // Instantiate a new GameObject newQuad
        GameObject newQuad = new GameObject();
       
        // Add mesh components to GameObject
        newQuad.AddComponent<MeshFilter>();
        newQuad.AddComponent<MeshRenderer>();
        newQuad.AddComponent<MeshCollider>();

        // Generate mesh for newQuad 
        Mesh newQuadMesh = new Mesh();

        // Set vertices for mesh
        newQuadMesh.vertices = vertices;

        // Define triangles 
        int[] tri = new int[6];
        // Lower left triangle 
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;
        // Upper right triangle 
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        // Set triangles for mesh 
        newQuadMesh.triangles = tri;

        // Define normals 
        Vector3[] normals = new Vector3[4];
        normals[0] = new Vector3(0.0f, 1.0f, 0.0f);
        normals[1] = new Vector3(0.0f, 1.0f, 0.0f);
        normals[2] = new Vector3(0.0f, 1.0f, 0.0f);
        normals[3] = new Vector3(0.0f, 1.0f, 0.0f);
        // Set normals for mesh 
        newQuadMesh.normals = normals;  

        // Define UV values: For texturing 
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        // Set uvs for mesh 
        newQuadMesh.uv = uv;

        // Set colors for mesh 
        Color[] colors = new Color[4];
        colors[0] = Color.white;
        colors[1] = Color.white;
        colors[2] = Color.white;
        colors[3] = Color.white;
        newQuadMesh.colors = colors;

        // Set mesh in newQuad GameObject
        // Set mesh for MeshFilter
        newQuad.GetComponent<MeshFilter>().mesh = newQuadMesh;
        // Set mesh for MeshCollider 
        newQuad.GetComponent<MeshCollider>().sharedMesh = newQuadMesh;
        // Set default material 
        newQuad.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));
        return newQuad;
    }

    // Ground plane initialization function 
    // Parameters:
    // Float length: Plane length 
    // Float width: Plane width 
    private void initializeGroundPlane(float length, float width)
    {
        // Define geometry of groundPlane 
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-width / 2.0f, 0.0f, -length / 2.0f);
        vertices[1] = new Vector3(width / 2.0f, 0.0f, -length / 2.0f);
        vertices[2] = new Vector3(-width / 2.0f, 0.0f, length / 2.0f);
        vertices[3] = new Vector3(width / 2.0f, 0.0f, length / 2.0f); 
        // Initialize groundPlane
        groundPlane = quadMesh(vertices);
        groundPlane.name = "GroundPlane";
        // Set parent of groundPlane to City object
        groundPlane.transform.SetParent(this.transform);
        
    }


	// Use this for initialization
	void Start () {
        // Initialize the ground plane
        initializeGroundPlane(groundPlaneLength, groundPlaneWidth);

        // Get groundPlane vertices 
        Vector3[] groundPlaneVertices = groundPlane.GetComponent<MeshFilter>().mesh.vertices;

        // Generate first buildingBlock 
        GameObject buildingBlock = quadMesh(groundPlaneVertices);
        buildingBlock.name = "BuildingBlock";
        buildingBlock.transform.SetParent(this.transform);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
