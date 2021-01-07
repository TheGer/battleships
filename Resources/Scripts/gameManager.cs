using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;


// Next step today:

// A list of ships displayed next to the player grid
// Carrier - 5 blocks
// Battleship - 4 blocks
// Cruiser - 3 blocks
// Submarine - 3 blocks (different colour?)
// Destroyer - 2 blocks

// When clicked, select that ship (for example 5 blocks)
// if you left click on the player grid, 5 blocks reserved HORIZONTALLY
// if you right click on the player grid, 5 blocks reserved VERTICALLY
// if ship doesn't fit, write a debug.log stating that the ship doesn't fit and leave it selected


//At the beginning of the game we are missing some information 
//1. Player name
//2. Is another player connected? 

//FOR THURSDAY
//Implement two player code with number of shots in firebase.
//P1 will shoot in red, P2 will shoot in blue on the same enemy grid


//Try to make sure that once there are two players, no other players can join.

//1. Figure out turns
//2. Figure out shots and hits 
//3. Figure out end of game


[System.Serializable]
public class Ship
{
    public int numberofblocks;
    public Color backColor;

    public int _x;
    public int _y;

    
    public bool vertical;
    public bool placed;

    public string shipname;

    public List<Shot> hits;


    public Ship(string _shipname, int blocks)
    {
        shipname = _shipname;
        numberofblocks = blocks;
        vertical = false;
        placed = false;
        backColor = Color.red;
        hits = new List<Shot>();

    }

    public bool checkHit(Shot s,BattleshipGrid g)
    {
        //linq is quite cool.  Are there any hits that have the same x and y in the list of hits??
        if (!hits.Any(hit=>(hit.x == s.x && hit.y == s.y)))
        {
            //I want hit to return true if the place where the shot is is filled
            

            if (checkHit(s.x, s.y, this.vertical))
            {
                hits.Add(s);
                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            Debug.Log("hit already!");
            return false;
        }

    }


    public void setBackColor(Color newcolor)
    {
        backColor = newcolor;
    }

    bool checkHit(int x, int y, bool orientation)
    {
        if (!orientation)
        {
            if (x >= _x && x < _x + numberofblocks && y == _y)
            {
                return true;

            }
        }
        else
        {
            if (y >= _y && y < _y + numberofblocks && x == _x)
            {
                return true;
            }     
        }
        return false;
    }

    bool checkFree(int x, int y, BattleshipGrid g, bool orientation)
    {
        if (!orientation)
        {
            foreach (Block b in g.blocks)
            {
                if (b.indexX >= x && b.indexX < x + numberofblocks && b.indexY == y)
                {
                    if (b.filled)
                    {
                        return false;
                    }
                }

            }
        }
        else
        {
            foreach (Block b in g.blocks)
            {
                if (b.indexY >= y && b.indexY < y + numberofblocks && b.indexX == x)
                {
                    if (b.filled)
                    {
                        return false;
                    }
                }

            }
        }
        return true;

    }

    public void place(int x, int y, bool orientation, BattleshipGrid grid)
    {
        //does not allow me to place a ship twice
        if (!placed)
        {
            if (!orientation)
            {
                //horizontal --*
                if (x + (numberofblocks-1) <= 10)
                {
                    //should fit horizontally, if no ships in the way
                    if (checkFree(x, y, grid, false))
                    {

                        foreach (Block b in grid.blocks)
                        {
                            if (b.indexX >= x && b.indexX < x + numberofblocks && b.indexY == y)
                            {

                                b.toptile.GetComponent<SpriteRenderer>().color = Color.grey;
                                b.bottomtile.GetComponent<SpriteRenderer>().color = this.backColor;
                                b.filled = true;
                            }
                        }
                        placed = true;
                        _x = x;
                        _y = y;

                    }

                }

            }
            else
            {
                //* vertical
                vertical = true;
                if (y + (numberofblocks-1) <= 10)
                {
                    if (checkFree(x, y, grid, true))
                    {
                        foreach (Block b in grid.blocks)
                        {
                            //should fit vertically, if no ships in the way
                            if (b.indexY >= y && b.indexY < y + numberofblocks && b.indexX == x)
                            {

                                b.toptile.GetComponent<SpriteRenderer>().color = Color.grey;
                                b.bottomtile.GetComponent<SpriteRenderer>().color = this.backColor;
                                b.filled = true;
                            }
                        }
                        placed = true;
                        _x = x;
                        _y = y;
                    }
                }

            }
        }

    }
}

public class BattleshipGrid
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

