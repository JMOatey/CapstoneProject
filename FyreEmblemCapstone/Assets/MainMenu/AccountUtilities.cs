using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Account
{

    public static class GameSaveUtility
    {
        public static string gameTitle;
        public static string gameSaveSelected;
        public static SaveData gameSaveData;
        public static bool loadGame = false;
        public static string username;
        public static int gameID;


        public static void Save()
        {
            GameSave save = new GameSave();
            save.username = username;
            save.title = gameTitle;
            save.data = gameSaveData;
            save.id = gameID;
            save.time = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            Debug.Log(GameSaveUtility.gameSaveSelected);
            FileUtility.SaveFile<GameSave>(save, Path.Combine("saves", GameSaveUtility.gameSaveSelected));
        }


        public static void Load()
        {
            GameSave save = FileUtility.LoadFile<GameSave>(Path.Combine("saves", GameSaveUtility.gameSaveSelected));
            username = save.username;
            gameTitle = save.title;
            gameID = save.id;
            gameSaveData = save.data;

            // Debug.Log(username);
            Debug.Log(gameTitle);
        }


        public static List<GameSave> LoadSaves()
        {
            List<GameSave> loadedSaves = new List<GameSave>();
            string tempPath = Path.Combine(Application.persistentDataPath, "data/saves");
            // Debug.Log("Searching: " + tempPath);

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            string[] files = Directory.GetFiles(tempPath);
            Debug.Log(String.Format("Loading {0} file(s)", files.Length));

            for(int i = 0; i < files.Length; i++)
            {
                string filename = Path.GetFileNameWithoutExtension(files[i]);
                filename = Path.Combine("saves", filename);

                // Debug.Log(filename);
                GameSave save = FileUtility.LoadFile<GameSave>(filename);
                // Debug.Log(save.id); 

                loadedSaves.Add(save);
            }

            return loadedSaves;
        }


        public static void NewSaveName()
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "data/saves");
            // Debug.Log("Searching: " + tempPath);

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            string[] files = Directory.GetFiles(tempPath);
            // Debug.Log(String.Format("Found {0} file(s)", files.Length));

            int new_id = 0;
            for(int i = 0; i < files.Length; i++)
            {
                // string filename = files[i];
                string filename = Path.GetFileNameWithoutExtension(files[i]);
                filename = Path.Combine("saves", filename);

                // Debug.Log(filename);
                GameSave save = FileUtility.LoadFile<GameSave>(filename);
                // Debug.Log(save.id); 

                if(save.id >= new_id)
                {
                    new_id = save.id;
                }
            }

            gameID = new_id+1;
            gameSaveSelected = String.Format("save_{0}", new_id+1);
            Debug.Log("New File: " + gameSaveSelected);
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
        public SaveData data;
        public int id;
        public int time;
    }


    [System.Serializable]
    public class SaveData
    {
        public string current;
        public SaveUnit support;
        public SaveUnit main;
        public SaveUnit bad;
    }


    [System.Serializable]
    public class SaveUnit
    {
        public string name;
        public float x, y, z;
        public float f_x, f_y, f_z;
        public int health;
        public string tag;
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
    public class CloudSaveUpdateResult
    {
        public string message;
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