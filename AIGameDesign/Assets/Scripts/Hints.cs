using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//This script goes on the player.
//Dependencies; An object tagged as "HintGiver" that can bump into this one, a UI element to display the hint.
public class Hints : MonoBehaviour
{
    [SerializeField] private GameObject[] activeClues;

    public Text UItext;

    public bool hintActive = false;

    public GameObject popupPanel;
    public Text popupText;
    public float popupTime = 3;
    private float timer = 0;
    private bool timerRun = false;

    private bool firstTalk = true;

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
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "HintGiver" && hintActive == false)
        {
            if (!firstTalk)
            {
                activeClues = GameObject.FindGameObjectsWithTag("Object");
                int hint = Random.Range(0, activeClues.Length);

                if (!hintActive)
                {
                    UItext.GetComponent<Animator>().Play("HintEnter");
                }
                else
                {
                    UItext.GetComponent<Animator>().Play("Hint Text");
                }

                UItext.text = "Try " + activeClues[hint].gameObject.name;

                popupPanel.SetActive(true);
                popupText.text = "Try " + activeClues[hint].gameObject.name;
                timerRun = true;

                hintActive = true;
            }
            else if (firstTalk)
            {
                popupPanel.SetActive(true);
                popupText.text =
                    "Hey partner. Ready to work on this case? Talk to me if you need any ideas.";
                timerRun = true;
                firstTalk = false;
            }
        }
    }
}
