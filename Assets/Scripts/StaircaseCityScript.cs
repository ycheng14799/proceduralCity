using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaircaseCityScript : MonoBehaviour {
    /* Public variables */ 
    // Building lot GameObjects
    // Positions for building placement
    public GameObject[] m_BuildingLots;
    
    // Width and length of building lots 
    public float m_buildingLotWidth;
    public float m_buildingLotLength;

    // Width and length percentages of individual building components
    public float m_minBuildingPartSidePercentage;
    public float m_maxBuildingPartSidePercentage;

    // Between building gap size 
    public float m_betweenBuildingGapSize; 

    // Building gap size 
    public float m_buildingGapMaxSize;

    // Initial building height 
    public float m_initHeight;
    // Building height increment 
    public float m_heightIncrement;
    // Sidewing height variation 
    public float m_sideWingHeightVariation;

    // Building movement speed 
    public float m_moveSpeed; 

    /* Private variables */
    // Height variable of buildings 
    // Should increment with each building 
    private float m_height;
    // Initial lot vertices 
    private Vector3[] initBuildingVertices;

    // QuadMesh method
    // Generates a Quad GameObject given vertices
    // @Params:
    // Vector3[] vertices: Vertices
    // string name: Name of new GameObject
    // string tag: Tag of new GameObject 
    // GameObject parent: Parent of new GameObject 
    // @Retval: 
    // GameObject of new Quad 
    private void quadMesh(GameObject newQuad, Vector3[] vertices, string name, string tag, GameObject parent)
    {
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

        // Set name, tag, and parent 
        if(name != null)
        {

            newQuad.name = name;
        }
        if(tag != null)
        {
            newQuad.tag = tag;
        }
        if(parent != null)
        {
            newQuad.transform.SetParent(parent.transform);
        }
    }

    // Extrude method 
    // Extrudes a building of a defined height given a Quad GameObject 
    // Function for extruding a single building from a building lot
    // @Params:
    // GameObject Quad: Empty building lot 
    // float height: Height of extrusion
    // string name: Name of new GameObject 
    // string tag: Tag of new GameObject
    // GameObject parent: Parent of current GameObject 
    // @Retval: 
    // GameObject of new extruded quad 
    private void extrudeQuad(GameObject quad, float height, string name, string tag, GameObject parent)
    {
        // Add mesh components to GameObject
        quad.AddComponent<MeshFilter>();
        quad.AddComponent<MeshRenderer>();
        quad.AddComponent<MeshCollider>();

        // Generate mesh for newQuad 
        Mesh newBuildingMesh = new Mesh();

        // Get vertex geometry of buildingLot 
        Vector3[] lotVertices = quad.GetComponent<MeshFilter>().mesh.vertices;

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
        quad.GetComponent<MeshFilter>().mesh = newBuildingMesh;
        // Set mesh for MeshCollider 
        quad.GetComponent<MeshCollider>().sharedMesh = newBuildingMesh;
        // Set default material 
        quad.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Diffuse"));

        // Set parent, name, and tag 
        if (parent != null)
        {
            quad.transform.SetParent(parent.transform);
        }
        if (name != null)
        {
            quad.name = name;
        }
        if (tag != null)
        {
            quad.gameObject.tag = tag;
        }
    }

    // Subdivide method 
    // Divides a given quad into two
    // @Params: 
    // GameObject quad: Initial quad GameObject 
    // bool lengthWise: Boolean specifying whether the division is length-wise or widthwise 
    // Let A and B denote the sides where the division will be invoked
    // float splitAPercentage: Percentage at which to split side A
    // float splitBPercentage: Percentage at which to split side B 
    // float gapPercentage: Percentage of side to set as a gap 
    // string AName: Name of part of divided quad
    // string BName: Name of part of divided quad 
    // string ATag: Tag of part of divided quad
    // string BTag: Tag of part of divided quad 
    // GameObject parent: Parent of divded quads
    // @RetVal: 
    // GameObject[]: Array of quad GameObjects 
    private GameObject[] subdivide(GameObject quad, bool lengthWise, float splitAPercentage, float splitBPercentage, float gapPercentage, string AName, string BName, string ATag, string BTag, GameObject parent)
    {
        // Get the initial quad's vertices 
        Vector3[] initQuadVertices = quad.GetComponent<MeshFilter>().mesh.vertices;
        // Initialize sides for division 
        Vector3 sideA, sideB;
        // Initialize new quad vertices 
        Vector3[] newAVertices = new Vector3[4];
        Vector3[] newBVertices = new Vector3[4]; 
        // Dependent on whether a length-wise or width-wise division was specified 
        if (lengthWise)
        {
            // Calculate division side vectors 
            sideA = initQuadVertices[2] - initQuadVertices[0];
            sideB = initQuadVertices[3] - initQuadVertices[1];
            // Set vertices for new quad A
            newAVertices[0] = initQuadVertices[0];
            newAVertices[1] = initQuadVertices[1];
            newAVertices[2] = initQuadVertices[0] + sideA * (splitAPercentage - gapPercentage * 0.5f);
            newAVertices[3] = initQuadVertices[1] + sideB * (splitBPercentage - gapPercentage * 0.5f);
            // Set vertices for new quad B
            newBVertices[0] = initQuadVertices[0] + sideA * (splitAPercentage + gapPercentage * 0.5f);
            newBVertices[1] = initQuadVertices[1] + sideB * (splitBPercentage + gapPercentage * 0.5f);
            newBVertices[2] = initQuadVertices[2];
            newBVertices[3] = initQuadVertices[3];
        } else
        {
            // Calculate division side vectors 
            sideA = initQuadVertices[1] - initQuadVertices[0];
            sideB = initQuadVertices[3] - initQuadVertices[2];
            // Set vertices for new quad A
            newAVertices[0] = initQuadVertices[0];
            newAVertices[1] = initQuadVertices[0] + sideA * (splitAPercentage - gapPercentage * 0.5f);
            newAVertices[2] = initQuadVertices[2];
            newAVertices[3] = initQuadVertices[2] + sideB * (splitBPercentage - gapPercentage * 0.5f);
            // Set vertices for new quad B
            newBVertices[0] = initQuadVertices[0] + sideA * (splitAPercentage + gapPercentage * 0.5f);
            newBVertices[1] = initQuadVertices[1];
            newBVertices[2] = initQuadVertices[2] + sideB * (splitBPercentage + gapPercentage * 0.5f);
            newBVertices[3] = initQuadVertices[3];
        }
        // Initialize new quad GameObjects 
        GameObject quadA = new GameObject();
        GameObject quadB = new GameObject();
        quadMesh(quadA, newAVertices, AName, ATag, parent);
        quadMesh(quadB, newBVertices, BName, BTag, parent);
        // Reset local positions 
        quadA.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        quadB.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        // Return quads 
        GameObject[] quads = new GameObject[] { quadA, quadB };
        return quads; 
    }

    // Function to generate a building 
    // @Params: 
    // GameObject quad: Building lot 
    private void build(GameObject quad)
    {
        // Divide building into Main and SideWing
        // Determine lengthwise or widthwise division 
        bool lengthwise = Random.value > 0.5f;
        // Randomise tags and names 
        string nameA, nameB, tagA, tagB; 
        if(Random.value > 0.5f)
        {
            nameA = "MainBuilding";
            nameB = "SideWing";
            tagA = "MainBuilding";
            tagB = "SideWing";
        } else
        {
            nameA = "SideWing";
            nameB = "MainBuilding";
            tagA = "SideWing";
            tagB = "MainBuilding";
        }
        // Determine split percentages 
        float splitAPercentage = Random.Range(m_minBuildingPartSidePercentage, m_maxBuildingPartSidePercentage);
        float splitBPercentage = Random.Range(m_minBuildingPartSidePercentage, m_maxBuildingPartSidePercentage);
        // Divide building into Main and SideWing
        GameObject[] quads = subdivide(quad, lengthwise, splitAPercentage, splitBPercentage, 0.0f, nameA, nameB, tagA, tagB, quad);
        
        // Split SideWing
        int sideWingIndex = 0;
        if (nameA != "SideWing")
        {
            sideWingIndex = 1;
        }
        // Determine split percentages 
        splitAPercentage = Random.Range(m_minBuildingPartSidePercentage, m_maxBuildingPartSidePercentage);
        splitBPercentage = Random.Range(m_minBuildingPartSidePercentage, m_maxBuildingPartSidePercentage);
        // Determine gap percentage 
        float gapPercentage = Random.Range(0.0f, m_buildingGapMaxSize);
        // Set lengthwise or widthwise 
        lengthwise = !lengthwise;
        // Divide sidewings 
        GameObject[] sideWings = subdivide(quads[sideWingIndex], lengthwise, splitAPercentage, splitBPercentage, gapPercentage, "SideWing", "SideWing", "SideWing", "SideWing", quad);
        // Store building parts in a singular array 
        GameObject[] buildingParts = new GameObject[]
        {
            quads[(sideWingIndex + 1) % 2],
            sideWings[0],
            sideWings[1]
        };
        // Destroy undivided side-wing 
        Destroy(quads[sideWingIndex]);

        // Extrude building lots 
        extrudeQuad(buildingParts[0], m_height, null, null, null);
        extrudeQuad(buildingParts[1], m_height + Random.Range(-m_sideWingHeightVariation, 0.0f), null, null, null);
        extrudeQuad(buildingParts[2], m_height + Random.Range(-m_sideWingHeightVariation, 0.0f), null, null, null);
    }

    // Use this for initialization
    void Start () {
        // Initialize height 
        m_height = m_initHeight;

        // Define initial building lot vertices 
        initBuildingVertices = new Vector3[]
        {
            new Vector3(-0.5f * m_buildingLotWidth, 0.0f, -0.5f * m_buildingLotLength),
            new Vector3(0.5f * m_buildingLotWidth, 0.0f, -0.5f * m_buildingLotLength),
            new Vector3(-0.5f * m_buildingLotWidth, 0.0f, 0.5f * m_buildingLotLength),
            new Vector3(0.5f * m_buildingLotWidth, 0.0f, 0.5f * m_buildingLotLength)
        };
        // Initialize building lots 
        for(int i = 0; i < m_BuildingLots.Length; i++)
        {
            // Define initial building lots
            quadMesh(m_BuildingLots[i], initBuildingVertices, null, null, null);
            // Build 
            build(m_BuildingLots[i]);
            // Transform building position 
            m_BuildingLots[i].transform.position = new Vector3(0.0f, 0.0f, i * (m_buildingLotLength + m_betweenBuildingGapSize));
            // Update height 
            m_height += m_heightIncrement;
            
        }

    }
	
	// Update is called once per frame
	void Update () {
        // Shift buildings backwards 
        for(int i = 0; i < m_BuildingLots.Length; i++)
        {
            // Change building position 
            m_BuildingLots[i].transform.position += Time.deltaTime * new Vector3(0.0f, 0.0f, m_moveSpeed);
            // Regenerate a new building when out of field of view 
            if (m_BuildingLots[i].transform.position.z <= -m_buildingLotLength)
            {
                // Destroy out of field of view building 
                DestroyImmediate(m_BuildingLots[i]);
                // Rebuild building
                m_BuildingLots[i] = new GameObject(); 
                quadMesh(m_BuildingLots[i], initBuildingVertices, "BuildingLot", "BuildingLot", this.gameObject);
                // Build 
                build(m_BuildingLots[i]);
                // Update height 
                m_height += m_heightIncrement;
                // Change position of building to end of queue 
                m_BuildingLots[i].transform.position = new Vector3(0.0f, 0.0f, (m_BuildingLots.Length - 1) * (m_buildingLotLength + m_betweenBuildingGapSize));
            }
        }
    }
}
