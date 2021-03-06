using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;

using UnityEngine.SceneManagement;

namespace Data
{
	public class FirebaseManager : MonoBehaviour
	{
		public static FirebaseManager Instance;

        public AudioSource Audio;
        //Firebase variables
        [Header("Firebase")] public DependencyStatus dependencyStatus;
        public FirebaseAuth auth;
        public FirebaseUser User;
        public DatabaseReference DBreference;

        //Login variables
        [Header("Login")] public TMP_InputField emailLoginField;
        public TMP_InputField passwordLoginField;
        public TMP_Text warningLoginText;
        public TMP_Text confirmLoginText;

        //Register variables
        [Header("Register")] public TMP_InputField usernameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public TMP_InputField passwordRegisterVerifyField;
        public TMP_Text warningRegisterText;

        //User Data variables
        [Header("UserData")] 
        public string usernameField ;

        public int xpField;
        public int xpPendu;
        public int xpPuissance4;
        public int xpMiniTank;

        /* public TMP_InputField killsField;
         public TMP_InputField deathsField;
         public Transform scoreboardContent;*/


        void Awake()
        {
			if (FirebaseManager.Instance==null)
            {
				FirebaseManager.Instance=this;
				DontDestroyOnLoad(this);
			}
            else
            {
                SceneManager.LoadScene(1);
            }
            
            
            //Check that all of the necessary dependencies for Firebase are present on the system
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    //If they are avalible Initialize Firebase
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        private void InitializeFirebase()
        {
            Debug.Log("Setting up Firebase Auth");
            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
            DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        }

        public void ClearLoginFeilds()
        {
            emailLoginField.text = "";
            passwordLoginField.text = "";
        }

        public void ClearRegisterFeilds()
        {
            usernameRegisterField.text = "";
            emailRegisterField.text = "";
            passwordRegisterField.text = "";
            passwordRegisterVerifyField.text = "";
        }

        //Function for the login button
        public void LoginButton()
        {
            //Call the login coroutine passing the email and password
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        }

        //Function for the register button
        public void RegisterButton()
        {
            //Call the register coroutine passing the email, password, and username
            StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        }

        //Function for the sign out button
        public void SignOutButton()
        {
            auth.SignOut();
            ClearRegisterFeilds();
            ClearLoginFeilds();
        }

        //Function for the save button
        public void SaveDataButton(string game)
        {
            StartCoroutine(UpdateUsernameAuth(usernameField));
            StartCoroutine(UpdateUsernameDatabase(usernameField));

            StartCoroutine(UpdateXp(xpPendu+xpPuissance4+xpMiniTank));
            
            StartCoroutine(UpdateXpPendu(xpPendu));
            StartCoroutine(UpdateXpPuissance4(xpPuissance4));
            StartCoroutine(UpdateXpMiniTank(xpMiniTank));
            
            /* StartCoroutine(UpdateKills(int.Parse(killsField.text)));
             StartCoroutine(UpdateDeaths(int.Parse(deathsField.text)));*/
        }
        //Function for the scoreboard button
        /* public void ScoreboardButton()
         {        
             StartCoroutine(LoadScoreboardData());
         }*/

        private IEnumerator Login(string _email, string _password)
        {
            //Call the Firebase auth signin function passing the email and password
            var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            if (LoginTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError) firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        break;
                }

                warningLoginText.text = message;
            }
            else
            {
                //User is now logged in
                //Now get the result
                User = LoginTask.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";
                StartCoroutine(LoadUserData());
                //Debug.LogFormat(xpField.ToString());

                yield return new WaitForSeconds(2);

                // usernameField.text = User.DisplayName;
                SceneManager.LoadScene(1); // Change to user data UI
                confirmLoginText.text = "";
                ClearLoginFeilds();
                ClearRegisterFeilds();
            }
        }

        private IEnumerator Register(string _email, string _password, string _username)
        {
            if (_username == "")
            {
                //If the username field is blank show a warning
                warningRegisterText.text = "Missing Username";
            }
            else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
            {
                //If the password does not match show a warning
                warningRegisterText.text = "Password Does Not Match!";
            }
            else
            {
                //Call the Firebase auth signin function passing the email and password
                var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                //Wait until the task completes
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    //If there are errors handle them
                    Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError) firebaseEx.ErrorCode;

                    string message = "Register Failed!";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "Missing Email";
                            break;
                        case AuthError.MissingPassword:
                            message = "Missing Password";
                            break;
                        case AuthError.WeakPassword:
                            message = "Weak Password";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            message = "Email Already In Use";
                            break;
                    }

