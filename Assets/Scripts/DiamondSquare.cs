// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquare : MonoBehaviour {

    #region Public Variables
    public int numDivisions;
    public float size;
    public float maxHeight;
    public GameObject player, table, AISkater, opponentAISkater;
    #endregion

    #region Private Variables
    int numAISkaters;
    NavMeshBaker navBaker;
    Vector3[] vertices;    
    int vertexCount;
    #endregion

    #region Script Specific Methods
    public void setupScene(int numSkaters)
    {
        numAISkaters = numSkaters;
        CreateTerrain();
        navBaker = this.GetComponent<NavMeshBaker>();
        navBaker.buildNavMesh();
        spawnObjects();
    }

    void CreateTerrain()
    {
        vertexCount = (numDivisions + 1) * (numDivisions + 1);
        vertices = new Vector3[vertexCount];
        Vector2[] UVs = new Vector2[vertexCount];
        int[] triangles = new int[numDivisions * numDivisions * 6];
        float halfSize = size * 0.5f;
        float divisionSize = size / numDivisions;
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        int triangleIndex = 0;

        for(int i=0; i<=numDivisions; i++)
        {
            for(int j=0; j<=numDivisions; j++)
            {
                vertices[i * (numDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize,
                    0.0f, halfSize - i * divisionSize);
                UVs[i * (numDivisions + 1) + j] = new Vector2((float)i / numDivisions,
                    (float)j / numDivisions);
                if(i < numDivisions && j < numDivisions)
                {
                    int topLeft = i * (numDivisions + 1) + j;
                    int bottomLeft = (i+1) * (numDivisions + 1) + j;
                    triangles[triangleIndex] = topLeft;
                    triangles[triangleIndex + 1] = topLeft + 1;
                    triangles[triangleIndex + 2] = bottomLeft + 1;
                    triangles[triangleIndex + 3] = topLeft;
                    triangles[triangleIndex + 4] = bottomLeft + 1;
                    triangles[triangleIndex + 5] = bottomLeft;
                    triangleIndex += 6;
                }
            }
        }

        vertices[0].y = 0;
        vertices[numDivisions].y = 0;
        vertices[vertices.Length-1].y = 0;
        vertices[vertices.Length-1-numDivisions].y = 0;
        int iterations = (int)Mathf.Log(numDivisions, 2);
        int numSquares = 1;
        int squareSize = numDivisions;

        for(int i=0; i<iterations; i++)
        {
            int row = 0;
            for(int j=0; j<numSquares; j++)
            {
                int col = 0;
                for(int k=0; k<numSquares; k++)
                {
                    DiamondSquareAlgorithm(row, col, squareSize, maxHeight);
                    col += squareSize;
                }
                row += squareSize;
            }
            numSquares *= 2;
            squareSize /= 2;
            maxHeight *= 0.5f;  
        }

        mesh.vertices = vertices;
        mesh.uv = UVs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        this.gameObject.AddComponent<MeshCollider>();
    }

    void DiamondSquareAlgorithm(int row, int col, int size, float offset)
    {
        int halfSize = (int)(size * 0.5f);
        int topLeft = row * (numDivisions + 1) + col;
        int bottomLeft = (row+size) * (numDivisions + 1) + col;
        int middle = (int)(row + halfSize) * (numDivisions + 1) + (int)(col + halfSize);

        vertices[middle].y = (vertices[topLeft].y + vertices[topLeft + size].y 
            + vertices[bottomLeft].y + vertices[bottomLeft + size].y)*0.25f + 
            Random.Range(-offset, offset);

        vertices[topLeft + halfSize].y = (vertices[topLeft].y + vertices[middle].y 
            + vertices[topLeft + size].y)/3 + Random.Range(-offset, offset);

        vertices[middle - halfSize].y = (vertices[topLeft].y + vertices[middle].y 
            + vertices[bottomLeft].y)/3 + Random.Range(-offset, offset);

        vertices[middle + halfSize].y = (vertices[topLeft+size].y + vertices[middle].y
            + vertices[bottomLeft+size].y) / 3 + Random.Range(-offset, offset);

        vertices[bottomLeft + halfSize].y = (vertices[bottomLeft].y + vertices[middle].y
            + vertices[bottomLeft + size].y) / 3 + Random.Range(-offset, offset);
    }

    void spawnObjects()
    {
        Renderer r = GetComponent<Renderer>();
        RaycastHit hit;
        Vector3 point;

        // Create player
        point = createRandomPoint();
        var playerObject = Instantiate(player, point, Quaternion.identity);
        playerObject.transform.LookAt(Vector3.zero);

        // Create table
        if (Physics.Raycast(new Vector3(0, r.bounds.max.y + 5f, 0), -Vector3.up, out hit))
        {
            point = hit.point;
            point.y += 0.5f;
            Instantiate(table, point, Quaternion.identity);
        }

        // Create AI skaters
        for(int i = 0; i < numAISkaters; i++)
        {
            point = createRandomPoint();
            playerObject = Instantiate(AISkater, point, Quaternion.identity);
            playerObject.GetComponent<AISkaterController>().
                setFloorScript(GetComponent<DiamondSquare>());
            playerObject.GetComponent<AISkaterController>().createRandomTarget();
        }

        // Create Opponent
        point = createRandomPoint();
        playerObject = Instantiate(opponentAISkater, point, Quaternion.identity);
        playerObject.GetComponent<AISkaterController>().
            setFloorScript(GetComponent<DiamondSquare>());
        playerObject.GetComponent<AISkaterController>().createRandomTarget();
    }

    public Vector3 createRandomPoint()
    {
        Renderer r = GetComponent<Renderer>(); 
        RaycastHit hit;

        float randomX = Random.Range(r.bounds.min.x + 10, r.bounds.max.x - 10);
        float randomZ = Random.Range(r.bounds.min.z + 10, r.bounds.max.z - 10);
        if (Physics.Raycast(new Vector3(randomX, r.bounds.max.y + 5f, randomZ),
            -Vector3.up, out hit))
        {
            Vector3 point = hit.point;
            point.y += 0.5f;
            return point;
        }
        else
            return Vector3.zero;
    }
    #endregion
}