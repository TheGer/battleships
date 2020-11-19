using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBoxController : MonoBehaviour
{
    bool highlighted = false;
    bool containsBoat = false;
    Color currentColor;
    public int indexX, indexY;

    private void Start()
    {
        currentColor = GetComponent<SpriteRenderer>().color;
    }

    private void OnMouseEnter()
    {
        Color trans;
        if (highlighted)
            trans = Color.red;
        else
            trans = currentColor;
        trans.a = 0.8f;
        GetComponent<SpriteRenderer>().color = trans;
    }

    private void OnMouseExit()
    {
        if (highlighted)
            GetComponent<SpriteRenderer>().color = Color.red;
        else
            GetComponent<SpriteRenderer>().color = currentColor;
    }

    void OnMouseDown()
    {
        // Replace with making a check to see if the boat will fit, 
        // and then placing the boat if possible
        highlighted = !highlighted;

        Debug.Log(highlighted + " " + indexX + indexY);

        if(highlighted)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }else
        {
            GetComponent<SpriteRenderer>().color = currentColor;
        }
    }
}