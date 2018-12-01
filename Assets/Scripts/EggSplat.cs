// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggSplat : MonoBehaviour {

    #region Private Variables
    CanvasGroup canvasGroup;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start () {
        canvasGroup = this.GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
        canvasGroup.alpha -= 0.02f;
        if (canvasGroup.alpha == 0)
            Destroy(this.gameObject);
	}
    #endregion

    #region Script Specific Methods
    public void displaySplat()
    {
        canvasGroup.alpha = 1.0f;
    }
    #endregion
}
