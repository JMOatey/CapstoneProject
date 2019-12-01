using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Account
{
    public class LoadGameMenu : MonoBehaviour
    {
        public GameObject noSaveFoundMessage;
        public DateTime previousTime;

        public TokenResult userData;

        public List<GameSave> gameSaves;
        public GameObject gameSaveTemplate;
        public GameObject gameSavesContainer;

        public string gameSaveSelected;


        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Loading the Load Game Menu");

            gameSaves = new List<GameSave>();

            if(FileUtility.CheckFile("user"))
            {
                Debug.Log("Found local user");
                userData = FileUtility.LoadFile<TokenResult>("user");

                Debug.Log("Signed in, syncing saves");
                SyncSaves();
            }

            if(CheckSaves() > 0)
            {
                Debug.Log("Found local saves");
                noSaveFoundMessage.SetActive(false);

                LoadSaves();
                UpdateSaveList();
            }

            previousTime = System.DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            if(userData == null || userData.access_token == "")
            {
                if(FileUtility.CheckFile("user"))
                {
                    Debug.Log("Found local user");
                    userData = FileUtility.LoadFile<TokenResult>("user");
                }
            }

            if(System.DateTime.Now.AddSeconds(-10.0f).CompareTo(previousTime) > 0)
            {
                previousTime = System.DateTime.Now;

                SyncSaves();
                if(CheckSaves() > 0)
                {
                    noSaveFoundMessage.SetActive(false);
                    LoadSaves();
                    UpdateSaveList();
                }
                else
                {
                    noSaveFoundMessage.SetActive(true);
                }
            }
        }


        public void PlayButton()
        {
            GameSaveUtility.gameSaveSelected = gameSaveSelected;
            GameSaveUtility.Load();
            GameSaveUtility.loadGame = true;
            SceneManager.LoadScene("Tactics");
        }


        public void UpdateSaveList()
        {
            if(CheckSaves() > 0)
            {
                noSaveFoundMessage.SetActive(false);

                int i = 0;
                try
                {
                    GameObject[] panels = GameObject.FindGameObjectsWithTag("GameSavePanel");
                    // Debug.Log(String.Format("Deleting {0} panels", panels.Length));

                    for(i = 0; i < panels.Length; i++)
                    {
                        // Debug.Log(panels[i].transform.position);
                        GameObject.Destroy(panels[i]);
                    }
                }
                catch (UnityException err)
                {
                    Debug.Log(err.Message);                    
                }
                               

                i = 0;
                foreach(GameSave save in gameSaves)
                {
                    GameObject gameSavePanel = GameObject.Instantiate(
                        gameSaveTemplate,
                        gameSaveTemplate.transform.position,
                        new Quaternion(),
                        gameSavesContainer.transform);

                    gameSavePanel.transform.Translate(0.0f, -65.0f * i, 0.0f);
                    gameSavePanel.GetComponentInChildren<TextMeshProUGUI>().SetText(String.Format("{0}", save.title));
                    gameSavePanel.tag = "GameSavePanel";
                    // gameSavePanel.GetComponent<Button>().onClick.AddListener(delegate{SelectSave(String.Format("save_{0}", i));});

                    int j = save.id;
                    gameSavePanel.GetComponent<Button>().onClick.AddListener(
                        () => {
                            SelectSave(String.Format("save_{0}", j));
                        }
                    );
                    gameSavePanel.SetActive(true);


                    // Debug.Log(gameSavePanel.transform.position);

                    i++;
                }
            }
        }


        public void SelectSave(string save)
        {
            Debug.Log(save);
            gameSaveSelected = save;
        }


        public int CheckSaves()
        {
            return GameSaveUtility.LoadSaves().Count;
        }

        public bool LoadSaves()
        {
            // Clear saves

            // Load saves
            gameSaves = GameSaveUtility.LoadSaves();

            if(gameSaves.Count > 0)
            {
                return true;
            }

            return false;
        }


        public int CheckSave(int id, GameSave[] saves)
        {
            // Debug.Log(String.Format("Looking for {0}", id));
            for(int i = 0; i < saves.Length; i++)
            {
                // Debug.Log(String.Format("Comparing to {0}", saves[i].id));
                if(saves[i].id == id)
                {
                    return i;
                }
            }

            // Debug.Log(String.Format("{0} not found", id));
            return -1;
        }


        public void SyncSaves()
        {
            // Pull saves
            // Debug.Log("Updating local saves in memory");
            LoadSaves();
            // Debug.Log("Syncing save data");
            // Debug.Log(String.Format("{0} Local saves", gameSaves.Count));

            GameSave[] remote_saves = ReadGameSaves();

            // Create new saves
            GameSave[] local_only_saves = new GameSave[gameSaves.Count];
            GameSave[] local_and_remote_saves = new GameSave[remote_saves.Length];
            GameSave[] remote_only_saves = new GameSave[remote_saves.Length];

            // If remote saves were found
            // Get saves that are local but not remote
            int i = 0;
            int j = 0;
            foreach(GameSave save in gameSaves)
            {
                // If the game is not remote
                if(CheckSave(save.id, remote_saves) < 0)
                {
                    local_only_saves[i] = save;
                    i++;
                }
                else
                {
                    local_and_remote_saves[j] = save;
                    j++;
                }
            }

            // Get saves that are remote but not local
            int k = 0;
            foreach(GameSave save in remote_saves)
            {
                if(CheckSave(save.id, gameSaves.ToArray()) < 0)
                {
                    remote_only_saves[k] = save;
                    k++;
                }
            }

            // Debug.Log(String.Format("{0} Local only saves", local_only_saves.Length));
            // Debug.Log(String.Format("{0} Remote only saves", remote_only_saves.Length));
            // Debug.Log(String.Format("{0} Local and Remote saves", local_and_remote_saves.Length));

            // Update remote for new saves
            if(CreateGameSaves(local_only_saves))
            {
                Debug.Log("Created new saves");
            }

            // Update old saves
            if(UpdateGameSaves(local_and_remote_saves))
            {
                Debug.Log("Updated old saves");
            }

            // Update local saves
            if(SaveGameSaves(remote_only_saves))
            {
                Debug.Log("Updated local saves");
            }
        }   


        public bool SaveGameSaves(GameSave[] saves)
        {
            foreach(GameSave save in saves)
            {
                if(save != null)
                {
                    FileUtility.SaveFile<GameSave>(save, Path.Combine("saves", String.Format("save_{0}", save.id)));
                }
            }

            return false;
        }


        public GameSave[] ReadGameSaves()
        {
            if(FileUtility.CheckFile("user"))
            {
                if(userData == null || userData.access_token == "")
                {
                    userData = FileUtility.LoadFile<TokenResult>("user");
                }

                // Make request
                // Debug.Log(userData.access_token);
                UnityWebRequest fetchSavesRequest = UnityWebRequest.Get(CloudAPI.SaveUrl);
                // Debug.Log(userData.access_token);
                fetchSavesRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);
                fetchSavesRequest.SendWebRequest();

                while(!fetchSavesRequest.isDone);

                // Handle request result
                if(fetchSavesRequest.isNetworkError || fetchSavesRequest.isHttpError)
                {
                    Debug.Log("Error while making fetch save request: " + fetchSavesRequest.error);
                }
                else
                {
                    string fetchSavesResult = fetchSavesRequest.downloadHandler.text;

                    CloudSaveResult result = JsonUtility.FromJson<CloudSaveResult>(fetchSavesResult);

                    if(result.code)
                    {
                        return result.message;
                    }
                }
            }

            return null;
        }


        public bool CreateGameSaves(GameSave[] saves)
        {
            if(FileUtility.CheckFile("user"))
            {
                if(userData == null || userData.access_token == "")
                {
                    userData = FileUtility.LoadFile<TokenResult>("user");
                }
                {
                    userData = FileUtility.LoadFile<TokenResult>("user");
                }

                foreach(GameSave save in saves)
                {
                    if(save != null)
                    {
                        string saveData = JsonUtility.ToJson(save);
                        
                        // Make request
                        UnityWebRequest createSavesRequest = UnityWebRequest.Post(String.Format(CloudAPI.SaveUrl + "/{0}", save.id), saveData);
                        createSavesRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);
                        createSavesRequest.SendWebRequest();

                        while(!createSavesRequest.isDone);

                        // Handle request result
                        if(createSavesRequest.isNetworkError || createSavesRequest.isHttpError)
                        {
                            Debug.Log("Error while making create save request: " + createSavesRequest.error);
                        }
                        else
                        {
                            string fetchSavesResult = createSavesRequest.downloadHandler.text;
                            CloudSaveResult result = JsonUtility.FromJson<CloudSaveResult>(fetchSavesResult);

                            if(!result.code)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }


        public bool UpdateGameSaves(GameSave[] saves)
        {
            // Debug.Log(saves.Length);
            if(FileUtility.CheckFile("user"))
            {
                if(userData == null || userData.access_token == "")
                {
                    userData = FileUtility.LoadFile<TokenResult>("user");
                }

                foreach(GameSave save in saves)
                {
                    if(save != null)
                    {
                        string saveData = JsonUtility.ToJson(save);
                        
                        // Make request
                        UnityWebRequest createSavesRequest = UnityWebRequest.Put(String.Format(CloudAPI.SaveUrl + "/{0}", save.id), saveData);
                        createSavesRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);
                        createSavesRequest.SendWebRequest();

                        while(!createSavesRequest.isDone);

                        // Handle request result
                        if(createSavesRequest.isNetworkError || createSavesRequest.isHttpError)
                        {
                            Debug.Log("Error while making update save request: " + createSavesRequest.error);
                        }
                        else
                        {
                            string fetchSavesResult = createSavesRequest.downloadHandler.text;
                            // Debug.Log(fetchSavesResult);
                            CloudSaveUpdateResult result = JsonUtility.FromJson<CloudSaveUpdateResult>(fetchSavesResult);

                            if(!result.code)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }


        public bool FetchSaves()
        {
            if(FileUtility.CheckFile("user"))
            {
                if(userData == null || userData.access_token == "")
                {
                    userData = FileUtility.LoadFile<TokenResult>("user");
                }
                // Debug.Log(userData.access_token);

                // Make request to sign in
                UnityWebRequest fetchSavesRequest = UnityWebRequest.Get(CloudAPI.SaveUrl);
                fetchSavesRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);
                fetchSavesRequest.SendWebRequest();

                while(!fetchSavesRequest.isDone);

                // Handle request result
                if(fetchSavesRequest.isNetworkError || fetchSavesRequest.isHttpError)
                {
                    Debug.Log("Error while making fetch save request: " + fetchSavesRequest.error);
                }
                else
                {
                    string fetchSavesResult = fetchSavesRequest.downloadHandler.text;

                    CloudSaveResult result = JsonUtility.FromJson<CloudSaveResult>(fetchSavesResult);

                    if(result.code)
                    {
                        Debug.Log(JsonUtility.ToJson(result.message));

                        int saveCount = CheckSaves();

                        for(int i = 0; i < result.message.Length; i++)
                        {
                            FileUtility.SaveFile(JsonUtility.ToJson(result.message[i]), String.Format("save_{0}", saveCount + i));
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
    
}
