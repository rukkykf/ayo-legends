using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public int          playerID;
    public Awale_Hole   capturedSeedsDump; 
    public bool         isTurn;
    public int[]        playerHoles; 

	// Use this for initialization
	void Awake () {
        playerHoles = new int[6]; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void play(int holeIndex) {
        if(Awale_board.S.isGameOn) {
            StartCoroutine(this.GetComponent<motionScriptNewBehaviourScript>().playerTurnMotion(holeIndex));
        }
    }

    virtual public void play() { }

    public void captureSeeds(int numSeeds) {
        for(int i = 0; i < numSeeds; i++) {
            capturedSeedsDump.addOneSeed(); 
        }
    }

    public int getTotalNumSeeds() {
        int total = 0; 
        for(int i = playerHoles[0]; i <= playerHoles[5]; i++) {
            total += Awale_board.S.holes[i].numSeeds;
        }
        return total; 
    }
}
