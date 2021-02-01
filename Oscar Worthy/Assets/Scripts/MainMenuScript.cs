using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject mainMenu;
    public GameObject controls;

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnContolsButtonPressed()
    {
        controls.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnControlsBackButtonPressed()
    {
        mainMenu.SetActive(true);
        controls.SetActive(false);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
