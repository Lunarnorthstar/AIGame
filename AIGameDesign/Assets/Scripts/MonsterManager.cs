using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script determines how the monster will behave with a given amount of clues
public class MonsterManager : MonoBehaviour
{
    public PlayerPickup playerPickup;
    MonsterBehaviour monsterBehaviour;

    public float MinDistanceToClue = 5; //how close player can get to clue before ai gets protective

    void Start()
    {
        playerPickup = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPickup>();
        monsterBehaviour = GetComponent<MonsterBehaviour>();
    }

    void FixedUpdate()
    {
        MinDistanceToClue = monsterBehaviour.protectiveRange;
    }

    public void updateMonster(int currentClues)
    {
        monsterBehaviour.currentClues = currentClues;

        switch (currentClues)
        {
            case 1:
                monsterBehaviour.Initialise();
                break;
            case 2:
                monsterBehaviour.isFollowing = true;
                break;
            case 3:
                monsterBehaviour.isFollowing = true;
                break;
            case 4:
                monsterBehaviour.isFollowing = false;
                break;
        }
    }

    public void toggleProtective(bool protective)
    {
        monsterBehaviour.isProtective = protective;
    }
}
