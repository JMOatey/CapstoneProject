using System;
using System.Collections;
using System.Collections.Generic;
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

            if(CheckSaves() > 0)
            {
                Debug.Log("Found local saves");
                noSaveFoundMessage.SetActive(false);

                LoadSaves();
                UpdateSaveList();
            }
            else
            {
                if(FileUtility.CheckFile("user"))
                {
                    Debug.Log("Found local user");
                    userData = FileUtility.LoadFile<TokenResult>("user");

                    FetchSaves();
                    if(CheckSaves() > 0)
                    {
                        Debug.Log("Fetched user saves");

                        LoadSaves();
                        UpdateSaveList();
                    }
                }

                noSaveFoundMessage.SetActive(true);
            }

            previousTime = System.DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            if(System.DateTime.Now.AddSeconds(-10.0f).CompareTo(previousTime) > 0)
            {
                previousTime = System.DateTime.Now;

                FetchSaves();
                if(CheckSaves() > 0)
                {
                    LoadSaves();
                    UpdateSaveList();
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
                    Debug.Log(String.Format("Deleting {0} panels", panels.Length));

                    for(i = 0; i < panels.Length; i++)
                    {
                        Debug.Log(panels[i].transform.position);
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
                    gameSavePanel.GetComponentInChildren<TextMeshProUGUI>().SetText(String.Format("{2}-{0} [{1}]", save.id, save.username, save.title));
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

            // gameSaves = new List<GameSave>();

            // Load saves
            gameSaves = GameSaveUtility.LoadSaves();

            // string base_filename = "save_{0}";
            // int i = 0;

            // string filename = String.Format(base_filename, i);
            // while(FileUtility.CheckFile(filename))
            // {
            //     GameSave save = FileUtility.LoadFile<GameSave>(filename);

            //     gameSaves.Add(save);

            //     Debug.Log("Loaded " + filename);

            //     i++;
            //     filename = String.Format(base_filename, i);
            // }

            // return false;
            if(gameSaves.Count > 0)
            {
                return true;
            }

            return false;
        }


        public bool FetchSaves()
        {
            if(FileUtility.CheckFile("user"))
            {
                Debug.Log(userData.access_token);

                // Make request to sign in
                UnityWebRequest fetchSavesRequest = UnityWebRequest.Get(CloudAPI.SaveUrl);
                fetchSavesRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);
                fetchSavesRequest.SendWebRequest();

                while(!fetchSavesRequest.isDone);

                // Handle request result
                if(fetchSavesRequest.isNetworkError || fetchSavesRequest.isHttpError)
                {
                    Debug.Log("Error while making sign in request: " + fetchSavesRequest.error);
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


        public bool PostSaves()
        {
            if(FileUtility.CheckFile("user"))
            {
                // int i = 0;
                foreach(GameSave save in gameSaves)
                {
                    string data = JsonUtility.ToJson(save);

                    UnityWebRequest postSavesRequest = UnityWebRequest.Put(String.Format(CloudAPI.SaveUrl + "/{0}", save.id), data);
                    postSavesRequest.SetRequestHeader("Authorization", "Bearer " + userData.access_token);
                    postSavesRequest.SendWebRequest();

                    while(!postSavesRequest.isDone);

                    // Handle request result
                if(postSavesRequest.isNetworkError || postSavesRequest.isHttpError)
                {
                    Debug.Log("Error while making sign in request: " + postSavesRequest.error);
                }
                else
                {
                    string fetchSavesResult = postSavesRequest.downloadHandler.text;

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
                
                
            }

            return false;
        }
    }
    
}
