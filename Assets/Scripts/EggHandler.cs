// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggHandler : MonoBehaviour {

    #region Public Variables
    public GameObject eggSplat;
    #endregion

    #region Private Variables
    bool isGrabbed, isThrown;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Create () {
        isGrabbed = false;
        isThrown = false;        
    }
    #endregion

    #region Collision Methods
    void OnCollisionEnter(Collision col)
    {
        var gameController = GameObject.FindGameObjectWithTag("GameController");
        GameStateHandler gameStateHandlerScript = gameController.
            GetComponent<GameStateHandler>();

        AudioSource crackAudio = GameObject.FindGameObjectWithTag("AudioCrack").
            GetComponent<AudioSource>();
        crackAudio.Play();
        if (col.gameObject.tag == "Opponent")
        {
            Debug.Log("Opponent hit!");
            gameStateHandlerScript.updatePlayerScore(10);
            AudioSource owAudio = GameObject.FindGameObjectWithTag("AudioOw").
                GetComponent<AudioSource>();
            owAudio.Play();
        }
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player hit!");
            GameObject UICanvas = GameObject.FindGameObjectWithTag("UICanvas");
            float randomX = Random.Range(186.5f, 837.5f);
            float randomY = Random.Range(148.5f, 619.5f);
            Vector3 position = new Vector3(randomX, randomY, 0.0f);
            var splat = Instantiate(eggSplat, position, Quaternion.identity);
            splat.transform.SetParent(UICanvas.transform);
            gameStateHandlerScript.updateOpponentScore(10);
            AudioSource laughterAudio = GameObject.FindGameObjectWithTag("AudioLaugh").
                GetComponent<AudioSource>();
            laughterAudio.Play();
        }
        if (col.gameObject.tag == "Skater")
        {
            Debug.Log("Skater hit!");
            gameStateHandlerScript.updatePlayerScore(-5);
            AudioSource heyAudio = GameObject.FindGameObjectWithTag("AudioHey").
                GetComponent<AudioSource>();
            heyAudio.Play();
        }
        Destroy(this.gameObject);
        gameStateHandlerScript.removeEgg();
    }
    #endregion

    #region Script Specific Methods
    public void grabEgg()
    {
        isGrabbed = true;
    }

    public void throwEgg(float playerSpeed)
    {
        isThrown = true;
        Rigidbody rigidBody = this.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.AddForce(this.gameObject.transform.forward * (750.0f + 1000* playerSpeed));     
    }

    public bool isCurrentlyGrabbed()
    {
        return isGrabbed;
    }

    public bool isCurrentlyThrown()
    {
        return isThrown;
    }
    #endregion
}
