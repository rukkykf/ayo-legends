using UnityEngine;
using UnityEngine.Networking; 
using System.Collections;

public class multiPlayer : NetworkBehaviour {
    public int playerID;
    public bool isTurn;
    public AyoHole_Multiplayer capturedSeedDump; 

    void Start() {
        isTurn = false; 
    }

   
    public void captureSeeds(int numSeeds) {
        for(int i = 0; i < numSeeds; i++) {
            capturedSeedDump.addOneSeed(); 
        }
    }

    void Update() {

        if (!isLocalPlayer)
            return; 

        if(Input.GetMouseButtonDown(0)) {

            AyoBoard_MultiPlayer.S.startPlayerMotion(playerID, playerID);            
        }
    }

    

}
