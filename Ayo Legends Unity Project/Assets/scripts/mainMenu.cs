using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  
using System.Collections;

public class mainMenu : MonoBehaviour {

    public GameObject       mainMenuPanel;
    public GameObject       storyModeMenuPanel;
    public GameObject       multiplayerModeMenuPanel;
  

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void mainMenuToStoryMode() {

        mainMenuPanel.SetActive(false);
        storyModeMenuPanel.SetActive(true); 
    }

    public void mainMenuToMultiPlayer() {
        mainMenuPanel.SetActive(false);
        multiplayerModeMenuPanel.SetActive(true);
    }

    public void exitGame() {
        Application.Quit(); 
    }

    public void storyModeToMainMenu() {
        mainMenuPanel.SetActive(true);
        storyModeMenuPanel.SetActive(false);

    }

    public void playStoryModeLevel(int level) {
        PlayerPrefs.SetString("mode", "story");
        PlayerPrefs.SetInt("currentLevel", level);
        SceneManager.LoadScene("storyMode"); 
    }

    public void playMultiOverNetwork() {
        SceneManager.LoadScene("multiplayer"); 
    }

    public void playMultiOnSameDevice() {
        PlayerPrefs.SetString("mode", "multi"); 
        SceneManager.LoadScene("core"); 
    }

    public void multiPlayerToMainMenu() {
        mainMenuPanel.SetActive(true);
        multiplayerModeMenuPanel.SetActive(false);
    }
}
