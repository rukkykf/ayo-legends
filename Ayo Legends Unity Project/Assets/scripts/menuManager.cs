using UnityEngine;
using System.Collections;

public class menuManager : MonoBehaviour {

    public introScene myMenu; 
    private introScene currentMenu;

    public void Awake() {
        currentMenu = myMenu; 
    }
    public void Start() {
        showMenu(currentMenu); 
    }

    public void showMenu(introScene menu) {
        if (currentMenu != null)
            currentMenu.isOpen = false;

        currentMenu = menu;
        currentMenu.isOpen = true; 
    }
}
