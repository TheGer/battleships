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


    IEnumerator handleShot()
    {
        yield return gm.dbScript.addShot(gm, new Shot(indexX, indexY));

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
                StartCoroutine(handleShot());

                //StartCoroutine(gm.waitForTurn());
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
