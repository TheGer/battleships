using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

class BattleshipGrid
{
    public List<Block> blocks;
    public GameObject parent;

    public BattleshipGrid()
    {
        blocks = new List<Block>();
    }

    public void makeClickable()
    {
        foreach (Block b in blocks)
        {
            b.toptile.AddComponent<playerBoxController>();
            b.setClickCoordinates();
        }
    }
}

class Block
{
    public GameObject toptile, bottomtile;
    public int indexX, indexY;

    public void flipTile() { }

    public void setClickCoordinates()
    {
        if (toptile.GetComponent<playerBoxController>() != null)
        {
            toptile.GetComponent<playerBoxController>().indexX = indexX;
            toptile.GetComponent<playerBoxController>().indexY = indexY;
        }
    }
}

public class Boat
{
    // public GameObject boat;
    public int x, y, length;
    public bool rotation; // true = vertical, false = horizontal
    bool placed;

    public Boat(int lengths)
    {
        length = lengths;
        rotation = false;
        placed = false;
    }

    public void place(int x,int y, bool orientation)
    {

    }
}

public class gameManager : MonoBehaviour
{
    BattleshipGrid playerGrid, enemyGrid;
    GameObject rowLabel, rowL, sq, buttonPrefab; // rowLabel is the TextPrefab, rowL is the instance of each new square, sq is the square prefab, buttonPrefab is a mystery
    string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
    //public bool boatActive;
    Boat[] allBoats;

    public Boat activeBoat = null;

    Button createWorldButton(string label, GameObject parent,Vector3 pos)
    {
        GameObject buttonCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/myButton"), pos, Quaternion.identity);
        buttonCanvas.transform.SetParent(parent.transform);
        buttonCanvas.GetComponentInChildren<Text>().text = label;

        buttonCanvas.name = label;

        buttonCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        buttonCanvas.GetComponent<Canvas>().sortingOrder = 1; // Ensure it's unobstructed

        return buttonCanvas.GetComponentInChildren<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sq = Resources.Load<GameObject>("Prefabs/Square");
        rowLabel = Resources.Load<GameObject>("Prefabs/TextPrefab");
        buttonPrefab = Resources.Load<GameObject>("Prefabs/myButton");

        allBoats = new Boat[5];

        Boat carrier = new Boat(5);
        Boat battleship = new Boat(4);
        Boat cruiser = new Boat(3);
        Boat submarine = new Boat(3);
        Boat destroyer = new Boat(2);

        allBoats[0] = carrier;
        allBoats[0] = battleship;
        allBoats[0] = cruiser;
        allBoats[0] = submarine;
        allBoats[0] = destroyer;

        GameObject anchor = new GameObject("playergrid");
        GameObject anchor2 = new GameObject("enemygrid");
        GameObject anchor3 = new GameObject("shipSelectionGrid");

        playerGrid = GenerateGrid(anchor);
        playerGrid.parent.transform.position = new Vector3(-10f, -10f);
        playerGrid.parent.transform.localScale = new Vector3(1.5f, 1.5f);
        playerGrid.makeClickable(); // This adds the playerBoxController script to all clickable boxes

        enemyGrid = GenerateGrid(anchor2);
        enemyGrid.parent.transform.position = new Vector3(10f, 10f);
        enemyGrid.parent.transform.localScale = new Vector3(1.5f, 1.5f);

        // Ship selection
        Button carrierBtn = createWorldButton("Carrier", anchor3, new Vector3(0f, 0f));
        Button battleshipBtn = createWorldButton("Battleship", anchor3, new Vector3(0f, -3f));
        Button submarineBtn = createWorldButton("Submarine", anchor3, new Vector3(0f, -6f));
        Button cruiserBtn = createWorldButton("Cruiser", anchor3, new Vector3(0f, -9f));
        Button destroyerBtn = createWorldButton("Destroyer", anchor3, new Vector3(0f, -12f));

        carrierBtn.onClick.AddListener(() => { Debug.Log("Carrier button pressed"); });
        carrierBtn.onClick.AddListener(() => { activeBoat = carrier; });
        battleshipBtn.onClick.AddListener(() => { Debug.Log("Battleship button pressed"); });
        submarineBtn.onClick.AddListener(() => { Debug.Log("Submarine button pressed"); });
        cruiserBtn.onClick.AddListener(() => { Debug.Log("Cruiser button pressed"); });
        destroyerBtn.onClick.AddListener(() => { Debug.Log("Destroyer button pressed"); });

        anchor3.transform.position = new Vector3(10f, -4f);

        // MakeBoats();
    }

