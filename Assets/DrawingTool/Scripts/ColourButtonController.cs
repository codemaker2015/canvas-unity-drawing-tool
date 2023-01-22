using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourButtonController : MonoBehaviour
{
    /// <summary>
    /// This script is added to the button prefabs. It is triggered
    /// when the button is pressed and the way the buttons are named
    /// it takes their index number that corresponds to the colour they
    /// are assigned and updates the drawing tool to ues that colour
    /// </summary>
    public void ButtonPressed() {
        string myName = gameObject.name;
        string[] splitName = myName.Split('_');
        int colourIndex = Int32.Parse(splitName[2]);
        GetComponentInParent<DrawingTool>().UpdateBrushColour(colourIndex);
    }
}
