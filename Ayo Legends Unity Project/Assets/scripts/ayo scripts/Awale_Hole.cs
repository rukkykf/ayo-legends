using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;   

public class Awale_Hole : MonoBehaviour {

    public GameObject         seedPrefab;
    public GameObject         seedsParent;
    public Text               showNumSeeds;
    public List<GameObject>   seeds;
    public int                playerID;
    public int                holeIndex;
    public int                numSeeds;
    
    

    void Awake(){
        numSeeds = 0;
        GameObject uiText = transform.Find("Canvas/Text").gameObject;
        showNumSeeds = uiText.GetComponent<Text>();
        showNumSeeds.text = numSeeds.ToString();
    }
   
    void OnMouseDown(){
        //Validate the click on this hole. 
        if (this.playerID == Awale_board.S.currentPlayer && numSeeds > 0 && Awale_board.S.isAiTurn == false)
            Awale_board.S.awalePlayers[playerID].play(holeIndex);
        else
            return; 
    }

    public void addOneSeed(){
        GameObject newSeed = Instantiate(seedPrefab, transform.position, transform.rotation) as GameObject;
        newSeed.transform.SetParent(seedsParent.transform); 
        seeds.Add(newSeed); 
        numSeeds += 1;
        showNumSeeds.text = numSeeds.ToString();

        
    }

    public void removeAllSeeds() {
        while(numSeeds > 0) {
            removeOneSeed(); 
        }
    }

    public int removeOneSeed() {
        if (seeds.Capacity == 0)
            return numSeeds; 

        Destroy(seeds[0]); 
        seeds.RemoveAt(0); 
        
               
        numSeeds -= 1;
        showNumSeeds.text = numSeeds.ToString(); 
        return numSeeds; 
    }

}
