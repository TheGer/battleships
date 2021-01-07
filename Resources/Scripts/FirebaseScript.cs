using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Extensions;
using Firebase.Unity.Editor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.IO;

//using UnityEngine.UIElements;


//this corresponds to the attributes inside firebase


public class FirebaseScript : MonoBehaviour
{
    
    DatabaseReference reference;

    //reference to the storage bucket
    FirebaseStorage storage;

    string output = "";

    int counter = 0;
    public int numberOfRecords = 0;

    //main data dictionary
    Dictionary<string, object> myDataDictionary;

    FirebaseAuth auth;

    string email = "gerrysaid.test5@gmail.com";
    string password = "IamNotSupposedToSeeThis1234!";



    public IEnumerator addDataClass(string datatoinsert,gameManager g)
    {
        //create a unique ID
        string newkey = reference.Push().Key;
        Debug.Log(newkey);
        //the key for the current player
        g.currentPlayerKey = newkey;
        //Update the unique key with the data I want to insert
        yield return StartCoroutine(updateDataClass(newkey, datatoinsert));

    }


    IEnumerator updateDataClass(string childlabel, string newdata)
    {

     
        //find the child of player4 that corresponds to playername and set the value to whatever is inside newdata
        Task updateJsonValueTask =  reference.Child(childlabel).SetRawJsonValueAsync(newdata).ContinueWithOnMainThread(
            updJsonValueTask =>
            {
                if (updJsonValueTask.IsCompleted)
                {
                   // dataupdated = true;
                }

            });


        yield return new WaitUntil(() => updateJsonValueTask.IsCompleted);




    }


    public IEnumerator uploadScreenshot()
    {
        yield return initFirebase();

        string actualfilename = "/sc_" + DateTime.Now.ToString("MM_dd_yyyyH_mm")+".png";

        string filename = Application.persistentDataPath + actualfilename;

        ScreenCapture.CaptureScreenshot(filename);

        yield return new WaitForSeconds(2f);

        Debug.Log(filename);

        storage = FirebaseStorage.DefaultInstance;

        Debug.Log(File.Exists(filename));

        
        StorageReference storage_ref = storage.GetReferenceFromUrl("gs://gerry-firebase1.appspot.com");

        StorageReference screenshots_folder = storage_ref.Child("screenshots"+ actualfilename);

        //string local_file_uri = string.Format("{0}://{1}",Uri.UriSchemeFile, filename);

        MetadataChange pngmetadata = new MetadataChange();

        pngmetadata.ContentType = "image/png";

        Task uploadScreenshotTask = screenshots_folder.PutFileAsync(filename,pngmetadata, 
            new Firebase.Storage.StorageProgress<UploadState>(state => {
            // called periodically during the upload - you can implement some sort of progress bar display here
            Debug.Log(String.Format("Progress: {0} of {1} bytes transferred.",
                               state.BytesTransferred, state.TotalByteCount));
            }), CancellationToken.None, null);
            
            
         uploadScreenshotTask.ContinueWith(resultTask => {
             if (!resultTask.IsFaulted && !resultTask.IsCanceled)
             {
                 Debug.Log("Upload finished.");
             }

             if (resultTask.IsFaulted)
             {
                 //my internet exploded or firebase exploded or some other error happened here
                 Debug.Log("Sorry, file  was not uploaded!" + resultTask.Exception);

                 return;
             }
         });
        

        yield return new WaitUntil(() => uploadScreenshotTask.IsCompleted);
        
      
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            Debug.Log("hello");
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }


    public IEnumerator downloadAndSaveImage()
    {

        string pathToSaveIn = Application.persistentDataPath;

        storage = FirebaseStorage.DefaultInstance;

        // Create local filesystem URL
        
        string filename = Application.persistentDataPath + "/warship1.jpg";

        StorageReference storage_ref = storage.GetReferenceFromUrl("gs://gerry-firebase1.appspot.com/warship1.jpg");

        // Start downloading a file
        Task task = storage_ref.GetFileAsync(filename,
          new Firebase.Storage.StorageProgress<DownloadState>((DownloadState state) => {
      // called periodically during the download
      Debug.Log(String.Format(
        "Progress: {0} of {1} bytes transferred.",
        state.BytesTransferred,
        state.TotalByteCount
      ));
          }), CancellationToken.None);

        task.ContinueWith(resultTask => {
            if (!resultTask.IsFaulted && !resultTask.IsCanceled)
            {
                Debug.Log("Download finished.");
            }
        });

        Debug.Log(filename);

        yield return new WaitUntil(() => task.IsCompleted);


        Sprite warship = LoadSprite(filename);
        GameObject.Find("backgroundImage").GetComponent<Image>().sprite = warship;


        yield return null;
    }


   public IEnumerator clearFirebase()
    {
        Task removeAllRecords = reference.RemoveValueAsync().ContinueWithOnMainThread(
            rmAllRecords =>
            {
                if (rmAllRecords.IsCompleted)
                {
                    Debug.Log("Database clear");
                }
            });

        yield return new WaitUntil(() => removeAllRecords.IsCompleted);

    }





