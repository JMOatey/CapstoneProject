using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Account;

public class ExitMenu : MonoBehaviour
{
    public GameObject exitMenuBox;
    public GameObject confirmMenu;
    public bool exitMenuActive;
    public bool gameSaved;


    public GameObject mainGuy;
    public GameObject supportGuy;
    public GameObject badGuy;

    // Start is called before the first frame update
    void Start()
    {
        exitMenuActive = false;
        gameSaved = false;

        Debug.Log("Title: " + GameSaveUtility.gameTitle);
        Debug.Log("Username: " + GameSaveUtility.username);
        Debug.Log(GameSaveUtility.gameID);
        Debug.Log(JsonUtility.ToJson(GameSaveUtility.gameSaveData));

        if(GameSaveUtility.loadGame)
        {
            Debug.Log("Loading Game");
            LoadSaveData(GameSaveUtility.gameSaveData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            gameSaved = false;
            exitMenuActive = !exitMenuActive;
            exitMenuBox.SetActive(exitMenuActive);
        }

    }


    public void LoadSaveData(SaveData data)
    {
        if(data.main != null && data.main.health > 0)
        {
            mainGuy.transform.position = new Vector3(data.main.x, data.main.y, data.main.z);    
            mainGuy.transform.forward = new Vector3(data.main.f_x, data.main.f_y, data.main.f_z);
            mainGuy.GetComponent<Unit>().Health = data.main.health;
        }
        else
        {
            GameObject.Destroy(mainGuy);
        }


        if(data.support != null && data.support.health > 0)
        {
            supportGuy.transform.position = new Vector3(data.support.x, data.support.y, data.support.z);
            supportGuy.transform.forward = new Vector3(data.support.f_x, data.support.f_y, data.support.f_z);
            supportGuy.GetComponent<Unit>().Health = data.support.health;
        }
        else
        {
            GameObject.Destroy(supportGuy);
        }

        if(data.bad != null && data.bad.health > 0)
        {
            badGuy.transform.position = new Vector3(data.bad.x, data.bad.y, data.bad.z);
            badGuy.transform.forward = new Vector3(data.bad.f_x, data.bad.f_y, data.bad.f_z);
            badGuy.GetComponent<Unit>().Health = data.bad.health;
        }
        else
        {
            GameObject.Destroy(badGuy);
        }
    }


    public SaveData GetSaveData()
    {
        SaveData data = new SaveData();

        SaveUnit main = new SaveUnit();
        SaveUnit support = new SaveUnit();
        SaveUnit bad = new SaveUnit();

        main.name = "main";
        support.name = "support";
        bad.name = "bad";

        main.tag = "Player";
        support.tag = "Player";
        bad.tag = "Enemy";

        if(mainGuy != null && mainGuy.GetComponent<Unit>().Health > 0)
        {
            main.x = mainGuy.transform.position.x;
            main.y = mainGuy.transform.position.y;
            main.z = mainGuy.transform.position.z;

            main.f_x = mainGuy.transform.forward.x;
            main.f_y = mainGuy.transform.forward.y;
            main.f_z = mainGuy.transform.forward.z;

            main.health = mainGuy.GetComponent<Unit>().Health;
        }
        data.main = main;
        
        if(supportGuy != null && supportGuy.GetComponent<Unit>().Health > 0)
        {
            support.x = supportGuy.transform.position.x;
            support.y = supportGuy.transform.position.y;
            support.z = supportGuy.transform.position.z;

            support.f_x = supportGuy.transform.forward.x;
            support.f_y = supportGuy.transform.forward.y;
            support.f_z = supportGuy.transform.forward.z;

            support.health = supportGuy.GetComponent<Unit>().Health;
        }
        data.support = support;

        if(supportGuy != null && supportGuy.GetComponent<Unit>().Health > 0)
        {
            bad.x = badGuy.transform.position.x;
            bad.y = badGuy.transform.position.y;
            bad.z = badGuy.transform.position.z;

            bad.f_x = badGuy.transform.forward.x;
            bad.f_y = badGuy.transform.forward.y;
            bad.f_z = badGuy.transform.forward.z;
            
            bad.health = badGuy.GetComponent<Unit>().Health;
        }        
        data.bad = bad;

        return data;
    }


    public void SaveGame()
    {
        Debug.Log("Game Saved");
        GameSaveUtility.gameSaveData = GetSaveData();
        GameSaveUtility.Save();
    }


    public void ExitGame()
    {
        Debug.Log("Exit Game");
        SceneManager.LoadScene("MainMenu");
    }


    public void ContinueButton()
    {
        gameSaved = false;
        exitMenuActive = false;
        exitMenuBox.SetActive(false);
    }


    public void SaveButton()
    {
        SaveGame();
        gameSaved = true;
    }


    public void ExitButton()
    {
        if(!gameSaved)
        {
            confirmMenu.SetActive(true);
        }
        else
        {
            ExitGame();
        }
    }


    public void SaveAndExitButton()
    {
        SaveGame();
        ExitGame();
    }


    public void ExitGameButton()
    {
        ExitGame();
    }
}