    public void makeClickableEnemy()
    {
        foreach (Block b in blocks)
        {
            b.toptile.AddComponent<enemyBoxController>();
            b.setEnemyClickCoordinates();

        }
    }


}

public class Block
{
    public GameObject toptile, bottomtile;
    public int indexX, indexY;
    public bool filled;


    public Block()
    {
        filled = false;
    }

    public void flipTile()
    {

    }

    public void setClickCoordinates()
    {
        if (toptile.GetComponent<playerBoxController>() != null)
        {
            toptile.GetComponent<playerBoxController>().indexX = indexX;
            toptile.GetComponent<playerBoxController>().indexY = indexY;
        }
    }

    public void setEnemyClickCoordinates()
    {
        if (toptile.GetComponent<enemyBoxController>() != null)
        {
            toptile.GetComponent<enemyBoxController>().indexX = indexX;
            toptile.GetComponent<enemyBoxController>().indexY = indexY;
        }
    }



}

public class gameSession
{
    //has the game started?
    public bool gameStarted,isMyTurn;

    //number of shots fired
    int shotsFired;

    //blocks hit (to change color)
    List<Block> hitBlocks;

    Ship[] theShips;

   




    //for hits
    public BattleshipGrid enemyGrid;

    public gameSession(Ship[] allShips)
    {
        theShips = allShips;
        isMyTurn = false;
        gameStarted = false;
    }

    public bool areAllShipsPlaced()
    {
        foreach (Ship s in theShips)
        {
            if (!s.placed)
            {
                return false;
            }
        }
        return true;
    }

    public void startGame()
    {
       
        gameStarted = true;
    }

    

  


}

public class Player
{
    public string PlayerName;
    public bool isHisTurn;

   
  
}

[System.Serializable]
public class Shot
{
    public int x, y;
    public bool hit;
    public string id;

    public Shot(int _x, int _y)
    {
        x = _x;
        y = _y;
        hit = false;
    }
}


[System.Serializable]
public class Fleet
{
    public List<Ship> allships = new List<Ship>();

    public Fleet(Ship[] _allships)
    {
        foreach (Ship s in _allships)
        {

            allships.Add(s);
        }
    }
}

public class gameManager : MonoBehaviour
{

    public BattleshipGrid playerGrid, enemyGrid;

    public Player currentPlayer, otherPlayer;

    GameObject rowLabel, rowL, buttonPrefab,timerText,theTimer;

    public gameSession session;

    bool timerrunning = false;

    public FirebaseScript dbScript;

    //these correspond with the unique keys in firebase
    public string currentPlayerKey,enemyPlayerKey;

    bool starts = false;

    GameObject sq;

    Ship[] allships;

    public Fleet battlefleet;

    public Ship currentlySelectedShip;

    int playercounter = 1;

    IEnumerator clearDB()
    {
        
        yield return dbScript.clearFirebase();
        Application.Quit();
    }

    IEnumerator waitForOtherPlayer()
    {
        yield return dbScript.getNumberOfRecords();
        playercounter = dbScript.numberOfRecords;

        while (playercounter<2)
        {
            //first player to join starts
            starts = true;

            Debug.Log("Waiting for other player" + Time.time);
            yield return dbScript.getNumberOfRecords();
            playercounter = dbScript.numberOfRecords;
           
            
            //lobby
        }
        //if another player joins the game can begin
        Player otherPlayer = new Player();
        //I need to get the key of the OTHER player
        
        yield return dbScript.getOtherPlayerKey(otherPlayer, this);

        //I now have the other player's key.  Let's randomly choose whose turn is next. 
        Debug.Log("other player has joined");

       // session.isMyTurn = true;



        


        yield return null;
    }

