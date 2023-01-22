using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingToolUiController : MonoBehaviour
{
    /// <summary>
    /// A quick script that ties the UI and drawing tool together mostly.
    /// Some of the scripts that the UI uses exist soley inside the
    /// DrawingTools.cs file, these methods are for when data
    /// is needed to pass from the UI to the DrawingTools script rather
    /// than just button clicks being registered
    /// </summary>
    
    
    public Slider widthSlider; 

    public Transform colourButtonsParent; //A transform with a HorizontalLayoutGroup set up on it
    public GameObject colourButton; //A button with a LayoutElement set up on it
    
    private void Start() {
        var drawingTool = GetComponentInParent<DrawingTool>(); //Reference to our drawing tool so we only have to find it once
        widthSlider.value = drawingTool.brushSize; //Set our slider to be the width selected when we start the tool
        
        //Init the colour buttons
        var colourAmt = drawingTool.brushColours.Count; //How many colours we have set in the drawing tools script
        for (int i = 0; i < colourAmt; i++) {
            var newButton = Instantiate(colourButton, colourButtonsParent); //Instantiate a new button on the empty parent
            newButton.GetComponent<Image>().color = drawingTool.brushColours[i]; //Set the colour of the button to be the colour it represents
            newButton.name = "Button_Colour_" + i; //Give it the naming convention that allows its index to be split in the ColourButtonController.cs file
        }
    }

    public void UpdateWidth() {
        GetComponentInParent<DrawingTool>().UpdateBrushWidth(widthSlider.value); //Called to update brush width everytime the slider value is updated in the UI
    }


}
