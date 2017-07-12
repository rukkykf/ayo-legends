using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic; 

public class AyoHole_Multiplayer : NetworkBehaviour {

    public GameObject seedPrefab;
    public GameObject seedsParent;
    public Text showNumSeeds;
    public List<GameObject> seeds;
    public int playerID;
    public int holeIndex;
    public int numSeeds;



    void Awake() {
        numSeeds = 0;
        GameObject uiText = transform.Find("Canvas/Text").gameObject;
        showNumSeeds = uiText.GetComponent<Text>();
        showNumSeeds.text = numSeeds.ToString();
    }


    void OnMouseDown() {

                
    }

    [Client]
    public void addOneSeed() {
        
        GameObject newSeed = Instantiate(seedPrefab, transform.position, transform.rotation) as GameObject;
        newSeed.transform.SetParent(seedsParent.transform);
        seeds.Add(newSeed);
        numSeeds += 1;
        showNumSeeds.text = numSeeds.ToString();


    }


    
    [Client]
    public void removeAllSeeds() {
        while (numSeeds > 0) {
            removeOneSeed();
        }
    }

    [Client]
    public void removeOneSeed() {
        if (seeds.Capacity == 0)
            return;

        Destroy(seeds[0]);
        seeds.RemoveAt(0);


        numSeeds -= 1;
        showNumSeeds.text = numSeeds.ToString();
        return;
    }

}
