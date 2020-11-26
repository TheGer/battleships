using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBoxController : MonoBehaviour
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
        if (Input.GetMouseButtonDown(0))
        {
            //horizontal
            Debug.Log("Horizontal: "+indexX + " " + indexY);

            //   gm.currentlySelectedShip.place(indexX, indexY, false,gm.playerGrid);

            if (gm.session.isMyTurn)
            { 
                flipColor();
                gm.session.isMyTurn = false;
                //if multiplayer, the logic of waiting for the other player will be somewhere here.

                //did I hit? 

                //if I did hit, I can shoot again

                //if I did not hit, I cannot shoot again

                //wait for a few seconds until I can shoot again
                StartCoroutine(gm.waitForTurn());
            }
        }
     


    }

   

    void flipColor()
    {
        // Destroy the gameObject after clicking on it
        highlighted = !highlighted;



        //  Debug.Log(highlighted);

       

        if(highlighted)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }else
        {
            GetComponent<SpriteRenderer>().color = currentColor;
        }
    }
}
