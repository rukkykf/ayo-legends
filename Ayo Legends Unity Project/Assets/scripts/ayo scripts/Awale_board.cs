using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

//this class represents the gameboard and game controller
public class Awale_board : MonoBehaviour {

    //singleton for the Awale_board
    static public Awale_board S;

    public List<Player>     awalePlayers;
    public int              currentPlayer; 
    public List<Awale_Hole> holes;
    public bool             isGameOn;
    public bool             isPaused;
    public Canvas           pauseMenu;
    public Canvas           infoSession;
    public bool             isAiTurn;
    public bool             isStoryMode; 

    void Awake(){
        S = this;
        //initialize the holes for the two players. first 6 holes for player one and last 6 holes for player two. 

        for (int i = 0; i < 6; i++){
            holes[i].playerID = 0;
            holes[i].holeIndex = i; 
        }

        for (int i = 6; i < 12; i++){
            holes[i].playerID = 1;
            holes[i].holeIndex = i; 

        }
        isGameOn = false;
        pauseMenu.enabled = false;
        infoSession.enabled = false; 
        isPaused = false;
        isAiTurn = false; 

        //determine the level of play and if this is storyMode
        
        if(PlayerPrefs.GetString("mode") == "story") {
            isStoryMode = true; 
        }
        else {
            isStoryMode = false; 
        }
    }
	// Use this for initialization
	void Start () {

     

        //let the players know their IDs
        awalePlayers[0].playerID = 0;
        awalePlayers[1].playerID = 1;

        //start the player turn for player one
        awalePlayers[0].isTurn = true;
        awalePlayers[1].isTurn = false;

        //set the current player to the first one. 
        currentPlayer = 0;

        //let each player know the indexes of the holes it controls. 
        for(int i = 0; i < 6; i++) {
            awalePlayers[0].playerHoles[i] = i; 
        }
        for(int i = 0; i < 6; i++) {
            awalePlayers[1].playerHoles[i] = i + 6;
        }

        //load board with seeds
        string textInfo = "";
        if (isStoryMode)
            textInfo = "You are playing first. Click a hole containing seeds, from your side of the board to make a move. Your side of the board is the lower bottom of the Ayo board. \nThe game will begin after the board is completely filled";
        else
            textInfo = "Player 1 is playing first. Player 1 is the player at the bottom of the screen. Click a hole on your side of the Ayo board to play a move.\nThe game will begin after the board is completely filled"; 

        startInfoSession(textInfo, "ok");
        StartCoroutine(awalePlayers[currentPlayer].GetComponent<motionScriptNewBehaviourScript>().initialHandMotion());
        
        
    }

    // Update is called once per frame
    void Update () {

        //check if the current player's turn is over and update the player. 
	    if(awalePlayers[currentPlayer].isTurn == false) {
            currentPlayer = getNextPlayer(); 
            awalePlayers[currentPlayer].isTurn = true;
            isAiTurn = false;

            if (awalePlayers[currentPlayer].getTotalNumSeeds() == 0) {
                startInfoSession("Player " + (currentPlayer + 1).ToString() + " has forfeited their turn since they have no seeds.", "ok");
                awalePlayers[currentPlayer].isTurn = false;
                currentPlayer = getNextPlayer();
                awalePlayers[currentPlayer].isTurn = true;
            }

           if (currentPlayer == 1 && isStoryMode == true) {
                isAiTurn = true;
                awalePlayers[currentPlayer].play();
            }
        }


        //check if the game is over. 


        //Check if the escape key is pressed to pause the game. 
        if(Input.GetKeyDown(KeyCode.Escape) && isPaused == false) {
            Time.timeScale = 0; 
            pauseMenu.enabled = true;
            isPaused = true; 
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isPaused == true) {
            Time.timeScale = 1;
            pauseMenu.enabled = false;
            isPaused = false; 
        }

        checkIfGameOver();

   }

    public int getNextPlayer() {
        if (currentPlayer == 0)
            return 1;
        else
            return 0; 
    }

