// Samuel James Bryan - 14701935

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

    #region Script Specific Methods
    public void unHighlightButton()
    {
        var button = this.GetComponent<Button>();
        button.OnDeselect(null);
    }
    #endregion
}