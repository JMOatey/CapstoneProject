using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

namespace Account
{
    public class AccountSignUpMenu : MonoBehaviour
    {
        public string username;
        public string password;
        public string passwordConfirm;

        public TextMeshProUGUI usernameInput;
        public TextMeshProUGUI passwordInput;
        public TextMeshProUGUI passwordConfirmInput;
        public GameObject accountSignUpMenu;
        public GameObject accountMenu;
        public GameObject accountMenuScript;


        public void SignIn()
        {
            username = usernameInput.text;
            password = passwordInput.text;
            passwordConfirm = passwordConfirmInput.text;

            if(password == passwordConfirm)
            {
                CloudUserData data = new CloudUserData();
                data.username = username;
                data.password = password;

                string dataJson = JsonUtility.ToJson(data);

                Debug.Log(dataJson);

                UnityWebRequest signInRequest = UnityWebRequest.Post(CloudAPI.SignUpUrl, dataJson);

                signInRequest.SendWebRequest();

                while(!signInRequest.isDone);

                if(signInRequest.isNetworkError || signInRequest.isHttpError)
                {
                    Debug.Log("Error while making sign in request: " + signInRequest.error);
                }
                else
                {
                    string signUpResult = signInRequest.downloadHandler.text;

                    // Debug.Log("Recieved: " + signInResult);

                    CloudResult result = JsonUtility.FromJson<CloudResult>(signUpResult);

                    if(result.code)
                    {
                        // Debug.Log(result.message.access_token);
                        // Debug.Log("Username to save: " + username);

                        result.message.username = username;

                        // Debug.Log("Username to save: " + result.message.username);

                        Debug.Log(JsonUtility.ToJson(result.message));

                        DataSaver.SaveData<TokenResult>(result.message, "user");

                        accountSignUpMenu.SetActive(false);
                        accountMenu.SetActive(true);

                        accountMenuScript.GetComponent<AccountMenu>().CheckLocalAccount();
                        // AccountMenu.CheckLocalAccount();
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