    BattleshipGrid GenerateGrid(GameObject parentObject)
    {
        int yCounter = 0;
        int lettercounter = 0;
        BattleshipGrid grid = new BattleshipGrid();

        for (float ycoord = -4.5f; ycoord <= 4.5f; ycoord++)
        {
            //for each row
            yCounter++;
            rowL = Instantiate(rowLabel, new Vector3(-5.5f, ycoord), Quaternion.identity);
            rowL.GetComponentInChildren<Text>().text = yCounter.ToString();
            rowL.transform.SetParent(parentObject.transform);

            int xCounter = 0;
            for (float xcoord = -4.5f; xcoord <= 4.5f; xcoord++)
            {
                xCounter++;
                //first row
                if (ycoord == 4.5f)
                {
                    rowL = Instantiate(rowLabel, new Vector3(xcoord, 5.5f), Quaternion.identity);
                    rowL.GetComponentInChildren<Text>().text = letters[lettercounter];
                    rowL.transform.SetParent(parentObject.transform);
                    lettercounter++;
                }

                Block b = new Block();
                b.bottomtile = Instantiate(sq, new Vector3(xcoord, ycoord), Quaternion.identity);
                b.toptile = Instantiate(sq, new Vector3(xcoord, ycoord), Quaternion.identity);
                b.toptile.transform.localScale = new Vector3(0.8f, 0.8f);
                b.toptile.name = "TopTile" + letters[xCounter - 1] + yCounter;
                b.toptile.AddComponent<BoxCollider2D>();
                b.toptile.GetComponent<BoxCollider2D>().isTrigger = true;
                b.toptile.GetComponent<BoxCollider2D>().size = new Vector3(1.25f, 1.25f, 1);
                b.bottomtile.GetComponent<SpriteRenderer>().color = Color.black;
                b.toptile.transform.SetParent(parentObject.transform);
                b.bottomtile.transform.SetParent(parentObject.transform);
                b.bottomtile.name = "BottomTile";

                b.indexX = xCounter;
                b.indexY = yCounter;

                grid.blocks.Add(b);
            }
        }
        grid.parent = parentObject;
        return grid;
    }

    IEnumerator MakeBoats()
    {
        for (int i = 0; i < allBoats.Length; i++)
        {
            yield return PlaceBoat(allBoats[i].length);
        }

        foreach(Block b in playerGrid.blocks)
        {
            Destroy(b.toptile.GetComponent<playerBoxController>());
        }

        enemyGrid.makeClickable();
    }

    public Boat boat;
    IEnumerator PlaceBoat(int length)
    {
        //while (boatActive)
            yield return null;
        
        yield return null;
    }

    public void clickedPlayerGrid(int x, int y)
    { 
        if (activeBoat != null)
        {
            print("lol boat");
            if (activeBoat.rotation) // ie vertical
            {
                print("Vertical boat");
                if (y + activeBoat.length > 10)
                {
                    Debug.Log("Doesn't fit");
                }
                else
                {
                    print("Fits");
                    activeBoat.x = x;
                    activeBoat.y = y;

                    print("Boat goes from " + x + " " + x + " all the way to " + x + " " + (y + activeBoat.length));

                    int index = (((y - 1) * 10) + x) - 1;
                    print("Changing block " + index + "to contain boat");

                    playerGrid.blocks[index].toptile.GetComponent<playerBoxController>().placeBoat();
                }
            }
            else
            {
                print("Horizontal boat");
                if (x + activeBoat.length > 10)
                {
                    Debug.Log("Doesn't fit");
                }
                else
                {
                    print("Fits");
                    activeBoat.x = x;
                    activeBoat.y = y;

                    print("Boat goes from " + x + " " + y + " all the way to " + (x + activeBoat.length) + " " + y);

                    int index = (((y - 1) * 10) + x) - 1;
                    print("Changing block " + index + " to contain boat");

                    playerGrid.blocks[index].toptile.GetComponent<playerBoxController>().placeBoat();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}