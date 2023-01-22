using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour
{
    [SerializeField]
    GameObject save, undo, clear, scroll, palette;
    private bool toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHideControls()
    {
        toggle = !toggle;
        save.SetActive(toggle);
        undo.SetActive(toggle);
        clear.SetActive(toggle);
        scroll.SetActive(toggle);
        palette.SetActive(toggle);
    }
}
