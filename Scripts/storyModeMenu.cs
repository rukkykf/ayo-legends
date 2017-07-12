using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic; 

public class storyModeMenu : MonoBehaviour {
    public List<Sprite>     levelImages;
    public List<Button>     levelButtons; 

    void Awake() {
        //check if this is the first time the game is being played and set level to a value. 
        int level;
        if(PlayerPrefs.HasKey("Level")) {
            level = PlayerPrefs.GetInt("Level"); 
        }
        else {
            level = 1;
            PlayerPrefs.SetInt("Level", 1); 
        }

        for(int i = 0; i < level; i++) {
            levelButtons[i].GetComponent<Image>().sprite = levelImages[i];
            levelButtons[i].GetComponent<Button>().interactable = true; 
        }
    }
}
