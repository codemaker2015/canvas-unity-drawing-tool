using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using UnityEditor;

public class RenderTextureToPng : MonoBehaviour
{
    /// <summary>
    /// This is ths script written that takes care of exporting the drawn image
    /// to a png file. It uses a render texture attached to an ortho camera that
    /// is set up in the scene to capture the size of the canvas.
    /// </summary>
    public RenderTexture rendTexture; //Render texture that our secondary camera captures

    private WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    public string fileName = "Render";
    
    public IEnumerator SnapshotImage() {
        yield return frameEnd; //Wait until the end of the frame
        
        RenderTexture currentActiveRT = RenderTexture.active; //Store our current render texture
        RenderTexture.active = rendTexture; //set new render texture
        
        Texture2D texture = new Texture2D(rendTexture.width, rendTexture.height, TextureFormat.RGB24, false); //Make a texture2d out of our render texture
        texture.ReadPixels(new Rect(0, 0, rendTexture.width, rendTexture.height), 0,0); //Read the pixels of our render texture

        byte[] bytes;
        bytes = texture.EncodeToPNG(); //Turn our texture2d to bytes
        
        RenderTexture.active = currentActiveRT; //Restore original render texture

        //Search for our SavedImages directory and save the image using the name in our fileName variable
        string path = "";
        if (Directory.Exists(Application.dataPath + "/SavedImages")) {
            path = Application.dataPath + "/SavedImages/" + fileName + ".png";
        } else {
            Directory.CreateDirectory(Application.dataPath + "/SavedImages");
            path = Application.dataPath + "/SavedImages/" + fileName + ".png";
        }
        
        System.IO.File.WriteAllBytes(path, bytes); //Put the info from the texture2d into the png
        Resources.Load(path); //Imports the image we made 
        Debug.Log("Saved to " + path); 
    }
    
    public void SaveImage() {
        StartCoroutine(SnapshotImage());
    }
}
