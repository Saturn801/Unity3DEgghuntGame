// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateHandler : MonoBehaviour {

    #region Public Variables
    public AudioSource backgroundAudio, gameOverAudio, menuAudio;
    public Image endGameImage;
    public Sprite win, lose, tie;
    public PanelScript endGamePanel, mainMenuPanel;
    public GameObject tempCamera, mesh;
    public InputField inputField;
    public Text playerText, eggCountText, opponentText, endGameBigText, endGameSmallText, quitText;
    #endregion

    #region Private Variables
    DiamondSquare diamondSquareScript;
    int eggsRemaining, playerScore, opponentScore;
    string playerName;
    bool isHardMode;
    GameObject currMesh;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        menuAudio.Play();
    }
    #endregion

    #region Script Specific Methods
    public void startGame(bool hardMode)
    {
        menuAudio.Stop();
        isHardMode = hardMode;
        backgroundAudio.Play();
        currMesh = Instantiate(mesh);
        tempCamera.SetActive(false);
        diamondSquareScript = currMesh.GetComponent<DiamondSquare>();
        if(isHardMode)
            diamondSquareScript.setupScene(10);
        else
            diamondSquareScript.setupScene(5);
        eggsRemaining = 9;
        playerScore = 0;
        opponentScore = 0;
        playerName = inputField.text;
        playerText.text = playerName + ": " + playerScore;
        eggCountText.text = "Eggs Remaining: " + eggsRemaining;
        opponentText.text = "Opponent: " + opponentScore;
    }

    public void removeEgg()
    {
        if(eggsRemaining > 0)
            eggsRemaining--;
        eggCountText.text = "Eggs Remaining: " + eggsRemaining;
        if(eggsRemaining == 0)
        {
            turnOnEndGamePanel(2);
        }
    }

    public void updatePlayerScore(int value)
    {
        playerScore += value;
        playerText.text = playerName + ": " + playerScore;
    }

    public void updateOpponentScore(int value)
    {
        opponentScore += value;
        opponentText.text = "Opponent: " + opponentScore;
    }

    public bool isCloseToTable(GameObject objectToCheck)
    {
        GameObject table = GameObject.FindGameObjectWithTag("Table");
        float dist = Vector3.Distance(table.transform.position, objectToCheck.transform.position);
        if (dist <= 10.0f)
            return true;
        else
            return false;
    }

    public void turnOnEndGamePanel(int endGameCondition)
    {
        gameOverAudio.Play();
        quitText.text = "";
        endGamePanel.turnOn();
        if (endGameCondition == 0) // Fell over by self
        {
            endGameBigText.text = "You Lose!";
            endGameSmallText.text = "You fell over!";
            endGameImage.sprite = lose;
        }
        else if (endGameCondition == 1) // Hit opponent and fell over
        {
            endGameBigText.text = "It's a Tie!";
            endGameSmallText.text = "You hit your opponent and you both fell over!";
            endGameImage.sprite = tie;
        }
        else // All eggs are used
        {
            if (playerScore > opponentScore)
            {
                endGameBigText.text = "You Win!";
                endGameSmallText.text = "You scored more than your opponent!";
                endGameImage.sprite = win;
            }
            else if (playerScore == opponentScore)
            {
                endGameBigText.text = "It's a Tie!";
                endGameSmallText.text = "You scored the same as your opponent!";
                endGameImage.sprite = tie;
            }
            else
            {
                endGameBigText.text = "You Lose!";
                endGameSmallText.text = "You scored less than your opponent!";
                endGameImage.sprite = lose;
            }
        }                    
    }

    public void endGame()
    {
        menuAudio.Play();
        mainMenuPanel.turnOn();
        endGamePanel.turnOff();
        backgroundAudio.Stop();
        removeGameObjects();
        Destroy(currMesh);
        tempCamera.SetActive(true);
        playerText.text = "";
        eggCountText.text = "";
        opponentText.text = "";
        quitText.text = "";
    }

    void removeGameObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject table = GameObject.FindGameObjectWithTag("Table");
        GameObject opponent = GameObject.FindGameObjectWithTag("Opponent");
        GameObject[] skaters = GameObject.FindGameObjectsWithTag("Skater");
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        Destroy(player);
        Destroy(table);
        Destroy(opponent);
        foreach (GameObject skater in skaters)
            Destroy(skater);
        foreach (GameObject target in targets)
            Destroy(target);
    }

    public bool isCurrModeHard()
    {
        return isHardMode;
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void toggleQuitText()
    {
        if (quitText.text == "")
            quitText.text = "Quit Game?\nY / N";
        else
            quitText.text = "";
    }
    #endregion
}
