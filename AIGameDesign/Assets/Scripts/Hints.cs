using System;
using System.Collections;
using System.Collections.Generic;
//using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//This script goes on the player.
//Dependencies; An object tagged as "HintGiver" that can bump into this one, a UI element to display the hint.
public class Hints : MonoBehaviour
{
    [SerializeField] private GameObject[] activeClues;

    public Text UItext;

    public bool hintActive = false; //Whether or not the player has a hint active. This affects their ability to get a new hint.
    public bool convopause = false;

    public GameObject interactPrompt;
    public GameObject popupPanel;
    public Text popupText;
    public float popupTime = 3; //The amount of time a popup lingers on the screen
    private float timer = 0;
    public bool timerRun = false; //Whether or not the timer should count up.

    private bool firstTalk = true; //Whether it's the first time you've talked to the hint giver.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timerRun)
        {
            timer += Time.deltaTime;
        }

        if (timer > popupTime)
        {
            timer = 0;
            timerRun = false;
            popupPanel.SetActive(false);
            convopause = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "HintGiver" && hintActive == false) //If the touching thing is the hint giver...
        {
            interactPrompt.SetActive(true);
            if (Input.GetKey(KeyCode.E) && !convopause) //When you press E...
            {
                
                if (!firstTalk) //If it's not the first time you talked to him...
                {
                    activeClues = GameObject.FindGameObjectsWithTag("Object"); //Grab all the active clues in the scene.
                    int hint = Random.Range(0, activeClues.Length); //Generate a random number in that set.

                    if (!hintActive) //If you don't have a hint active...
                    {
                        UItext.GetComponent<Animator>().Play("HintEnter");
                    }
                    else
                    {
                        UItext.GetComponent<Animator>().Play("Hint Text");
                    }

                    UItext.text = "Try " + activeClues[hint].gameObject.name; //Update the UI to display the hint, which is the name of the clue (which is the location of the clue)

                    popupPanel.SetActive(true);
                    popupText.text = "Try " + activeClues[hint].gameObject.name; //Relay the same information via a popup window
                    timerRun = true; //That only runs for a certain number of seconds...

                    hintActive = true;
                }
                else if (firstTalk) //If it's the first time you've talked to him...
                {
                    popupPanel.SetActive(true);
                    popupText.text =
                        "Hey partner. Ready to work on this case? Talk to me if you need any ideas."; //Display custom text.
                    timer = -2;
                    timerRun = true; //That runs for a certain number of seconds (plus 2 because it's longer)
                    firstTalk = false; //And it's not the first time you've talked to him anymore.
                }

                convopause = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "HintGiver") //If the touching thing is the hint giver...
        {
            interactPrompt.SetActive(false);
        }
    }
}
