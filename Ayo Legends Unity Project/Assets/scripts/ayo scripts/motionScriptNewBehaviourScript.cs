using UnityEngine;
using System.Collections;
using System.Collections.Generic; 


public class motionScriptNewBehaviourScript : MonoBehaviour {

    //Resting position values for the player hands
    public float         restingXVal;
    public float         restingYVal;
    public float         restingZVal;
    public float         speed;
    public bool          beginMotion; 
    private Vector3      restingPosition;
    private Vector3      targetPosition;  //where is the hand moving to next? 

    void Awake() {

        //set the values of restingPosition
        restingPosition.x = restingXVal;
        restingPosition.y = restingYVal;
        restingPosition.z = restingZVal;

        speed = 4.0f;
        transform.position = restingPosition;

    }

    // Use this for initialization
    void Start() {

        beginMotion = false;

    }
	
	// Update is called once per frame
	void Update () {
        if(beginMotion == true) { 
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, step);
        }
    }

    private IEnumerator moveToPosition(Vector3 pos) {
        while(true) {
            beginMotion = true; 
            targetPosition = pos; 

                if (Vector3.Equals(transform.position, targetPosition)) {
                   
                    beginMotion = false; 
                    yield break;
                }
                else
                    yield return null; 
        }
    }


    public Vector3 holePosToHandPos(int holeIndex) {
        Vector3 pos = Awale_board.S.holes[holeIndex].transform.position;

        //retain the Y value. 
        pos.y = transform.position.y;
        return pos; 
    }

    //this is called at the start of the game to get fill up the holes with 4 seeds each
    public IEnumerator initialHandMotion() {
       Vector3 pos;
       for(int j = 0; j < 4; j++) {

           for(int i = 0; i < 12; i++) {
                pos = holePosToHandPos(Awale_board.S.holes[i].holeIndex);
                yield return StartCoroutine(moveToPosition(pos));
                yield return StartCoroutine(handFoldMotion()); 
                Awale_board.S.holes[i].addOneSeed(); 
          }
       }
        yield return moveToPosition(restingPosition);
        Awale_board.S.isGameOn = true; 
        yield break; 
    }

    public IEnumerator playerTurnMotion(int holeIndex) {

        //Move the hand from the resting position to the initial hole position where the hand gets to pick up the seeds
        Vector3 pos;
        int numMovements = Awale_board.S.holes[holeIndex].numSeeds;  //number of steps the hand will move after picking up seeds from this hole. 

        pos = holePosToHandPos(holeIndex);
        yield return StartCoroutine(moveToPosition(pos));
        yield return StartCoroutine(handFoldMotion()); 
        Awale_board.S.holes[holeIndex].removeAllSeeds();

        int motionHoleIndex = holeIndex;  //the next hole the hand is supposed to move to

        for(int i = 0; i < numMovements; i++) {
            motionHoleIndex = Awale_board.S.getNextHole(motionHoleIndex);

            //if the hand goes round the board skip over the original hole
            if (motionHoleIndex == holeIndex) {
                i -= 1; 
                continue;
            }

            pos = holePosToHandPos(motionHoleIndex);
            yield return StartCoroutine(moveToPosition(pos));

            yield return StartCoroutine(handFoldMotion()); 
            Awale_board.S.holes[motionHoleIndex].addOneSeed(); 
           
        }

        //check for valid captures. 
        bool captureChainOn = true; 

        while(captureChainOn) {

            if(Awale_board.S.holes[motionHoleIndex].numSeeds > 1 && Awale_board.S.holes[motionHoleIndex].numSeeds <= 3 && Awale_board.S.holes[motionHoleIndex].playerID != Awale_board.S.currentPlayer) {
                Awale_board.S.awalePlayers[Awale_board.S.currentPlayer].captureSeeds(Awale_board.S.holes[motionHoleIndex].numSeeds);
                pos = holePosToHandPos(motionHoleIndex); 
                yield return StartCoroutine(moveToPosition(pos));
                yield return StartCoroutine(handFoldMotion()); 
               
                Awale_board.S.holes[motionHoleIndex].removeAllSeeds();
                motionHoleIndex = Awale_board.S.getPreviousHole(motionHoleIndex); 
            }
            else {
                captureChainOn = false; 
            }
        }

        yield return moveToPosition(restingPosition);
        Awale_board.S.awalePlayers[Awale_board.S.currentPlayer].isTurn = false; 
        yield break; 
    }

    //simulates the motion of the hand when capturing or dropping seeds. 
    private IEnumerator handFoldMotion() {
        yield return new WaitForSeconds(.18f);
        transform.Find("player").gameObject.GetComponent<HandPhysicsController>().StartBendFingers();
        yield return new WaitForSeconds(.18f);
        transform.Find("player").gameObject.GetComponent<HandPhysicsController>().StopBendFingers();
        yield return new WaitForSeconds(.18f);
        yield break; 
    }
}