    public int getNextPlayer(int a) {
        if (a == 0)
            return 1;
        else
            return 0; 
        
    }

    //returns the index of the next hole on the board. 
    public int getNextHole(int holeNum) {
        int newHoleNum = holeNum + 1;
        if (newHoleNum == 12) {
            newHoleNum = 0;
        }

        return newHoleNum;
    }
    
    public int getPreviousHole(int holeNum) {
        int newHoleNum = holeNum - 1; 
        if(newHoleNum == -1) {
            newHoleNum = 11; 
        }
        return newHoleNum; 
    }


    //pause menu controls
    public void backToMainMenu() {
        SceneManager.LoadScene("level_0.1"); 
    }

    public void resumeGame() {
        Time.timeScale = 1;
        pauseMenu.enabled = false; 
    }

    public void exitGame() {
        Application.Quit(); 
    }

    public void restartGame() {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    //game over and information controls
    public void checkIfGameOver() {
        for(int i = 0; i < 2; i++) {
            if(awalePlayers[i].capturedSeedsDump.numSeeds > 24) {
                string textInfo = "<size=25>GAME OVER</size> \nPlayer " + (i + 1).ToString() + " has captured more than 24 seeds";
                

                if(isStoryMode) {
                    int currentLevel = PlayerPrefs.GetInt("currentLevel");
                    int level = PlayerPrefs.GetInt("Level"); 

                    //if the human player won this level. 
                    if(i == 0) {
                        //check if the currentLevel is the same as the overall level of the player.
                        if(currentLevel == level && level < 4) {
                            level += 1;
                            PlayerPrefs.SetInt("Level", level); 
                        }

                        textInfo = "<size=25>GAME OVER</size> \nYou have captured more than 24 seeds and as winner, you are now in level " + level.ToString() + "To proceed to the next level go to the main menu, select story mode and play new level";
                    }
                    else if(i == 1){
                        //if the human player didn't win.
                        textInfo = "<size=25>GAME OVER</size> \nYou lost this game ): \nPlayer 2 has captured more than 24 seeds...that's more than 50% of the 48 seeds on the board and so she wins";
                    }
                }

                startInfoSession(textInfo, "gameOver"); 
                break; 
            }
        }

        //in the event that both players have captured 24 seeds each (very unlikely btw)
        if(awalePlayers[0].capturedSeedsDump.numSeeds == 24 && awalePlayers[1].capturedSeedsDump.numSeeds == 24) {
            startInfoSession("This is a stalemate, so the game is over and there is no winner", "gameOver"); 
        }
    }

    public void startInfoSession(string text, string sessionMode) {
        infoSession.enabled = true; 
        infoSession.gameObject.transform.Find("Panel/Text").GetComponent<Text>().text =  text;
        
        switch (sessionMode){
            case "gameOver":
                infoSession.gameObject.transform.Find("Panel/Buttons/Restart").gameObject.SetActive(true);
                infoSession.gameObject.transform.Find("Panel/Buttons/Back To Main Menu").gameObject.SetActive(true);
                infoSession.gameObject.transform.Find("Panel/Buttons/Ok").gameObject.SetActive(false);
                break;
            case "ok":
                infoSession.gameObject.transform.Find("Panel/Buttons/Ok").gameObject.SetActive(true); 
                break; 
        }

        Time.timeScale = 0;
    }

    public void endInfoSession() {
        Time.timeScale = 1;
        infoSession.enabled = false;
        infoSession.gameObject.transform.Find("Panel/Buttons/Restart").gameObject.SetActive(false);
        infoSession.gameObject.transform.Find("Panel/Buttons/Back To Main Menu").gameObject.SetActive(false);
        infoSession.gameObject.transform.Find("Panel/Buttons/Ok").gameObject.SetActive(false);
    }

    public void restartGameFromInfoSession() {
        endInfoSession();
        restartGame(); 
    }

    public void backToMainMenuFromInfoSession() {
        endInfoSession();
        backToMainMenu(); 
    }
}

