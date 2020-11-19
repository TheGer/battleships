using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBoxController : MonoBehaviour
{
    bool highlighted = false;
    public bool containsBoat = false;
    Color currentColor;
    public int indexX, indexY;
    GameObject camera;

    private void Start()
    {
        camera = GameObject.Find("Main Camera");
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

        camera.GetComponent<gameManager>().clickedPlayerGrid(indexX, indexX);

        /*if(camera.GetComponent<gameManager>().activeBoat != null)
        {
            print("Lol boat");
            Boat theBoat = camera.GetComponent<gameManager>().activeBoat;

            if (theBoat.rotation) // ie Vertical
            {
                if (indexY + theBoat.length > 10)
                {
                    Debug.Log("Doesn't fit");
                }
                else
                {
                    theBoat.x = indexX;
                    theBoat.y = indexY;

                    for(int i = 0; i < theBoat.length; i++)
                    {
                        // Set all blocks in the range *this block* - **length* blocks above this block* to containsBoat=true
                    }

                    print("Boat goes from " + indexX + " " + indexY + " all the way to " + indexX + " " + (indexY + theBoat.length));
                }
            }
            else
            {
                if (indexX + theBoat.length > 10)
                {
                    Debug.Log("Doesn't fit");
                }
                else
                {
                    theBoat.x = indexX;
                    theBoat.y = indexY;

                    for (int i = 0; i < theBoat.length; i++)
                    {
                        // Set all blocks in the range *this block* - **length* blocks above this block* to containsBoat=true
                    }

                    print("Boat goes from " + indexX + " " + indexY + " all the way to " + (indexX + theBoat.length) + " " + indexY);
                }
            }
        }*/
    }

    public void placeBoat()
    {
        containsBoat = true;
    }
}