    IEnumerator addPlayerToFirebase()
    {

        
       yield return dbScript.initFirebase();
        currentPlayer = new Player();

        currentPlayer.PlayerName = "P"+playercounter;
        currentPlayer.isHisTurn = true;


        
        
        //yield return dbScript.clearFirebase();


        yield return dbScript.addDataClass(JsonUtility.ToJson(currentPlayer),this);

        yield return waitForOtherPlayer();

        

    }


    public IEnumerator BeginGame()
    {
        timerText.GetComponentInChildren<Text>().text = "00:00";
        while (true)
        {
            if (session.areAllShipsPlaced())
            {
               break;   
            }
            yield return null;
        }
        //this will happen only when all ships are placed.
        
        session.startGame();
        StartCoroutine(updateTimer());
        
        yield return addPlayerToFirebase();

        battlefleet = new Fleet(allships);

        yield return dbScript.saveShips(this, battlefleet);

        //start the turns. 
        if (starts)
        {
            session.isMyTurn = true;
            while (true) { 
              
              
              if (session.isMyTurn)
              {
           //         Debug.Log("my turn!");
                    yield return null;
              }
              else
              {
          //          Debug.Log("their turn!");
                    yield return null;
              }


           }
        }
        else
        {
            while (true)
            {

                if (session.isMyTurn)
                {
        //            Debug.Log("nmy turn!");
                    yield return null;
                }
                else
                {
         //           Debug.Log("ntheir turn!");
                    yield return null;
                }


            }
        }



        yield return null;

    }



