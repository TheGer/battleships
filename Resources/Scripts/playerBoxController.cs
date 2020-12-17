using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBoxController : MonoBehaviour
{
    bool highlighted = false;
    Color currentColor;
    public int indexX, indexY;

    gameManager gm;

    private void Start()
    {
        currentColor = GetComponent<SpriteRenderer>().color;

        gm = Camera.main.GetComponent<gameManager>();

        
        
    }




    void OnMouseOver()
    {
        if (!gm.session.gameStarted)
        { 
            if (Input.GetMouseButtonDown(0))
            {
                //horizontal
                Debug.Log("Horizontal: "+indexX + " " + indexY);

                gm.currentlySelectedShip.place(indexX, indexY, false,gm.playerGrid);


              //  flipColor();
            }
            if (Input.GetMouseButtonDown(1))
            {
                //horizontal
                Debug.Log("Vertical: " + indexX + " " + indexY);

                gm.currentlySelectedShip.place(indexX, indexY, true,gm.playerGrid);
              //  flipColor();
            }
        }


    }

    public void flipColor()
    {
        // Destroy the gameObject after clicking on it
        highlighted = !highlighted;



        //  Debug.Log(highlighted);

       

        if(highlighted)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }else
        {
            GetComponent<SpriteRenderer>().color = currentColor;
        }
    }
}
