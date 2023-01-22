
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingTool : MonoBehaviour
{
	/// <summary>
	/// The bread and butter of this whole operation...
	/// It controls the linerenderers and mousepositions and what not
	/// for when the user is drawing. You can use the variables under the Brush
	/// Settings header willy nilly to have some fun. Just changed the size of the
	/// Brush Colours list in the inspector and add what colours you want. Dont
	/// go too wild tho...
	/// </summary>
	[Header("Setup Variables")]
	[SerializeField] private GameObject backgroundImage;
	[SerializeField] private Transform drawnLinesParent;
	[SerializeField] private GameObject linePrefab;
	
	[SerializeField] private float lineStepDistance; //How far the mouse moves before adding new point to line

	private GameObject currentLine = null; //Reference the line that is being drawn
	private bool drawing = false; //If we are currently drawing a line
	private bool waitingToDraw = false; //Used to see if the mouse is off the canvas but the button is held down
	private int layerOrder = 0; //Used so each line can get layered ontop of the last
	
	private List<GameObject> allDrawnLines = new List<GameObject>();
	private List<Vector3> currentLinePositions = new List<Vector3>(); //While we are drawing we update this list with all the positions of the line
	

	[Header("Brush Settings")]
	[Range(0.05f, 1f)]public float brushSize = 0.3f;
	public int currentMaterialIndex = 0;
	public List<Color> brushColours = new List<Color>(); //Works best with around 8 colours, can do more but the UI overflows :/
	private List<Material> brushMaterals = new List<Material>(); //Materials are created for this list on startup using the colours above

	private void Start() {
		//Init the colour brush settings
		for (int i = 0; i < brushColours.Count; i++) {
			Material newColour = new Material(Shader.Find("Sprites/Default"));
			newColour.color = brushColours[i];
			Debug.Log(newColour);
			brushMaterals.Add(newColour);
		}
	}


	private void Update() {
		if (InBounds()) {
			//Start a line
			if (Input.GetMouseButtonDown(0)) {
				CreateLine();
			}
			//If mouse is down while we are drawing
			if (drawing && Input.GetMouseButton(0)) {
				AddToLine();
				DrawLine();
			}
			//If mouse is lifted
			if (Input.GetMouseButtonUp(0)) {
				EndLine();
			}
			
			//If we have come back onto the canvas with our mouse down
			if (waitingToDraw && Input.GetMouseButton(0)) {
				waitingToDraw = false;
				CreateLine();
			}
		}
		
		//If we go out of bounds while drawing
		if (!InBounds() && drawing) {
			EndLine();
		}

		//If our mouse is down and we are out of bounds, say we are waiting to draw
		if (!InBounds() && Input.GetMouseButton(0)) {
			waitingToDraw = true;
		}
		if (!InBounds() && !Input.GetMouseButton(0)) {
			waitingToDraw = false;
		}
	}

	private bool InBounds() {
		//Get our mouse position
		var mousePosRaw = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var mousePosFinal = new Vector3(mousePosRaw.x, mousePosRaw.y, drawnLinesParent.position.z);
		//Get the width and height of the sprite (dont forget the scale of our image object)
		var width = backgroundImage.GetComponent<SpriteRenderer>().size.x * backgroundImage.transform.localScale.x * 0.95f;
		var height = backgroundImage.GetComponent<SpriteRenderer>().size.y * backgroundImage.transform.localScale.y * 0.95f;
		var backTrans = backgroundImage.transform.position; //Get reference to the position of the image
		//Check if mouse is inside its bounds
		if ((mousePosFinal.x > backTrans.x - width / 2 && mousePosFinal.x < backTrans.x + width / 2) && (mousePosFinal.y > backTrans.y - height / 2 && mousePosFinal.y < backTrans.y + height / 2)) {
			return true;	
		}
		return false;
	}

	//Runs when the mouse is clicked down and starts off the line
	private void CreateLine() {
		drawing = true;
		//Create the line at mouse position and add positions to line
		var mousePosRaw = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get mouse position to world
		var mousePosFinal = new Vector3(mousePosRaw.x, mousePosRaw.y, drawnLinesParent.position.z);
		currentLine = Instantiate(linePrefab, mousePosFinal, Quaternion.identity, drawnLinesParent); //Instantiate the line at this position
		allDrawnLines.Add(currentLine); //Store this new line in our array 
		//Add the mouse position to the start of the line, adds twice to solve an error but dw about it ;)
		currentLinePositions.Add(mousePosFinal);
		currentLinePositions.Add(mousePosFinal);
		
		//Set brush settings
		var currentRend = currentLine.GetComponent<LineRenderer>();
		currentRend.startWidth = brushSize;
		currentRend.material = brushMaterals[currentMaterialIndex];
		currentRend.sortingOrder = layerOrder;
		layerOrder++;


	}

	//Used every frame to update the line to match where the player has drawn
	private void DrawLine() {
		var renderer = currentLine.GetComponent<LineRenderer>();//Current line renderer we are using
		renderer.positionCount = currentLinePositions.Count; //Set its position count to be the same as how many positions we have entered
		//Loop through all of our positions and add them to the renderer
		for (int i = 0; i < currentLinePositions.Count; i++) { 
			renderer.SetPosition(i, currentLinePositions[i]);
		}
	}

	//Checks to see if we have moved an appropriate distance from the last added point and then adds to the list
	private void AddToLine() {
		//Get the mouse position but on the same Z pos as the parent object the lines get placed under
		var mousePosRaw = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get mouse position to world
		var mousePosFinal = new Vector3(mousePosRaw.x, mousePosRaw.y, drawnLinesParent.position.z);
		
		//Get the last added position and check the distance between the mouse and that position
		var currentIndex = currentLinePositions.Count;
		if (Vector3.Distance(currentLinePositions[currentIndex - 1], mousePosFinal) > lineStepDistance) {
			currentLinePositions.Add(mousePosFinal);
		}
	}
	
	//Resets it all so we can draw a new line
	private void EndLine() {
		drawing = false;
		currentLine = null;
		currentLinePositions.Clear();
	}
	
	//Remove the last line from the list
	public void Undo() {
		if (allDrawnLines.Count > 0) {
			var toDestroy = allDrawnLines[allDrawnLines.Count - 1];
			allDrawnLines.Remove(toDestroy);
			Destroy(toDestroy);
		}
	}

	public void UpdateBrushWidth(float newWidth) {
		brushSize = newWidth;
	}

	public void UpdateBrushColour(int newColour) {
		//Set the colour of the brush (used in conjunction with the DrawingToolUiController.cs script)
		if (newColour >= brushColours.Count) {
			currentMaterialIndex = 0;
			Debug.LogWarning("Trying to find colour index of " + newColour + " which is not possible...");
		} else {
			currentMaterialIndex = newColour;
		}
	}

	public void ClearCanvas() {
		EndLine();
		while (allDrawnLines.Count > 0) {
			var toDestroy = allDrawnLines[allDrawnLines.Count - 1];
			allDrawnLines.Remove(toDestroy);
			Destroy(toDestroy);
		}
	}
}
