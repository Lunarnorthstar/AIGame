using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!hintActive)
        {
            UItext.text = "No hint right now";
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "HintGiver" && hintActive == false)
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

            hintActive = true;
        }
    }
}