    public IEnumerator updateTimer()
    {
        float timerValue = 0f;
     
        Text clockText = theTimer.GetComponentInChildren<Text>();

        timerrunning = true;

       // clockText.text = "00:00";
       while (true) { 
            if (session.isMyTurn)
            {
                timerValue++;

                float minutes = timerValue / 60f;
                float seconds = timerValue % 60f;

                clockText.text = string.Format("{0:00}:{1:00}", minutes, seconds);


                //code that is running every second
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
        
    }



    Button createWorldButton(string label, GameObject parent, Vector3 pos)
    {
        GameObject theCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/myButton"), pos, Quaternion.identity);
        theCanvas.transform.SetParent(parent.transform);
        theCanvas.GetComponentInChildren<Text>().text = label;

        theCanvas.name = label;

        theCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        theCanvas.GetComponent<Canvas>().sortingOrder = 1;

        return theCanvas.GetComponentInChildren<Button>();


    }





    // Start is called before the first frame update
    void Start()
    {
        //template for a square
        sq = Resources.Load<GameObject>("Prefabs/Square");

        //template for the labels and text that there is on screen
        rowLabel = Resources.Load<GameObject>("Prefabs/TextPrefab");

        //template for the buttons that are loaded on screen
        buttonPrefab = Resources.Load<GameObject>("Prefabs/myButton");

        //add the firebase script to the main camera
        Camera.main.gameObject.AddComponent<FirebaseScript>();

        dbScript = Camera.main.GetComponent<FirebaseScript>();

        //made a copy of rowlabel in the variable timertext
        timerText = rowLabel;

        


        allships = new Ship[5];

        Ship carrier = new Ship("Carrier",5);
        Ship battleship = new Ship("Battleship",4);
        Ship cruiser = new Ship("Cruiser",3);
        Ship submarine = new Ship("Submarine",3);
        Ship destroyer = new Ship("Destroyer",2);


        allships[4] = carrier;
        allships[3] = battleship;
        allships[2] = submarine;
        allships[1] = cruiser;
        allships[0] = destroyer;



        GameObject anchor = new GameObject("playergrid");
        GameObject anchor2 = new GameObject("enemygrid");
        GameObject anchor3 = new GameObject("shipselectiongrid");



        //draw player grid
        playerGrid = GenerateGrid(anchor);
        anchor.transform.position = new Vector3(-10f, -10f);
        anchor.transform.localScale = new Vector3(1.5f, 1.5f);
        playerGrid.makeClickable();

        //ship selection grid


        Button carrierButton = createWorldButton("Carrier", anchor3, new Vector3(0f, 0f));

        carrierButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Carrier");

                //carrierButton.enabled = false;
                currentlySelectedShip = allships[4];
                


            }
        );

        Button battleshipButton = createWorldButton("Battleship", anchor3, new Vector3(0f, -3f));

        battleshipButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Battleship");

                currentlySelectedShip = allships[3];
            }
        );


        Button submarineButton = createWorldButton("Submarine", anchor3, new Vector3(0f, -6f));

        submarineButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Submarine");
                allships[2].setBackColor(Color.blue);
                currentlySelectedShip = allships[2];
            }
        );

        Button cruiserButton = createWorldButton("Cruiser", anchor3, new Vector3(0f, -9f));

        cruiserButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Cruiser");

                currentlySelectedShip = allships[1];
            }
        );

        Button destroyerButton = createWorldButton("Destroyer", anchor3, new Vector3(0f, -12f));

        destroyerButton.onClick.AddListener(
            () =>
            {
                Debug.Log("Destroyer");

                currentlySelectedShip = allships[0];
            }
        );

        //the position of the ship selection grid.
        anchor3.transform.position = new Vector3(10f, -4f);

        enemyGrid = GenerateGrid(anchor2);
        enemyGrid.parent.transform.position = new Vector3(10f, 10f);
        enemyGrid.parent.transform.localScale = new Vector3(1.5f, 1.5f);
        enemyGrid.makeClickableEnemy();


        theTimer = Instantiate(timerText, new Vector3(-18f, 19f), Quaternion.identity);

        session = new gameSession(allships);

        StartCoroutine(BeginGame());


      
    }


    public IEnumerator waitForTurn()
    {
        // yield return new WaitForSeconds(10f);
        //session.isMyTurn = true;

        yield return null;
    }

    string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };


    BattleshipGrid GenerateGrid(GameObject parentObject)
    {
        int rowcounter = 0;
        int columncounter = 0;
        int lettercounter = 0;
        BattleshipGrid grid = new BattleshipGrid();

        for (float ycoord = -4.5f; ycoord <= 4.5f; ycoord++)
        {
            //for each row

            rowcounter++;
            rowL = Instantiate(rowLabel, new Vector3(-5.5f, ycoord), Quaternion.identity);
            rowL.GetComponentInChildren<Text>().text = rowcounter.ToString();
            rowL.transform.SetParent(parentObject.transform);
            //rowL.transform.GetChild(0).transform.position = new Vector3(-2f, ycoord));


            for (float xcoord = -4.5f; xcoord <= 4.5f; xcoord++)
            {
                //first row at the top
                //This is the last thing to happen - showing the letters across the top
                if (ycoord == 4.5f)
                {

                    rowL = Instantiate(rowLabel, new Vector3(xcoord, 5.5f), Quaternion.identity);
                    rowL.GetComponentInChildren<Text>().text = letters[lettercounter];
                    rowL.transform.SetParent(parentObject.transform);
                    lettercounter++;
                }

                columncounter++;
                Block b = new Block();
                b.bottomtile = Instantiate(sq, new Vector3(xcoord, ycoord), Quaternion.identity);
                b.toptile = Instantiate(sq, new Vector3(xcoord, ycoord), Quaternion.identity);
                b.toptile.transform.localScale = new Vector3(0.8f, 0.8f);
                b.toptile.name = "TopTile";
                b.toptile.AddComponent<BoxCollider2D>();
                b.toptile.GetComponent<BoxCollider2D>().isTrigger = true;
                b.bottomtile.GetComponent<SpriteRenderer>().color = Color.black;
                b.toptile.transform.SetParent(parentObject.transform);
                b.bottomtile.transform.SetParent(parentObject.transform);
                b.bottomtile.name = "BottomTile";
                //setting the indexes of the blocks
                b.indexX = columncounter;
                b.indexY = rowcounter;

                grid.blocks.Add(b);

            }
            columncounter = 0;
        }
        grid.parent = parentObject;
        return grid;

    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(clearDB());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(dbScript.uploadScreenshot());
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(dbScript.downloadAndSaveImage());
        }
    }
}
