using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//This script goes on the player object. It handles the collection, spawning, and loss of objects on contact.
//Dependencies; Player: a trigger collider to make contact with the target objects. Target Objects: A non-trigger collider to activate the trigger. Hint Giver: the "HintGiver" tag.
public class PlayerPickup : MonoBehaviour
{
    [Tooltip("The amount of objects the player has - no touching!")]
    public static int objects = 0;
    [Tooltip("The amount of objects the player needs to have before triggering the final one")]
    public int totalObjects = 9;
    [Tooltip("The amount of clues that can be collected before the hint giver despawns")]
    public int hintGiverDespawnThreshold = 5;

    [Tooltip("The UI element that displays the object count")]
    public Text uiText; //The UI element that displays the player's object count.

    public GameObject[] objectsToActivate;
    public GameObject finalObject; //The final, special object

    public MonsterManager monsterManager; // a reference to the monster manager script that determines how the monster will behave with this amount of clues

    bool hasDespawnedHintGiver;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < totalObjects; i++) //Run the objectPicker function an amount of times equal to the max objects
        {
            objectPicker();
        }

        InvokeRepeating("checkDistanceToClues", 0, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        uiText.text = "Objects:" + objects; //Display current object count on UI
    }

    void checkDistanceToClues()//checks distance to clues (not done in update for performance reasons)
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (Vector3.Distance(transform.position, objectsToActivate[i].transform.position) < monsterManager.MinDistanceToClue)
            {
                monsterManager.toggleProtective(true);
                return;
            }
            else
            {
                monsterManager.toggleProtective(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other) //When the player bumps into something...
    {
        if (other.tag == "Object") //If it's an object to be picked up...
        {
            objects++; //Increment the object count
            gameObject.GetComponent<Hints>().hintActive = false;
            other.gameObject.SetActive(false); //Remove the object from the scene.
            uiText.GetComponent<Animator>().SetTrigger("Got Object");

            monsterManager.updateMonster(objects);//upddates the behaviour of the monster to the current amount of clues you have
        }

        if (objects >= hintGiverDespawnThreshold && !hasDespawnedHintGiver) //If you've collected enough to despawn the hint giver...
        {
            GameObject.FindWithTag("HintGiver").SetActive(false); //Despawn him.
            hasDespawnedHintGiver = true; //so that its not trying to despawn him when he's already gone (throws null reference exeption)
        }

        if (objects == totalObjects) //If you've collected all the objects...
        {
            finalObject.SetActive(true); //Activate the special, final one.
        }
    }

    public void objectPicker() //Handles the spawning and respawning of objects.
    {
        while (true)
        {
            int random = UnityEngine.Random.Range(0, objectsToActivate.Length); //Get a random object in the array...
            if (objectsToActivate[random].activeSelf == false) //If it's not already active...
            {
                objectsToActivate[random].SetActive(true); //Activate it.
                break; //Stop infinite looping.
            }
        }
    }

    public void LoseObject() //Handles the loss of objects. Called externally. Not necessary.
    {
        if (objects > 0) //If you have any objects...
        {
            objects--; //Lose one...
            objectPicker(); //And spawn a random one in the scene.
        }
    }
}
