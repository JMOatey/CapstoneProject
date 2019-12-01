using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Account;

public class NewGameMenu : MonoBehaviour
{
    public TextMeshProUGUI titleInput;
    public GameObject playButton;
    public TextMeshProUGUI playButtonText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(titleInput.text);
        if(titleInput.text.Length > 4)
        {
            playButton.GetComponent<Button>().interactable = true;
            playButtonText.text = "PLAY";
            playButtonText.fontSize = 64f;
        }
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
