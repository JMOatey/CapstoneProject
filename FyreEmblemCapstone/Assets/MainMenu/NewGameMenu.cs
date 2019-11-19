using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Account;

public class NewGameMenu : MonoBehaviour
{
    public TextMeshProUGUI titleInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButton()
    {
        GameSaveUtility.gameTitle = titleInput.text;
        GameSaveUtility.NewSaveName();
        GameSaveUtility.Save();
        GameSaveUtility.Load();

        SceneManager.LoadScene("Tactics");
    }
}
