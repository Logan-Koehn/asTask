using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class Shapes : MonoBehaviour
{
    public invisInstructions inst;
    // Start is called before the first frame update
    void Start()
    {
        GameObject instructions = GameObject.FindWithTag("SceneInstructions");
        inst = instructions.GetComponent<invisInstructions>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void hide()
    {
        foreach (GameObject previewBar in inst.previewBars)
        {
            previewBar.SetActive(false);
        }
    }
}