    IEnumerator createUser()
    {
        auth = FirebaseAuth.DefaultInstance;

      



        Task createusertask = auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(
             //created an anonymous inner class inside continueonmainthread which is of type Task
             createUserTask =>
             {
                //if anything goes wrong
                if (createUserTask.IsCanceled)
                 {
                    //I pressed escape or cancelled the task
                    Debug.Log("Sorry, user was not created!");
                     return;
                 }
                 if (createUserTask.IsFaulted)
                 {
                    //my internet exploded or firebase exploded or some other error happened here
                    Debug.Log("Sorry, user was not created!" + createUserTask.Exception);
                 
                     return;
                 }
                //if anything goes wrong, otherwise
                Firebase.Auth.FirebaseUser myNewUser = createUserTask.Result;
                 Debug.Log("Your nice new user is:" + myNewUser.DisplayName + " " + myNewUser.UserId);
                //THIS IS WHAT HAPPENS AT THE END OF THE ASYNC TASK
       

             }
             );

   

        //*a better way to wait until the end of the coroutine*//
        yield return new WaitUntil(() => createusertask.IsCompleted);



    }

    //sign in to the firebase instance so we can read some data
    //Coroutine Number 1
    IEnumerator signInToFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

        //the outside task is a DIFFERENT NAME to the anonymous inner class
        Task signintask = auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(
             signInTask =>
             {
                 if (signInTask.IsCanceled)
                 {
                     //write cancelled in the console
                     Debug.Log("Cancelled!");
                     return;
                 }
                 if (signInTask.IsFaulted)
                 {
                     //write the actual exception in the console
                     Debug.Log("Something went wrong!" + signInTask.Exception);
                     return;
                 }

                 Firebase.Auth.FirebaseUser loggedInUser = signInTask.Result;
                 Debug.Log("User " + loggedInUser.DisplayName + " has logged in!");
               
             }
            );
      

        yield return new WaitUntil(() => signintask.IsCompleted);

