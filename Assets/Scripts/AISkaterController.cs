// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AISkaterController : MonoBehaviour {

    #region Public Variables
    public GameObject targetObject;
    public bool isOpponent;
    public GameObject egg;
    #endregion

    #region Private Variables
    DiamondSquare floorScript;
    GameObject currentTarget, heldEgg;
    NavMeshAgent navMeshAgent;
    bool isHoldingEgg, isAggresive;
    GameStateHandler gameStateHandlerScript;
    int state;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();
        var gameController = GameObject.FindGameObjectWithTag("GameController");
        gameStateHandlerScript = gameController.GetComponent<GameStateHandler>();
        isAggresive = gameStateHandlerScript.isCurrModeHard();
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.SetDestination(currentTarget.transform.position);
        // Check if we've reached the destination
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    if (state == 0) // Reached random target  
                    {
                        Destroy(currentTarget);
                        if (isOpponent && !isHoldingEgg)
                        {
                            currentTarget = GameObject.FindGameObjectWithTag("Table");
                            state = 1;
                        }
                        else if (isOpponent && isHoldingEgg)
                        {
                            currentTarget = GameObject.FindGameObjectWithTag("Player");
                            state = 2;
                        }
                        else
                            createRandomTarget();
                    }
                    else if (state == 1) // Reached table
                    {
                        if (isAggresive)
                        {
                            currentTarget = GameObject.FindGameObjectWithTag("Player");
                            state = 2;
                        }
                        else
                        {
                            createRandomTarget();
                            state = 0;
                        }
                    }
                }
            }
        }
        if (isOpponent && !isHoldingEgg && gameStateHandlerScript.isCloseToTable(this.gameObject))
        {
            grabEgg();
        }
        if (isOpponent && isHoldingEgg && isCloseToPlayer() && state == 2)
        {
            throwEggAtPlayer();
            if (isAggresive)
            {
                currentTarget = GameObject.FindGameObjectWithTag("Table");
                state = 1;
            }
            else
            {
                createRandomTarget();
                state = 0;
            }
        }
    }
    #endregion

    #region Script Specific Methods
    public void setFloorScript(DiamondSquare script)
    {
        floorScript = script;
    }

    public void createRandomTarget()
    {
        Vector3 point = floorScript.createRandomPoint();
        var newTargetObject = Instantiate(targetObject, point, Quaternion.identity);
        currentTarget = newTargetObject;
    }

    void grabEgg()
    {
        var eggs = GameObject.FindGameObjectsWithTag("Egg");
        foreach (GameObject egg in eggs)
        {
            EggHandler eggStatus = egg.GetComponent<EggHandler>();
            if (!eggStatus.isCurrentlyGrabbed() && !eggStatus.isCurrentlyThrown())
            {
                Destroy(egg);
                var localOffset = new Vector3(-0.55f, 0.65f, 1);
                var worldOffset = transform.rotation * localOffset;
                var spawnPosition = transform.position + worldOffset;
                heldEgg = Instantiate(egg, spawnPosition, Quaternion.identity);
                heldEgg.GetComponent<EggHandler>().grabEgg();
                heldEgg.transform.parent = this.transform;
                isHoldingEgg = true;
                break;
            }
        }
    }

    void throwEggAtPlayer()
    {
        GameObject playerTarget = GameObject.FindGameObjectWithTag("PlayerTarget");
        Destroy(heldEgg);
        var localOffset = new Vector3(0.0f, 0.35f, 1);
        var worldOffset = this.transform.rotation * localOffset;
        var spawnPosition = this.transform.position + worldOffset;
        var thrownEgg = Instantiate(egg, spawnPosition, this.transform.rotation);
        thrownEgg.transform.LookAt(playerTarget.transform);
        thrownEgg.GetComponent<EggHandler>().throwEgg(1);
        isHoldingEgg = false;
        AudioSource throwAudio = GameObject.FindGameObjectWithTag("AudioThrow").
            GetComponent<AudioSource>();
        throwAudio.Play();
    }

    bool isCloseToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float dist = Vector3.Distance(player.transform.position, this.transform.position);
        if (dist <= 20.0f)
            return true;
        else
            return false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            createRandomTarget();
            state = 0;
        }   
    }
    #endregion
}
