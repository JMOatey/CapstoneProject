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


    public int CheckSaves()
        {
            string base_filename = "save_{0}";
            int i = 0;

            string filename = String.Format(base_filename, i);
            while(FileUtility.CheckFile(filename))
            {
                filename = String.Format(base_filename, i);
                i++;
            }

            Debug.Log(String.Format("{0} local saves found", i));

            return i;
        }


    public void PlayButton()
    {
        int saveCount = CheckSaves();

        GameSaveUtility.gameTitle = titleInput.text;
        GameSaveUtility.gameSaveSelected = String.Format("save_{0}", saveCount + 1);

        SceneManager.LoadScene("Tactics");
    }
}
