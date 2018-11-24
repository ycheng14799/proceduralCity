using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cityScript : MonoBehaviour {
    // Ground plane parameters 
    public float groundPlaneLength; 
    public float groundPlaneWidth;
    // Ground plane object 
    private GameObject groundPlane;

    // Road parameters 
    // Road width 
    public float roadwidth;

    // Building block parameters
    // Minimum area 
    public float minBuildingBlockArea; 

    // Area calculation function 
    // Parameters: 
    // Array of four vertices 
    private float calculateArea(Vector3[] vertices)
    {
        // Calculate four sides of quad 
        Vector3 sideOne = vertices[1] - vertices[0];
        Vector3 sideTwo = vertices[2] - vertices[0];
        Vector3 sideThree = vertices[1] - vertices[3];
        Vector3 sideFour = vertices[2] - vertices[3]; 

        // Calculate area 
        return 0.5f * Vector3.Cross(sideOne, sideTwo).magnitude + 0.5f * Vector3.Cross(sideThree, sideFour).magnitude; 
    }


    // Plane subdivision function 
    // Parameters: 
    // GameObject originPlane: Plane for subdivision
    // bool Lengthwise division: Determines if length-wise or width-wise division
    private void subdivide(GameObject originPlane, bool lengthWiseDivision)
    {
        // Get vertices of original plane 
        Vector3[] originVert = originPlane.GetComponent<MeshFilter>().mesh.vertices;

        // Calculate original area 
        float areaOrigin = calculateArea(originVert); 

        // Catch base-case: 
        if(areaOrigin >= minBuildingBlockArea)
        {
            // Calculate position of division
            float divisionPositionOne = Random.Range(0.2f, 0.8f);
            float divisionPositionTwo = Random.Range(0.2f, 0.8f);
            // Get positions of each vertex 
            // Bottom Left 
            Vector3 BL = originVert[0];
            // Bottom Right 
            Vector3 BR = originVert[1];
            // Top Left 
            Vector3 TL = originVert[2];
            // Top Right 
            Vector3 TR = originVert[3];
            // Initialize new quads 
            GameObject quadA;
            GameObject quadB; 
            // Length-wise division 
            if (lengthWiseDivision)
            {
                quadA = quadMesh(new Vector3[]{BL, BL + (BR - BL) * divisionPositionOne - (BR - BL).normalized * roadwidth * 0.5f, TL, TL + (TR - TL) * divisionPositionTwo - (TR - TL).normalized * roadwidth * 0.5f});
                quadB = quadMesh(new Vector3[] { BL + (BR - BL) * divisionPositionOne + (BR - BL).normalized * roadwidth * 0.5f, BR, TL + (TR - TL) * divisionPositionTwo + (TR - TL).normalized * roadwidth * 0.5f, TR});
            }
            // Width-wise division
            else
            {
                // Calculate the two building blocks
                quadA = quadMesh(new Vector3[]{BL, BR, BL + (TL-BL)*divisionPositionOne - (TL - BL).normalized * roadwidth * 0.5f, BR + (TR - BR) * divisionPositionTwo - (TR - BR).normalized * roadwidth * 0.5f });
                quadB = quadMesh(new Vector3[]{BL + (TL - BL) * divisionPositionOne + (TL - BL).normalized * roadwidth * 0.5f, BR + (TR - BR) * divisionPositionTwo + (TR - BR).normalized * roadwidth * 0.5f, TL, TR});
            }
            // Set names 
            quadA.name = "BuildingBlock";
            quadB.name = "BuildingBlock";
            // Set parent object 
            quadA.transform.SetParent(this.transform);
            quadB.transform.SetParent(this.transform);
            // Destroy previous 
            DestroyImmediate(originPlane);
            // Recursive call 
            subdivide(quadA, !lengthWiseDivision);
            subdivide(quadB, !lengthWiseDivision);

        }
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

        // Subdivide buildingBlock 
        // Determine first division 
        bool lengthWiseDivision = Random.value > 0.5f;
        subdivide(buildingBlock, lengthWiseDivision);

        // Disable ground plane visibility 
        groundPlane.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
