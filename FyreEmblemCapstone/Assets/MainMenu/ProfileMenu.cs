using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace Account
{
    public class ProfileMenu : MonoBehaviour
    {
        public GameObject signInMenuButton;
        public GameObject signUpMenuButton;

        public GameObject signInButton;
        public GameObject signUpButton;
        public GameObject signOutButton;

        public GameObject signInMenu;
        public GameObject signUpMenu;
        public GameObject signedInMenu;

        // Sign in stuff

        public TextMeshProUGUI usernameSignInInput;
        public TextMeshProUGUI passwordSignInput;
        public TextMeshProUGUI usernameSignUpInput;
        public TextMeshProUGUI passwordSignUpInput;
        public TextMeshProUGUI passwordSignUpConfirmInput;

        // Menu stuff
        public TextMeshProUGUI profileTitle;
        public TextMeshProUGUI hoursPlayed;
        public TextMeshProUGUI damageDealt;
        public TextMeshProUGUI gamesPlayed;
        public TextMeshProUGUI turnsPlayed;
        public TextMeshProUGUI tilesWalked;
        public TextMeshProUGUI unitesDefeated;

        // end menu stuff

        public GameObject signedOutMenu;


        public TokenResult userData;

        // Start is called before the first frame update
        void Start()
        {
            if(CheckSave("user"))
            {
                LoadSave("user");
                signedOutMenu.SetActive(false);
                signedInMenu.SetActive(true);
            }
            else
            {
                signedInMenu.SetActive(false);
                signedOutMenu.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public bool CheckSave(string save_file)
        {
            return FileUtility.CheckFile(save_file);              
        }


        public bool LoadSave(string save_file)
        {
            if(!CheckSave(save_file))
            {
                return false;
            }

            userData = FileUtility.LoadFile<TokenResult>(save_file);

            // load menu data
            if(userData.username.Length > 10)
            {
                profileTitle.fontSize = 18;
            }
            else if(userData.username.Length > 7)
            {
                profileTitle.fontSize = 24;
            }
            else if(userData.username.Length > 4)
            {
                profileTitle.fontSize = 30;
            }
            profileTitle.SetText("Welcome " + userData.username + "!");
            Debug.Log("Welcome " + userData.username + "!");

            return true;
        }


        public bool SignIn(string username, string password)
        {
            CloudUserData data = new CloudUserData();
            data.username = username;
            data.password = password;

            // Make request to sign in
            string dataJson = JsonUtility.ToJson(data);
            UnityWebRequest signInRequest = UnityWebRequest.Post(CloudAPI.SignInUrl, dataJson);
            signInRequest.SendWebRequest();
            while(!signInRequest.isDone);

            // Handle request result
            if(signInRequest.isNetworkError || signInRequest.isHttpError)
            {
                Debug.Log("Error while making sign in request: " + signInRequest.error);
            }
            else
            {
                string signInResult = signInRequest.downloadHandler.text;

                CloudResult result = JsonUtility.FromJson<CloudResult>(signInResult);

                if(result.code)
                {
                    result.message.username = username;

                    Debug.Log(JsonUtility.ToJson(result.message));

                    FileUtility.SaveFile<TokenResult>(result.message, "user");

                    return true;
                }
            }

            return false;
        }


        public bool SignUp(string username, string password)
        {
            CloudUserData data = new CloudUserData();
            data.username = username;
            data.password = password;

            // Make request to sign in
            string dataJson = JsonUtility.ToJson(data);
            UnityWebRequest signUpRequest = UnityWebRequest.Post(CloudAPI.SignUpUrl, dataJson);
            signUpRequest.SendWebRequest();
            while(!signUpRequest.isDone);

            // Handle request result
            if(signUpRequest.isNetworkError || signUpRequest.isHttpError)
            {
                Debug.Log("Error while making sign in request: " + signUpRequest.error);
            }
            else
            {
                string signUpResult = signUpRequest.downloadHandler.text;

                CloudSignUpResult result = JsonUtility.FromJson<CloudSignUpResult>(signUpResult);

                if(result.code)
                {
                    Debug.Log(JsonUtility.ToJson(result.message));
                    return true;
                }
            }

            return false;
        }


        public bool DeleteSave(string save_file)
        {
            if(!CheckSave(save_file))
            {
                return false;
            }

            FileUtility.DeleteFile(save_file);

            return true;
        }

        public void SignInButton()
        {
            string username = usernameSignInInput.text;
            string password = passwordSignInput.text;
            if(SignIn(username, password))
            {
                if(LoadSave("user"))
                {
                    signInMenu.SetActive(false);
                    signedInMenu.SetActive(true); 
                }
            }
        }


        public void SignOutButton()
        {
            DeleteSave("user");
            signedInMenu.SetActive(false);
            signedOutMenu.SetActive(true);
        }


        public void SignUpButton()
        {
            string username = usernameSignUpInput.text;
            string password = passwordSignUpInput.text;
            string passwordConfirm = passwordSignUpConfirmInput.text;

            if(password == passwordConfirm)
            {
                if(SignUp(username, password))
                {
                    if(SignIn(username, password))
                    {
                        if(LoadSave("user"))
                        {
                            signUpMenu.SetActive(false);
                            signedInMenu.SetActive(true); 
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Passwords must match");
            }
        }
    }
    
}
