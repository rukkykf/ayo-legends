using UnityEngine;
using UnityEngine.Networking; 
using System.Collections;

public class AyoPlayerMotionScript_MultiPlayer : NetworkBehaviour {

    //Resting position values for the player hands
    public float        restingXVal;
    public float        restingYVal;
    public float        restingZVal;
    public bool         beginMotion;
    public Vector3      targetPosition;
    public float        speed;
    public Vector3      restingPosition; 

    void Awake() {

        //set the values of restingPosition
        restingPosition.x = restingXVal;
        restingPosition.y = restingYVal;
        restingPosition.z = restingZVal;

        speed = 4.0f;
        transform.position = restingPosition;
    }

    [ClientRpc]
    public void RpcStartInitialHandMotion() {
        StartCoroutine(initialHandMotion()); 
    }

    [ClientRpc]
    public void RpcStartPlayerTurnMotion(int holeIndex) {
        StartCoroutine(playerTurnMotion(holeIndex)); 
    }

   
    public IEnumerator playerTurnMotion(int holeIndex) {

        //Move the hand from the resting position to the initial hole position where the hand gets to pick up the seeds
        Vector3 pos;
        int numMovements = AyoBoard_MultiPlayer.S.holes[holeIndex].numSeeds;  //number of steps the hand will move after picking up seeds from this hole. 

        pos = holePosToHandPos(holeIndex);
        yield return StartCoroutine(moveToPosition(pos));
        yield return StartCoroutine(handFoldMotion());
        for(int k = 0; k < numMovements; k++)
            AyoBoard_MultiPlayer.S.holes[holeIndex].removeOneSeed();

        int motionHoleIndex = holeIndex;  //the next hole the hand is supposed to move to

        for (int i = 0; i < numMovements; i++) {
            motionHoleIndex = AyoBoard_MultiPlayer.S.getNextHole(motionHoleIndex);

            if (motionHoleIndex == holeIndex)
                continue; 

            pos = holePosToHandPos(motionHoleIndex);
            yield return StartCoroutine(moveToPosition(pos));

            yield return StartCoroutine(handFoldMotion());
            AyoBoard_MultiPlayer.S.holes[motionHoleIndex].addOneSeed();

        }

        //check for valid captures. 
        bool captureChainOn = true;

        while (captureChainOn) {

            if (AyoBoard_MultiPlayer.S.holes[motionHoleIndex].numSeeds > 1 && AyoBoard_MultiPlayer.S.holes[motionHoleIndex].numSeeds <= 3 && AyoBoard_MultiPlayer.S.holes[motionHoleIndex].playerID != AyoBoard_MultiPlayer.S.currentPlayer) {
                AyoBoard_MultiPlayer.S.networkedPlayers[AyoBoard_MultiPlayer.S.currentPlayer].GetComponent<multiPlayer>().captureSeeds(AyoBoard_MultiPlayer.S.holes[motionHoleIndex].numSeeds);
                pos = holePosToHandPos(motionHoleIndex);
                yield return StartCoroutine(moveToPosition(pos));
                yield return StartCoroutine(handFoldMotion());

                for (int y = 0; y < numMovements; y++)
                    AyoBoard_MultiPlayer.S.holes[holeIndex].removeOneSeed();

                motionHoleIndex = AyoBoard_MultiPlayer.S.getPreviousHole(motionHoleIndex);
            }
            else {
                captureChainOn = false;
            }
        }

        yield return moveToPosition(restingPosition);
        AyoBoard_MultiPlayer.S.networkedPlayers[AyoBoard_MultiPlayer.S.currentPlayer].GetComponent<multiPlayer>().isTurn = false;
        yield break;
        
    }

    void Update() {
        if (beginMotion == true) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, step);
        }
    }

    
    public IEnumerator moveToPosition(Vector3 pos) {
        while (true) {
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
        Vector3 pos = AyoBoard_MultiPlayer.S.holes[holeIndex].transform.position;

        //retain the Y value. 
        pos.y = transform.position.y;
        return pos;
    }

   
    public IEnumerator initialHandMotion() {
        Vector3 pos;
        for (int j = 0; j < 4; j++) {

            for (int i = 0; i < 12; i++) {
                pos = holePosToHandPos(AyoBoard_MultiPlayer.S.holes[i].holeIndex);
                yield return StartCoroutine(moveToPosition(pos));
                yield return StartCoroutine(handFoldMotion());
                AyoBoard_MultiPlayer.S.holes[i].addOneSeed();
            }
        }
        yield return moveToPosition(restingPosition);
        AyoBoard_MultiPlayer.S.startGame = true; 
        yield break;
    }

 
    private IEnumerator handFoldMotion() {
        yield return new WaitForSeconds(.18f);
        transform.Find("player").gameObject.GetComponent<HandPhysicsController>().StartBendFingers();
        yield return new WaitForSeconds(.18f);
        transform.Find("player").gameObject.GetComponent<HandPhysicsController>().StopBendFingers();
        yield return new WaitForSeconds(.18f);
        yield break;
    }

}
