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
    public float roadVariance;

    // Block parameters
    // Minimum dimensions  
    public float minBlockArea;
    public float minAllowableBlockWidth;
    public float minAllowableBlockLength;

    // Building parameters 
    // Minimum dimensions 
    public float minBuildingArea;
    public float minAllowableBuildingWidth;
    public float minAllowableBuildingLength;
    public float minAllowableBuildingHeight;
    public float maxAllowableBuildingHeight;
    public float minMainPercentage;
    public float maxMainPercentage;
    public float minMainSideLength;

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
    private void subdivide(GameObject originPlane, bool lengthWiseDivision, bool includeRoad, float minBuildingBlockArea, 
        float minAllowableWidth, float minAllowableLength, string name, string tag)
    {
        // Get vertices of original plane 
        Vector3[] originVert = originPlane.GetComponent<MeshFilter>().mesh.vertices;

        originPlane.name = name;
        originPlane.gameObject.tag = tag;

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
            // Get minimum length 
            float minWidth = Mathf.Min((BR - BL).magnitude, (TR - TL).magnitude);
            float minLength = Mathf.Min((TL - BL).magnitude, (TR - BR).magnitude);
            // Get minimum width 

            // Initialize new quads 
            GameObject quadA;
            GameObject quadB;

            // Initialize roadWidthWithNoise variable
            float roadWidthWithNoise;
            if (includeRoad)
            {
                roadWidthWithNoise = roadwidth + Random.Range(-roadVariance / 2.0f, roadVariance / 2.0f);
            } else
            {
                roadWidthWithNoise = 0.0f;
            }
            // Length-wise division 
            if (lengthWiseDivision && minWidth > minAllowableWidth)
            {
                quadA = quadMesh(new Vector3[]{BL, BL + (BR - BL) * divisionPositionOne - (BR - BL).normalized * roadWidthWithNoise * 0.5f, TL, TL + (TR - TL) * divisionPositionTwo - (TR - TL).normalized * roadWidthWithNoise * 0.5f});
                quadB = quadMesh(new Vector3[] { BL + (BR - BL) * divisionPositionOne + (BR - BL).normalized * roadWidthWithNoise * 0.5f, BR, TL + (TR - TL) * divisionPositionTwo + (TR - TL).normalized * roadWidthWithNoise * 0.5f, TR});
                // Set names 
                quadA.name = name;
                quadB.name = name;
                quadA.gameObject.tag = tag;
                quadB.gameObject.tag = tag;
                // Set parent object 
                quadA.transform.SetParent(this.transform);
                quadB.transform.SetParent(this.transform);
                // Destroy previous 
                DestroyImmediate(originPlane);
                // Recursive call 
                subdivide(quadA, !lengthWiseDivision, includeRoad, minBuildingBlockArea,
                    minAllowableWidth, minAllowableLength, name, tag);
                subdivide(quadB, !lengthWiseDivision, includeRoad, minBuildingBlockArea,
                    minAllowableWidth, minAllowableLength, name, tag);
            }
            // Width-wise division
            else if (!lengthWiseDivision && minLength > minAllowableLength)
            {
                // Calculate the two building blocks
                quadA = quadMesh(new Vector3[]{BL, BR, BL + (TL-BL)*divisionPositionOne - (TL - BL).normalized * roadWidthWithNoise * 0.5f, BR + (TR - BR) * divisionPositionTwo - (TR - BR).normalized * roadWidthWithNoise * 0.5f });
                quadB = quadMesh(new Vector3[]{BL + (TL - BL) * divisionPositionOne + (TL - BL).normalized * roadWidthWithNoise * 0.5f, BR + (TR - BR) * divisionPositionTwo + (TR - BR).normalized * roadWidthWithNoise * 0.5f, TL, TR});
                // Set names 
                quadA.name = name;
                quadB.name = name;
                quadA.gameObject.tag = tag;
                quadB.gameObject.tag = tag;
                // Set parent object 
                quadA.transform.SetParent(this.transform);
                quadB.transform.SetParent(this.transform);
                // Destroy previous 
                DestroyImmediate(originPlane);
                // Recursive call 
                subdivide(quadA, !lengthWiseDivision, includeRoad, minBuildingBlockArea,
                    minAllowableWidth, minAllowableLength, name, tag);
                subdivide(quadB, !lengthWiseDivision, includeRoad, minBuildingBlockArea,
                    minAllowableWidth, minAllowableLength, name, tag);
            }
            else
            {
                subdivide(originPlane, !lengthWiseDivision, includeRoad, minBuildingBlockArea,
                    minAllowableWidth, minAllowableLength, name, tag);
            }
            

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

    // Function for building the roadmap 
    private void generateRoadMap()
    {
        // Initialize the ground plane
        initializeGroundPlane(groundPlaneLength, groundPlaneWidth);

        // Get groundPlane vertices 
        Vector3[] groundPlaneVertices = groundPlane.GetComponent<MeshFilter>().mesh.vertices;

        // Generate first buildingBlock 
        GameObject buildingBlock = quadMesh(groundPlaneVertices);
        buildingBlock.name = "BuildingBlock";
        buildingBlock.gameObject.tag = "BuildingBlock";
        buildingBlock.transform.SetParent(this.transform);

        // Subdivide buildingBlock 
        // Determine first division 
        bool lengthWiseDivision = Random.value > 0.5f;
        // First recursive call to subdivide 
        // Generate road map 
        subdivide(buildingBlock, lengthWiseDivision, true, minBlockArea,
                    minAllowableBlockWidth, minAllowableBlockLength, "BuildingBlock", "BuildingBlock");
        // Disable ground plane visibility 
        groundPlane.SetActive(false);
    }

    // Function for generating building lots from roadmap 
    private void generateBuildingLots()
    {
        // Make further divisions to initially generated plots
        // For actual building parameters in a block
        foreach (GameObject aBuildingBlock in GameObject.FindGameObjectsWithTag("BuildingBlock"))
        {
            bool lengthWiseDivision = Random.value > 0.5f;
            subdivide(aBuildingBlock, lengthWiseDivision, false, minBuildingArea,
                    minAllowableBuildingWidth, minAllowableBuildingLength, "BuildingLot", "BuildingLot");
        }
    }

    // Function for extruding a single building from a building lot
    private void extrudeBuilding(float height, GameObject buildingLot)
    {
        // Instantiate a new GameObject newBuilding
        GameObject newBuilding = new GameObject();

        // Add mesh components to GameObject
        newBuilding.AddComponent<MeshFilter>();
        newBuilding.AddComponent<MeshRenderer>();
        newBuilding.AddComponent<MeshCollider>();

        // Generate mesh for newQuad 
        Mesh newBuildingMesh = new Mesh();

        // Get vertex geometry of buildingLot 
        Vector3[] lotVertices = buildingLot.GetComponent<MeshFilter>().mesh.vertices;

        // Height vector 
        Vector3 heightVec = new Vector3(0.0f, height, 0.0f);

        // Define new vertex geometry 
        Vector3[] buildingVertices =
        {
            // Bottom lower left corner
            lotVertices[0],
            lotVertices[0],
            lotVertices[0],
            // Bottom lower right color 
            lotVertices[1],
            lotVertices[1],
            lotVertices[1],
            // Bottom upper left corner 
            lotVertices[2],
            lotVertices[2],
            lotVertices[2],
            // Bottom upper right corner 
            lotVertices[3],
            lotVertices[3],
            lotVertices[3],
            // Top lower left corner
            lotVertices[0] + heightVec,
            lotVertices[0] + heightVec,
            lotVertices[0] + heightVec,
            // Top lower right corner 
            lotVertices[1] + heightVec,
            lotVertices[1] + heightVec,
            lotVertices[1] + heightVec,
            // Top upper left corner 
            lotVertices[2] + heightVec,
            lotVertices[2] + heightVec,
            lotVertices[2] + heightVec,
            // Top upper right corner
            lotVertices[3] + heightVec,
            lotVertices[3] + heightVec,
            lotVertices[3] + heightVec,

        };
        // Set newBuildingMesh vertices 
        newBuildingMesh.vertices = buildingVertices;

        // Define new triangles 
        int[] triangles =
        {
            // Face front 
            0, 12, 3, 
            12, 15, 3,
            // Face right 
            4, 16, 9, 
            16, 21, 9,
            // Face back 
            10, 22, 6, 
            22, 18, 6,
            // Face left 
            7, 19, 1,
            19, 13, 1,
            // Face bottom 
            8, 2, 11, 
            2, 5, 11, 
            // Face top 
            14, 20, 17,
            20, 23, 17
        };
        // Set newBuildingMesh triangles 
        newBuildingMesh.triangles = triangles;

        // Assign uv values for texturing 
        Vector2[] uv = new Vector2[24];
        // Front 
        uv[0] = new Vector2(0, 0);
        uv[12] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 0);
        uv[15] = new Vector2(1, 1);
        // Right
        uv[4] = new Vector2(0, 0);
        uv[16] = new Vector2(0, 1);
        uv[9] = new Vector2(1, 0);
        uv[21] = new Vector2(1, 1);
        // Back 
        uv[10] = new Vector2(0, 0);
        uv[22] = new Vector2(0, 1);
        uv[6] = new Vector2(1, 0);
        uv[18] = new Vector2(1, 1);
        // Left 
        uv[7] = new Vector2(0, 0);
        uv[19] = new Vector2(0, 1);
        uv[1] = new Vector2(1, 0);
        uv[13] = new Vector2(1, 1);
        // Bottom 
        uv[8] = new Vector2(0, 0);
        uv[2] = new Vector2(0, 1);
        uv[11] = new Vector2(1, 0);
        uv[5] = new Vector2(1, 1);
        // Top 
        uv[14] = new Vector2(0, 0);
        uv[20] = new Vector2(0, 1);
        uv[17] = new Vector2(1, 0);
        uv[23] = new Vector2(1, 1);

        newBuildingMesh.uv = uv; 

        // Calculate normals 
        Vector3[] normals = new Vector3[24];
        // Front normal 
        Vector3 frontNormal = Vector3.Cross((buildingVertices[12] - buildingVertices[0]), (buildingVertices[12] - buildingVertices[3]));
        frontNormal.Normalize();
        normals[0] = frontNormal;
        normals[12] = frontNormal;
        normals[3] = frontNormal;
        normals[15] = frontNormal;
        // Right normal 
        Vector3 rightNormal = Vector3.Cross((buildingVertices[16] - buildingVertices[4]), (buildingVertices[9] - buildingVertices[4]));
        rightNormal.Normalize();
        normals[4] = rightNormal;
        normals[16] = rightNormal;
        normals[9] = rightNormal;
        normals[21] = rightNormal;
        // Back normal 
        Vector3 backNormal = Vector3.Cross((buildingVertices[22] - buildingVertices[10]), (buildingVertices[6] - buildingVertices[10]));
        backNormal.Normalize();
        normals[10] = backNormal;
        normals[22] = backNormal;
        normals[6] = backNormal;
        normals[18] = backNormal;
        // Left normal 
        Vector3 leftNormal = Vector3.Cross((buildingVertices[19] - buildingVertices[7]), (buildingVertices[1] - buildingVertices[7]));
        leftNormal.Normalize();
        normals[7] = leftNormal;
        normals[19] = leftNormal;
        normals[1] = leftNormal;
        normals[13] = leftNormal;
        // Bottom normal 
        Vector3 bottomNormal = Vector3.Cross((buildingVertices[2] - buildingVertices[8]), (buildingVertices[11] - buildingVertices[8]));
        bottomNormal.Normalize();
        normals[2] = bottomNormal;
        normals[8] = bottomNormal;
        normals[11] = bottomNormal;
        normals[5] = bottomNormal;
        //Top normal
        Vector3 topNormal = Vector3.Cross((buildingVertices[20] - buildingVertices[14]), (buildingVertices[17] - buildingVertices[14]));
        topNormal.Normalize();
        normals[14] = topNormal;
        normals[20] = topNormal;
        normals[17] = topNormal;
        normals[23] = topNormal;
        // Set newBuildingMesh normals
        newBuildingMesh.normals = normals;

        // Set mesh in newBuidling GameObject
        // Set mesh for MeshFilter
        newBuilding.GetComponent<MeshFilter>().mesh = newBuildingMesh;
        // Set mesh for MeshCollider 
        newBuilding.GetComponent<MeshCollider>().sharedMesh = newBuildingMesh;
        // Set default material 
        newBuilding.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));

        // Set parent of building 
        newBuilding.transform.SetParent(buildingLot.transform);
        // Set name and tag 
        newBuilding.name = "Building";
        newBuilding.gameObject.tag = "Building";
    }

    // Function to extrude all buildings from building lots 
    private void extrudeAll(float minHeight, float maxHeight)
    {
        foreach (GameObject aBuildingLot in GameObject.FindGameObjectsWithTag("BuildingLot"))
        {
            float buildingHeight = Random.Range(minHeight, maxHeight);
            extrudeBuilding(buildingHeight, aBuildingLot);
        }
    }

    // Function to get the height of a building
    private float getBuildingHeight(GameObject building) {
        float height = 0.0f;
        // Get mesh vertices of the building GameObject 
        Vector3[] vertices = building.GetComponent<MeshFilter>().mesh.vertices;
        // Get vertex of top bottom left corner 
        Vector3 upperRightCorner = vertices[12];
        // Get Y component of upper right corner vertex 
        height = upperRightCorner.y;
        return height; 
    }

    // Function to split building into main and sidewings: Create U-Shape
    private void splitU(GameObject building)
    {
        // Get initial height of building 
        float initHeight = getBuildingHeight(building);
        // Get vertices of building
        Vector3[] initVertices = building.GetComponent<MeshFilter>().mesh.vertices;
        
        /*
        Constraints: 
        minMainPercentage, maxMainPercentage, minMainSideLength
        */

        // Get percentage split 
        float percentageSplit = Random.Range(minMainPercentage, maxMainPercentage);

        // Get original building ground plane 
        Vector3[] initGroundPlane = new Vector3[]
        {
            initVertices[0],
            initVertices[3],
            initVertices[6],
            initVertices[9]
        };

        // Subdivide building ground plane
        // Bottom Left 
        Vector3 BL = initGroundPlane[0];
        // Bottom Right 
        Vector3 BR = initGroundPlane[1];
        // Top Left 
        Vector3 TL = initGroundPlane[2];
        // Top Right 
        Vector3 TR = initGroundPlane[3];
        // Get the minimum "length" and "width"
        float minBuildLength = Mathf.Min((TL - BL).magnitude, (TR - BR).magnitude);
        float minBuildWidth = Mathf.Min((BL - BR).magnitude, (TL - TR).magnitude);

        // Check if length-wise division can be done
        bool lengthWiseValid = percentageSplit * minBuildLength > minMainSideLength;

        // Check if width-wise division can be done 
        bool widthWiseValid = percentageSplit * minBuildWidth > minMainSideLength;

        // If both length-wise and width-wise are valid 
        if(lengthWiseValid && widthWiseValid)
        {
            // Decide on length-wise or width-wise 
        }
        // Otherwise perform the valid operation 
        else if (lengthWiseValid)
        {
            // Length-wise division 
        } 
        else if (widthWiseValid)
        {
            // Width-wise division 
        }



    }

	// Use this for initialization
	void Start () {
        // Generate roadmap 
        generateRoadMap();
        // Subdivide blocks into building lots 
        generateBuildingLots();
        // Extrude buildings from building block
        extrudeAll(minAllowableBuildingHeight, maxAllowableBuildingHeight);
        // Splt buildings into U-Shapes 
        splitU(GameObject.FindGameObjectWithTag("Building"));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
