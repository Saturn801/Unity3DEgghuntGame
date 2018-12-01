// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    #region Public Variables
    public float gravity;
    public GameObject egg;
    #endregion

    #region Private Variables
    float speed, currCameraXRotation, currCameraYRotation;
    bool isHoldingEgg, fallenOver, quitMenuOpen;
    GameObject heldEgg;
    GameStateHandler gameStateHandlerScript;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start () {
        currCameraXRotation = 0;
        currCameraYRotation = 0;
        quitMenuOpen = false;
        speed = 0;
        fallenOver = false;
        isHoldingEgg = false;
        var gameController = GameObject.FindGameObjectWithTag("GameController");
        gameStateHandlerScript = gameController.GetComponent<GameStateHandler>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!fallenOver)
        {
            if (Input.GetKey(KeyCode.UpArrow) && speed <= 0.3)
                speed += 0.001f;
            if (Input.GetKey(KeyCode.DownArrow) && speed > 0)
                speed -= 0.001f;
            if (Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(0, -0.5f - (3f * speed), 0, Space.Self);
            if (Input.GetKey(KeyCode.RightArrow))
                transform.Rotate(0, 0.5f + (3f * speed), 0, Space.Self);
            transform.position += transform.forward * speed;


            Camera camera = Camera.main;
            Vector3 currCameraRotation = camera.transform.eulerAngles;
            if (Input.GetKey(KeyCode.W) && currCameraXRotation > -90.0f)
            {
                currCameraRotation.x -= 1.0f;
                currCameraXRotation -= 1.0f;
            }               
            if (Input.GetKey(KeyCode.S) && currCameraXRotation < 90.0f)
            {
                currCameraRotation.x += 1.0f;
                currCameraXRotation += 1.0f;
            }              
            if (Input.GetKey(KeyCode.A) && currCameraYRotation > -120.0f)
            {
                currCameraRotation.y -= 1.0f;
                currCameraYRotation -= 1.0f;
            }               
            if (Input.GetKey(KeyCode.D) && currCameraYRotation < 120.0f)
            {
                currCameraRotation.y += 1.0f;
                currCameraYRotation += 1.0f;
            }               
            camera.transform.eulerAngles = currCameraRotation;


            if (Input.GetKey(KeyCode.Return) && !isHoldingEgg &&
                gameStateHandlerScript.isCloseToTable(this.gameObject))
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


            if (Input.GetKey(KeyCode.Space) && isHoldingEgg)
            {
                Destroy(heldEgg);
                var localOffset = new Vector3(0.0f, 0.35f, 1);
                var worldOffset = Camera.main.transform.rotation * localOffset;
                var spawnPosition = Camera.main.transform.position + worldOffset;
                var thrownEgg = Instantiate(egg, spawnPosition, Camera.main.transform.rotation);
                thrownEgg.GetComponent<EggHandler>().throwEgg(speed);
                isHoldingEgg = false;
                AudioSource throwAudio = GameObject.FindGameObjectWithTag("AudioThrow").
                    GetComponent<AudioSource>();
                throwAudio.Play();
            }


            if (Input.GetKey(KeyCode.Q) && !quitMenuOpen)
            {
                gameStateHandlerScript.toggleQuitText();
                quitMenuOpen = true;
            }
            if (Input.GetKey(KeyCode.Y) && quitMenuOpen)
            {
                gameStateHandlerScript.endGame();
            }
            if (Input.GetKey(KeyCode.N) && quitMenuOpen)
            {
                gameStateHandlerScript.toggleQuitText();
                quitMenuOpen = false;
            }

        }       
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.down * gravity * GetComponent<Rigidbody>().mass);
    }
    #endregion

    #region Collision Methods
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Opponent" || col.gameObject.tag == "Skater" ||
            col.gameObject.tag == "Table" || col.gameObject.tag == "Wall")
        {
            if (speed > 0.05f)
            {
                Debug.Log("Crash");
                fallOver(col.gameObject.tag);
            }               
        }
    }
    #endregion

    #region Script Specific Methods
    void fallOver(string collisionName)
    {
        AudioSource fallAudio = GameObject.FindGameObjectWithTag("AudioFall").GetComponent<AudioSource>();
        fallAudio.Play();
        speed = 0;
        fallenOver = true;
        GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward *
            -1000.0f * GetComponent<Rigidbody>().mass);
        if(collisionName == "Opponent")
            GameObject.FindGameObjectWithTag("GameController").
                GetComponent<GameStateHandler>().turnOnEndGamePanel(1);
        else
            GameObject.FindGameObjectWithTag("GameController").
                GetComponent<GameStateHandler>().turnOnEndGamePanel(0);
    }
    #endregion
}
