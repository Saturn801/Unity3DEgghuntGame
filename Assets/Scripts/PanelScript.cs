// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour {

    #region Private Variables;
    CanvasGroup canvas;
    #endregion

    #region MonoBehaviour Methods
    // Use this for initialization
    void Start()
    {
        canvas = (CanvasGroup)GetComponent(typeof(CanvasGroup));
    }
    #endregion

    #region Script Specific Methods
    public void turnOff()
    {
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void turnOn()
    {
        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
    #endregion
}
