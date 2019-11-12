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
    public class AccountMenu : MonoBehaviour
    {
        public TextMeshProUGUI resultMessage;
        public TextMeshProUGUI saveMessage;
        public GameObject signInButton;
        public GameObject signUpButton;
        public GameObject signOutButton;
        public GameObject saveOne;
        public GameObject saveTwo;
        public GameObject saveThree;
        public string[] saveData;
        public TokenResult userData;

        // Start is called before the first frame update
        void Start()
        {
            // Look for saved access token
            Debug.Log("Here");
            CheckLocalAccount();
        }

        // Update is called once per frame
        void Update()
        {
            // If the user is signed in, 
        }

        public void CheckLocalAccount()
        {
            // Check if a local user file exists
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, "user.txt");

            //Exit if Directory or File does not exist
            if(Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                if(File.Exists(tempPath))
                {
                    // Load user data from file
                    userData = DataSaver.LoadData<TokenResult>("user");

                    Debug.Log("Signed in as " + userData.username);
                    
                    // Update UI
                    resultMessage.SetText("Welcome " + userData.username);
                    signInButton.SetActive(false);
                    signUpButton.SetActive(false);
                    signOutButton.SetActive(true);

                    // Fetch cloud saves
                    FetchSaves();
                }
            }

            return;
        }


        public void SignOut()
        {
            // Delete local user data
            DataSaver.DeleteData("user");

            // Update UI
            resultMessage.SetText("You are not signed in");
            saveMessage.SetText("");
            signInButton.SetActive(true);
            signUpButton.SetActive(true);
            signOutButton.SetActive(false);
        }


        public void FetchSaves()
        {
            UnityWebRequest fetchSaveRequest = UnityWebRequest.Get(CloudAPI.SaveUrl);
            fetchSaveRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);

            fetchSaveRequest.SendWebRequest();

            while(!fetchSaveRequest.isDone);

            if(fetchSaveRequest.isNetworkError || fetchSaveRequest.isHttpError)
            {
                Debug.Log("Error while making sign in request: " + fetchSaveRequest.error);
            }
            else
            {
                Debug.Log(fetchSaveRequest.downloadHandler.text);
                CloudSaveResult result = JsonUtility.FromJson<CloudSaveResult>(fetchSaveRequest.downloadHandler.text);

                if(result.code)
                {
                    // Set save data
                    saveData = result.message;
                
                    // Update UI
                    if(saveData.Length == 0)
                    {
                        saveMessage.SetText("No saves found");
                    }
                    else if(saveData.Length == 1)
                    {
                        saveOne.SetActive(true);
                    }
                    else if(saveData.Length == 2)
                    {
                        saveOne.SetActive(true);
                        saveTwo.SetActive(true);

                        saveOne.transform.Translate(new Vector3(-150.0f, 0.0f, 0.0f));
                        saveTwo.transform.Translate(new Vector3(150.0f, 0.0f, 0.0f));
                    }
                    else if(saveData.Length == 3)
                    {
                        saveOne.SetActive(true);
                        saveTwo.SetActive(true);
                        saveThree.SetActive(true);

                        saveOne.transform.Translate(new Vector3(-200.0f, 0.0f, 0.0f));
                        saveThree.transform.Translate(new Vector3(200.0f, 0.0f, 0.0f));
                    }
                }
            }
        }
    }
}
