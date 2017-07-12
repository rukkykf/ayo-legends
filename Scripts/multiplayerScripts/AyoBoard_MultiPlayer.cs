using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking; 

public class AyoBoard_MultiPlayer : NetworkBehaviour {
    //singleton for the board
    static public AyoBoard_MultiPlayer S;

    public List<GameObject> networkedPlayers;
    public List<AyoPlayerMotionScript_MultiPlayer> playerHands; 
    public int currentPlayer;
    public List<AyoHole_Multiplayer> holes;
    public bool isGameOn;
    public bool isPaused;
    public bool startGame; 
    public Canvas pauseMenu;
    public bool playersActivated;
    public List<AyoHole_Multiplayer> capturedSeedsDumps; 

    void Awake() {
        S = this;
        //initialize the holes for the two players. first 6 holes for player one and last 6 holes for player two. 

        for (int i = 0; i < 6; i++) {
            holes[i].playerID = 0;
            holes[i].holeIndex = i;
        }

        for (int i = 6; i < 12; i++) {
            holes[i].playerID = 1;
            holes[i].holeIndex = i;

        }
    }

    void Start() {
        isGameOn = false;
        startGame = false;
        playersActivated = false; 
    }

    void Update() {
        if(NetworkManager.singleton.numPlayers == 2 && isGameOn == false) {
            beginGame();
            isGameOn = true; 
        }

        if(NetworkManager.singleton.numPlayers < 2 && isGameOn == true) {
            playersActivated = false;
            isGameOn = false;
            return; 
        }

        if(playersActivated == true) {

            if(networkedPlayers[currentPlayer].GetComponent<multiPlayer>().isTurn == false) {
                currentPlayer = getNextPlayer();
                networkedPlayers[currentPlayer].GetComponent<multiPlayer>().isTurn = true; 
            }
        }
        
    }

    public void beginGame() {
        networkedPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("networkedPlayer"));

        //setup both players and choose the current player. 
        networkedPlayers[0].GetComponent<multiPlayer>().playerID = 0;
        networkedPlayers[0].GetComponent<multiPlayer>().capturedSeedDump = capturedSeedsDumps[0]; 

        networkedPlayers[1].GetComponent<multiPlayer>().playerID = 1;
        networkedPlayers[1].GetComponent<multiPlayer>().capturedSeedDump = capturedSeedsDumps[1]; 
        currentPlayer = 1;

        networkedPlayers[currentPlayer].GetComponent<multiPlayer>().isTurn = true;
        playersActivated = true;

        playerHands[0].RpcStartInitialHandMotion(); 
    }

    
    
    public void startPlayerMotion(int playerID, int holeIndex) {
        playerHands[playerID].RpcStartPlayerTurnMotion(holeIndex); 
    }

    
    public int getNextPlayer() {
        if (currentPlayer == 0)
            return 1;
        else
            return 0;
    }

    public int getNextHole(int holeNum) {
        int newHoleNum = holeNum + 1;
        if (newHoleNum == 12) {
            newHoleNum = 0;
        }

        return newHoleNum;
    }

    public int getPreviousHole(int holeNum) {
        int newHoleNum = holeNum - 1;
        if (newHoleNum == -1) {
            newHoleNum = 11;
        }
        return newHoleNum;
    }

}
