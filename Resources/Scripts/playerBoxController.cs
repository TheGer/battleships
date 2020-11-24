using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBoxController : MonoBehaviour
{
    bool highlighted = false;
    //public bool containsBoat = false; // Commented out to sync with teacher, it's added to gameManager
    Color currentColour;
    public int indexX, indexY;
    //GameObject camera;
    gameManager gm;

    private void Start()
    {
        currentColour = GetComponent<SpriteRenderer>().color;
        gm = Camera.main.GetComponent<gameManager>();

        //camera = GameObject.Find("Main Camera");
    }

    private void OnMouseEnter()
    {
        Color trans;
        trans = GetComponent<SpriteRenderer>().color;
        trans.a = 0.8f;
        GetComponent<SpriteRenderer>().color = trans;
    }

    private void OnMouseExit()
    {
        Color colour = GetComponent<SpriteRenderer>().color;
        colour.a = 1f;
        GetComponent<SpriteRenderer>().color = colour;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // This makes a horizontal boat
            Debug.Log("Horizontal boat " + indexX + " " + indexY);
            gm.activeBoat.place(indexX, indexY, false, gm.playerGrid);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // Makes a vertical boat
            Debug.Log("Vertical boat " + indexX + " " + indexY);
            gm.activeBoat.place(indexX, indexY, true, gm.playerGrid);
        }
    }

    void flipColour()
    {
        highlighted = !highlighted;

        if (highlighted)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = currentColour;
        }
    }

    /*void OnMouseDown()
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
            GetComponent<SpriteRenderer>().color = currentColour;
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
        }
    //}

    public void placeBoat()
    {
        containsBoat = true;
    }*/
}