using System;
using System.IO;
using UnityEngine;

namespace Account
{

    public static class GameSaveUtility
    {
        public static string gameTitle;
        public static string gameSaveSelected;
        public static string gameSaveData;
        public static string username;


        public static void Save()
        {
            GameSave save = new GameSave();
            save.username = username;
            save.title = gameTitle;
            save.data = gameSaveData;

            FileUtility.SaveFile<GameSave>(save, GameSaveUtility.gameSaveSelected);
        }
    }

    public class FileUtility
    {
        public static bool CheckFile(string filename)
        {
            // Check if a local user file exists
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, filename + ".txt");

            //Exit if Directory or File does not exist
            if(Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                if(File.Exists(tempPath))
                {
                    return true;
                }
            }

            return false;  
        }


        public static T LoadFile<T>(string filename)
        {
            T file = DataSaver.LoadData<T>(filename);

            return file;
        }


        public static void SaveFile<T>(T data, string filename)
        {
            DataSaver.SaveData<T>(data, filename);
        }


        public static void DeleteFile(string filename)
        {
            DataSaver.DeleteData(filename);
        }
    }

    public class CloudAPI
    {
        public static string SignInUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/accounts/sign-in";
        public static string SignUpUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/accounts/sign-up";
        public static string SaveUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/saves";
        public static string ProfileUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/profiles";
    }


    [System.Serializable]
    public class GameSave
    {
        public string username;
        public string title;
        public string data;
    }


    [System.Serializable]
    public class CloudUserData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class CloudResult
    {
        public TokenResult message;
        public bool code;
    }


    [System.Serializable]
    public class CloudSignUpResult
    {
        public string message;
        public bool code;
    }


    [System.Serializable]
    public class CloudSaveResult
    {
        public GameSave[] message;
        public bool code;
    }


    [System.Serializable]
    public class TokenResult
    {
        public string username;
        public string access_token;
        public string refresh_token;
    }
}