                    warningRegisterText.text = message;
                }
                else
                {
                    //User has now been created
                    //Now get the result
                    User = RegisterTask.Result;

                    if (User != null)
                    {
                        //Create a user profile and set the username
                        UserProfile profile = new UserProfile {DisplayName = _username};

                        //Call the Firebase auth update user profile function passing the profile with the username
                        var ProfileTask = User.UpdateUserProfileAsync(profile);
                        //Wait until the task completes
                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        if (ProfileTask.Exception != null)
                        {
                            //If there are errors handle them
                            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                            warningRegisterText.text = "Username Set Failed!";
                        }
                        else
                        {
                            //Username is now set
                            //Now return to login screen
                            SceneManager.LoadScene(1);
                            warningRegisterText.text = "";
                            ClearRegisterFeilds();
                            ClearLoginFeilds();
                        }
                    }
                }
            }
        }

        public IEnumerator UpdateUsernameAuth(string _username)
        {
            //Create a user profile and set the username
            UserProfile profile = new UserProfile {DisplayName = _username};

            //Call the Firebase auth update user profile function passing the profile with the username
            var ProfileTask = User.UpdateUserProfileAsync(profile);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
            }
            else
            {
                //Auth username is now updated
            }
        }

        public IEnumerator UpdateUsernameDatabase(string _username)
        {
            //Set the currently logged in user username in the database
            var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Database username is now updated
            }
        }

        public IEnumerator UpdateXp(int _xp)
        {
            //Set the currently logged in user xp
            
            var DBTask = DBreference.Child("users").Child(User.UserId).Child("xp").SetValueAsync(_xp);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
            //yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Xp is now updated
            }
        }

        public IEnumerator UpdateXpPendu(int _xp)
        {
            //Set the currently logged in user xp


            var DBTask = DBreference.Child("users").Child(User.UserId).Child("xpPendu").SetValueAsync(_xp);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
            //yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Xp is now updated
            }
        }
        public IEnumerator UpdateXpPuissance4(int _xp)
        {
            //Set the currently logged in user xp


            var DBTask = DBreference.Child("users").Child(User.UserId).Child("xpPuissance4").SetValueAsync(_xp);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
            //yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Xp is now updated
            }
        }
        public IEnumerator UpdateXpMiniTank(int _xp)
        {
            //Set the currently logged in user xp


            var DBTask = DBreference.Child("users").Child(User.UserId).Child("xpMiniTank").SetValueAsync(_xp);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
            //yield return new WaitUntil(predicate: () => DBTask1.IsCompleted);

            if (DBTask.Exception != null )
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Xp is now updated
            }
        }

        private IEnumerator LoadUserData()
        {
            //Get the currently logged in user data
            var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
            

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            
            else if (DBTask.Result.Value == null)
            {
                //No data exists yet
                xpField=0 ;
                xpPendu = 0;
                xpPuissance4 = 0;
                xpMiniTank = 0;
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;
                usernameField = snapshot.Child("username").Value.ToString();

                xpField = Int32.Parse(snapshot.Child("xp").Value.ToString());
                xpPendu = Int32.Parse(snapshot.Child("xpPendu").Value.ToString());
                xpPuissance4 = Int32.Parse(snapshot.Child("xpPuissance4").Value.ToString());
                xpMiniTank = Int32.Parse(snapshot.Child("xpMiniTank").Value.ToString());
            }
        }
 		/*private void WriteNewScore (int score) 
        {
			
            // Create new entry at /user-scores/$userid/$scoreid and at
            // /leaderboard/$scoreid simultaneously
            string key = DBreference.Child("scores").Push().Key;
            LeaderBoardEntry entry = new LeaderBoardEntry(User.UserId, score);
            Dictionary<string, Object> entryValues = entry.ToDictionary();

            Dictionary<string, Object> childUpdates = new Dictionary<string, Object>();
            childUpdates["/scores/" + key] = entryValues;
            childUpdates["/user-scores/" + User.UserId+ "/" + key] = entryValues;

            DBreference.UpdateChildrenAsync(childUpdates);
        }	

        private IEnumerator LoadScoreboardData()
        {
            //Get all the users data ordered by kills amount
            var DBTask = DBreference.Child("users").OrderByChild("kills").GetValueAsync();
    
            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
    
            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                //Data has been retrieved
                DataSnapshot snapshot = DBTask.Result;
    
                //Destroy any existing scoreboard elements
                foreach (Transform child in scoreboardContent.transform)
                {
                    Destroy(child.gameObject);
                }
    
                //Loop through every users UID
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    string username = childSnapshot.Child("username").Value.ToString();
                    int kills = int.Parse(childSnapshot.Child("kills").Value.ToString());
                    int deaths = int.Parse(childSnapshot.Child("deaths").Value.ToString());
                    int xp = int.Parse(childSnapshot.Child("xp").Value.ToString());
    
                    //Instantiate new scoreboard elements
                    GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                    scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, kills, deaths, xp);
                }
    
                //Go to scoareboard screen
                UIManager.instance.ScoreboardScreen();
            }
        }*/
	}
    
}

