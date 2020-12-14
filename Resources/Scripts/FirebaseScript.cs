using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.UIElements;


//this corresponds to the attributes inside firebase


public class FirebaseScript : MonoBehaviour
{
    
    DatabaseReference reference;

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

            //Debug.Log(myDataDictionary.Keys.ToList());

    }

    public IEnumerator addShot(gameManager g,Shot s)
    {
        string jsonshot = JsonUtility.ToJson(s);
        Task addshottask = reference.Child(g.currentPlayerKey).Push().SetRawJsonValueAsync(jsonshot);

        yield return new WaitUntil(() => addshottask.IsCompleted);

    }

    public IEnumerator saveShips(gameManager g, Fleet f)
    {
        
        string jsonfleet = JsonUtility.ToJson(f);


        Debug.Log(f.ToString());

        Debug.Log(jsonfleet);

        

        Task addfleettask = reference.Child(g.currentPlayerKey).Push().SetRawJsonValueAsync(jsonfleet);

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
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://gerry-firebase1.firebaseio.com/");
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        yield return signInToFirebase();
        Debug.Log("Firebase Initialized!");
        yield return true;
    }




    //list data from firebase
    void Start()
    {



    }



}