        Debug.Log("User has signed in");



    }




    //get the number of records for a child
    public IEnumerator getNumberOfRecords()
    {
       Task numberofrecordstask =  reference.GetValueAsync().ContinueWithOnMainThread(
            getValueTask =>
            {
                if (getValueTask.IsFaulted)
                {
                    Debug.Log("Error getting data " + getValueTask.Exception);
                }

                if (getValueTask.IsCompleted)
                {
                    DataSnapshot snapshot = getValueTask.Result;
                    Debug.Log(snapshot.ChildrenCount);
                    numberOfRecords = (int)snapshot.ChildrenCount;
             
                }


            }
            );
        /*
        while (!numberofrecordsretreived)
            yield return null;*/

        yield return new WaitUntil(() => numberofrecordstask.IsCompleted);


    }

    IEnumerator getDataFromFirebase(string childLabel)
    {

        Task getdatatask = reference.Child(childLabel).GetValueAsync().ContinueWithOnMainThread(
            getValueTask =>
            {
                if (getValueTask.IsFaulted)
                {
                    Debug.Log("Error getting data " + getValueTask.Exception);
                }

                if (getValueTask.IsCompleted)
                {
                    DataSnapshot snapshot = getValueTask.Result;
                    //Debug.Log(snapshot.Value.ToString());

                    //snapshot object is casted to an instance of its type
                    myDataDictionary = (Dictionary<string, object>)snapshot.Value;


                    //    Debug.Log("Data received");
                    
                }


            }
            );
        //shock absorber
        /* while (!displaydata)
         {
             //the data has NOT YET been saved to snapshot
             yield return null;

         }*/

        yield return new WaitUntil(() => getdatatask.IsCompleted);

        //the data has been saved to snapshot here
        yield return StartCoroutine(displayData());
    }


    public IEnumerator getOtherPlayerKey(Player otherPlayer , gameManager g)
    {
        Task getotherplayer = reference.GetValueAsync().ContinueWithOnMainThread(
            getOtherPlayerTask =>
                {
                    if (getOtherPlayerTask.IsFaulted)
                    {
                        Debug.Log("Error getting data " + getOtherPlayerTask.Exception);
                    }
                    if (getOtherPlayerTask.IsCompleted)
                    {
                        DataSnapshot snapshot = getOtherPlayerTask.Result;
                        //Debug.Log(snapshot.Value.ToString());

                        //snapshot object is casted to an instance of its type
                        myDataDictionary = (Dictionary<string, object>)snapshot.Value;


                    }
                }
            
            );

        //wait until I'm done.
        yield return new WaitUntil(() => getotherplayer.IsCompleted);
        //all the data inside mydatadictionary
        //this gets me the second key (which is what I want)

        foreach (var element in myDataDictionary)
        {
            if (!(g.currentPlayerKey == element.Key.ToString()))
            {
                
                g.enemyPlayerKey = element.Key.ToString();
            }
        }


        //register shots listener


        DatabaseReference enemyshotReference = reference.Child(g.enemyPlayerKey).Child("Shots");
        DatabaseReference myshotReference = reference.Child(g.currentPlayerKey).Child("Shots");

        enemyshotReference.ChildAdded += (sender, args) => handleEnemyShot(sender, args, g);
        myshotReference.ChildChanged += (sender, args) => handleMyShot(sender, args, g);


        //Debug.Log(myDataDictionary.Keys.ToList());

    }

    public IEnumerator fireShot(gameManager g,Shot s)
    {

        
        string jsonshot = JsonUtility.ToJson(s);


        
        

        Task addshottask = reference.Child(g.currentPlayerKey).Child("Shots").Push().SetRawJsonValueAsync(jsonshot);
        

        yield return new WaitUntil(() => addshottask.IsCompleted);



    }

    void handleMyShot(object sender, ChildChangedEventArgs args, gameManager g)
    {
        DataSnapshot snapshot = args.Snapshot;

        Dictionary<string, object> myshotdata = (Dictionary<string, object>)snapshot.Value;

        Shot myshot = new Shot(Convert.ToInt32(myshotdata["x"]), Convert.ToInt32(myshotdata["y"]));

        foreach (Block b in g.enemyGrid.blocks)
        {
            if (b.indexX == myshot.x && b.indexY == myshot.y)
            {
                b.toptile.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

    }





    void handleEnemyShot(object sender, ChildChangedEventArgs args,gameManager g)
    {

        
        DataSnapshot snapshot = args.Snapshot;
        
        Dictionary<string,object> enemyshotdata = (Dictionary<string,object>)snapshot.Value;

        //selecting the value from the key, in this case the key is X and the value is the value of the enemy shot. 

        //let us highlight the shot on the playergrid. 
        Shot enemyshot = new Shot(Convert.ToInt32(enemyshotdata["x"]),Convert.ToInt32(enemyshotdata["y"]));


        foreach (Block b in g.playerGrid.blocks)
        {
            if (b.indexX == enemyshot.x && b.indexY == enemyshot.y)
            {
                b.toptile.GetComponent<playerBoxController>().flipColor();
            }
        }

        Debug.Log(enemyshot.ToString());

        //get player ships
        foreach (Ship s in g.battlefleet.allships)
        {
            //ship in your fleet has been hit!  this is added to the list of hits in the ship
            if (s.checkHit(enemyshot, g.playerGrid)) { 

                Debug.Log(s.shipname + "Has been hit!");


                enemyshot.hit = true;

                string jsonshot = JsonUtility.ToJson(enemyshot);

                Debug.Log(jsonshot);


                //update the enemy shot to tell the enemy that the shot has hit.
                reference.Child(g.enemyPlayerKey).Child("Shots").Child(snapshot.Key).SetRawJsonValueAsync(jsonshot);



                //update ships in db
                StartCoroutine(saveShips(g, g.battlefleet));


            }

        }



            g.session.isMyTurn = true;
            
 

    }

    
   

    public IEnumerator saveShips(gameManager g, Fleet f)
    {
        
        string jsonfleet = JsonUtility.ToJson(f);


        Debug.Log(f.ToString());

        Debug.Log(jsonfleet);

        
        //modified.  I don't really need a unique key for the ships.  Also makes it easier to get the list of ships from fleet.
        Task addfleettask = reference.Child(g.currentPlayerKey).Child("Ships").SetRawJsonValueAsync(jsonfleet);

        yield return new WaitUntil(() => addfleettask.IsCompleted);
    }


   public  IEnumerator getAllDataFromFirebase()
    {

        Task getdatatask = reference.GetValueAsync().ContinueWithOnMainThread(
            getValueTask =>
            {
                if (getValueTask.IsFaulted)
                {
                    Debug.Log("Error getting data " + getValueTask.Exception);
                }

                if (getValueTask.IsCompleted)
                {
                    DataSnapshot snapshot = getValueTask.Result;
                    //Debug.Log(snapshot.Value.ToString());

                    //snapshot object is casted to an instance of its type
                    myDataDictionary = (Dictionary<string, object>)snapshot.Value;

                  
                }


            }
            );
        

        yield return new WaitUntil(() => getdatatask.IsCompleted);

        //the data has been saved to snapshot here
        yield return StartCoroutine(displayData());
    }

    IEnumerator displayData()
    {
        foreach (var element in myDataDictionary)
        {
            Debug.Log(element.Key.ToString() + "<->" + element.Value.ToString());
            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }




    IEnumerator getAllData()
    {
        yield return getNumberOfRecords();
        Debug.Log(numberOfRecords);

  

        yield return getAllDataFromFirebase();

        Debug.Log("All records retreived");

        yield return null;
    }

    public IEnumerator initFirebase()
    {
        if (!signedin) { 
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gerry-firebase1.firebaseio.com/");
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            yield return signInToFirebase();
            Debug.Log("Firebase Initialized!");
            yield return true;
            signedin = true;
        } else
        {
            yield return null;
        }
    }


    public bool signedin = false;

    //list data from firebase
    void Start()
    {



    }



}
