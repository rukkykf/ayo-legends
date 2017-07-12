using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class AIPlayer : Player{

    private List<int> actualGameState; 

    void Start() {
        actualGameState = new List<int>(); 
    }

	override public void play() {
        int holeIndex = 0;


        int level = PlayerPrefs.GetInt("currentLevel");
        if (level == 1) {
            holeIndex = levelOne();
        }
        else if (level == 2) {
            holeIndex = levelTwo();
        }
        else if (level == 3) {
            holeIndex = levelThree();
        }
        base.play(holeIndex);
    }


    public int levelOne() {
        bool keepSearching = true;
        int potentialHole = 0;
        while(keepSearching) { 
            potentialHole = Random.Range(playerHoles[0], playerHoles[5] + 1); //max is exclusive hence the need to add one. 
            if (Awale_board.S.holes[potentialHole].numSeeds == 0) {
                keepSearching = true;
            }
            else {
                keepSearching = false; 
            }

            //if the AI player has no seeds to play
            if(getTotalNumSeeds() == 0) {
                keepSearching = false;
                potentialHole = playerHoles[0]; 
            }
        }
        return potentialHole; 
    }

    //this is similar to a bruteforce search or rather a minimax tree with a depth of one because it just obtains the move with the largest possible gain
    //For this player. 

    public int levelTwo() {
        cloneCurrentGameState(); 

        int potentialCaptured = 0;
        int potentialHole = playerHoles[0]; 

        for (int i = 0; i < 6; i++) {
            int capture = utility(actualGameState, playerHoles[i], 1);
            if (capture >  potentialCaptured) {
                potentialHole = playerHoles[i];
                potentialCaptured = capture; 
            }
        }

        if (potentialCaptured == 0)
            return levelOne();
        else
            return potentialHole; 
    }

    //this is a minimax search with a depth of two. 
    //alpha beta pruning may be applied later to improve performance. 
   //refactoring needs to be done to ensure that this code is easier to manage and possibly, reuse in the future for other board games
    public int levelThree() {
        //return if the player has no seeds to play
      
        if (getTotalNumSeeds() == 0)
            return playerHoles[0];

        cloneCurrentGameState();

        //it's AI player's turn now. 
        List<List<int>> listOfFirstLevelStates = new List<List<int>>();
        List<int> firstLevelCaptureValues = new List<int>();
        List<int> secondLevelMaximumCaptureValues = new List<int>();
        List<int> potentialHoles = new List<int>(); 
        for(int i = 0; i < 6; i++) {

            //this ensures that successors are only generated for holes that actually contain seeds
            if (actualGameState[playerHoles[i]] == 0)
                continue; 

            listOfFirstLevelStates.Add(successor(actualGameState, playerHoles[i], 1));
            firstLevelCaptureValues.Add(utility(actualGameState, playerHoles[i], 1));
            potentialHoles.Add(playerHoles[i]); 
        }

        //if the human player has no seeds to play discontinue the algorithm 
        if (Awale_board.S.awalePlayers[0].getTotalNumSeeds() == 0)
            return levelTwo();

        //This section of the algorithm generates gameplay states as a result of the human player acting on each of the states in the first level of the search tree. 


        int[] humanPlayerHoles = Awale_board.S.awalePlayers[0].playerHoles; 

        for(int i = 0; i < listOfFirstLevelStates.Count; i++) {

            //generate potential capture values for the human player on this game state and store these values in a list. 
            List<int> humanUtilityValues = new List<int>();
            int maximumHumanUtility = 0; 
            for(int b = 0; b < 6; b++) {
                if (listOfFirstLevelStates[i][humanPlayerHoles[b]] == 0)
                    continue;

                humanUtilityValues.Add(utility(listOfFirstLevelStates[i], humanPlayerHoles[b], 0)); 
            }

            //find the maximum utility in humanUtilityValues and add it to the second level capture values.
            if (humanUtilityValues.Count == 0) {
                maximumHumanUtility = 0;
                secondLevelMaximumCaptureValues.Add(maximumHumanUtility); 
                continue;
            } 
            for(int a = 0; a < humanUtilityValues.Count; a++) {
                if(humanUtilityValues[a] > maximumHumanUtility) {
                    maximumHumanUtility = humanUtilityValues[a]; 
                }
            }
            secondLevelMaximumCaptureValues.Add(maximumHumanUtility);

        }


        //obtain a value that represents the maximum gain that can be made by the AIPlayer 2 steps into the forseeable future by subtracting potential losses from potential games for each move the AIplayer could possibly make

        List<int> actualUtilityValues = new List<int>(); 
        for(int x = 0; x < potentialHoles.Count; x++) {
            actualUtilityValues.Add(firstLevelCaptureValues[x] - secondLevelMaximumCaptureValues[x]); 
        }

        //Find the maximum of all the utility values and keep track of the corresponding hole that results in this maximum value. 
        int maximumAIUtility = actualUtilityValues[0];
        int correspondingHole = potentialHoles[0]; 

        for(int x = 0; x < potentialHoles.Count; x++) {
            if(actualUtilityValues[x] > maximumAIUtility) {
                maximumAIUtility = actualUtilityValues[x];
                correspondingHole = potentialHoles[x]; 
            }
        }

        return correspondingHole; 

    }

    //minimax utility functions.
    public void cloneCurrentGameState() {
        actualGameState.Clear(); 

        for (int i = 0; i < 12; i++) {
            actualGameState.Add(Awale_board.S.holes[i].numSeeds);

        }

    }


    //obtain utility for a move in a particular state, for a particular action and a particular player utility(s, a, p)
    //this essentially returns the number of seeds that could be captured by this player, after choosing a particular hole (action) in the given state of the board
    public int utility(List<int> nonLocalState, int holeIndex, int player) {

        if(!isPlayerHole(player, holeIndex)) {
            return -1; 
        }

        List<int> state = new List<int>(); 
        for(int i = 0; i < 12; i++) {
            state.Add(nonLocalState[i]); 
        }

        int capturedSeeds = 0; 


        int numMovements = state[holeIndex];
        int point = holeIndex;

      
        for (int i = 0; i < numMovements; i++) {
            point = Awale_board.S.getNextHole(point);
            state[point] += 1; 
        }

        bool captureChainOn = true;
        int nextPlayer = 0;

        nextPlayer = Awale_board.S.getNextPlayer(player); 

        while(captureChainOn) {

            if(state[point] > 1 && state[point] <= 3 && isPlayerHole(nextPlayer, point)) {
                capturedSeeds += state[point];
                point = Awale_board.S.getPreviousHole(point); 
            }
            else {
                captureChainOn = false; 
            }
        }

        return capturedSeeds; 
    }

    //return the resulting state from a move by a player on the current board state.
    public List<int> successor(List<int> nonLocalState, int holeIndex, int player) {
        if (!isPlayerHole(player, holeIndex)) {
            return null;
        }

        //duplicate the list state
        List<int> state = new List<int>();
        for (int i = 0; i < 12; i++) {
            state.Add(nonLocalState[i]);
        }

        int capturedSeeds = 0;


        int numMovements = state[holeIndex];
        int point = holeIndex;
        state[point] = 0; 

        for (int i = 0; i < numMovements; i++) {
            point = Awale_board.S.getNextHole(point);
            state[point] += 1;
        }

        bool captureChainOn = true;
        int nextPlayer = 0;

        nextPlayer = Awale_board.S.getNextPlayer(player);

        while (captureChainOn) {

            if (state[point] > 1 && state[point] <= 3 && isPlayerHole(nextPlayer, point)) {
                capturedSeeds += state[point];
                state[point] = 0; 
                point = Awale_board.S.getPreviousHole(point);
            }
            else {
                captureChainOn = false;
            }
        }

        return state; 
    }

    public bool isPlayerHole(int playerID, int holeIndex) {
        if (playerID == 0) {
            return (holeIndex >= 0 && holeIndex < 6);
        }
        else if (playerID == 1) {
            return (holeIndex >= 6 && holeIndex < 12);
        }
        else
            return false; 
    }

}
