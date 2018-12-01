// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour {

    #region Private Variables
    NavMeshSurface navMesh;
    #endregion

    #region Script Specific Methods
    public void buildNavMesh()
    {
        navMesh = this.GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();
    }
    #endregion
}