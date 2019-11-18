using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject newGameMenu;
    public GameObject loadGameMenu;
    public GameObject profileMenu;
    public GameObject aboutMenu;

    public GameObject newGameButton;
    public GameObject loadGameButton;
    public GameObject profileGameButton;
    public GameObject aboutButton;

    private bool newGameActive;
    private bool loadGameActive;
    private bool profileActive;
    private bool aboutActive;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NewGameButton()
    {
        Debug.Log("New Game Menu");

        loadGameActive = false;
        profileActive = false;
        aboutActive = false;
        loadGameMenu.SetActive(false);
        profileMenu.SetActive(false);
        aboutMenu.SetActive(false);

        newGameActive = !newGameActive;
        newGameMenu.SetActive(newGameActive);
    }

    public void LoadGameButton()
    {
        Debug.Log("Load Game Menu");

        newGameActive = false;
        profileActive = false;
        aboutActive = false;
        newGameMenu.SetActive(false);
        profileMenu.SetActive(false);
        aboutMenu.SetActive(false);

        loadGameActive = !loadGameActive;
        loadGameMenu.SetActive(loadGameActive);
    }

    public void ProfileButton()
    {
        Debug.Log("Profile Menu");

        newGameActive = false;
        loadGameActive = false;
        aboutActive = false;
        newGameMenu.SetActive(false);
        loadGameMenu.SetActive(false);
        aboutMenu.SetActive(false);

        profileActive = !profileActive;
        profileMenu.SetActive(profileActive);
    }

    public void AboutButton()
    {
        Debug.Log("About Menu");

        newGameActive = false;
        loadGameActive = false;
        profileActive = false;
        newGameMenu.SetActive(false);
        loadGameMenu.SetActive(false);
        profileMenu.SetActive(false);
        
        aboutActive = !aboutActive;
        aboutMenu.SetActive(aboutActive);
    }

    public void ExitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
