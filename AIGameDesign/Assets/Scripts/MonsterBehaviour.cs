using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehaviour : MonoBehaviour
{
   Animator anim;
    NavMeshAgent agent;
    Renderer rend;
  
    public Material green;
    public Material red;
    public Material yellow;

    //the distance at which it will choose a new position to go to.
    //maximum distance from player to calculate new distance
    public float recalculationDistance;
    public float walkRadius;
    public float idleDelay;

    [Space]
    public float currentClues;
    public float alertRange; // the distance to the monster at which the monster gets alert (follows)
    public float protectiveRange; // the distance the player gets to a clue before the monster gets aggresive (follows fast)

    [Space]
    public float normalSpeed;
    public float protectiveSpeed;

    [Space]
    public Transform player; // the place relative the player that the monster will follow.

    [Header("Read Only")]
    [SerializeField] float distanceToTarget;
    [SerializeField] float distanceToPlayer;

    [Space]
    public bool isAlert;
    public bool isFollowing;
    public bool isProtective;
    public bool shouldWalk;
    public bool shouldRun;
    private bool hasReachedTarget;
    private bool isIdling;

    Vector3 targetPos;
    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    public void Initialise()
    {
      //  agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        //anim = GetComponent<Animator>();

        targetPos = transform.position;

        InvokeRepeating("checkIfReachedTarget", 0, 0.1f);
    }

    private void OnAnimatorMove()
    {
        transform.position = agent.nextPosition;    
    }
    void checkIfReachedTarget()
    {
        anim.SetBool("walk", shouldWalk);
        anim.SetBool("Run", shouldRun);
        distanceToTarget = Vector3.Distance(transform.position, targetPos);
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //if you are close enough to player, turn on alert (slowly following). this only works at more than 4 clues  and less than 8.
        //at more than 8 enemy AI is always in alert
        if (currentClues >= 4)//if more than 4 clues
        {
            if (currentClues < 8)// but less than 8
            {
                if (distanceToPlayer < alertRange)// and is within range
                {
                    isAlert = true;
                }
                else // and isnt within range
                {
                    isAlert = false;
                }
            }
            else // is more than 4 clues and is more than or equal to 8 (always alert)
            {
                isAlert = true;
            }
        }

        if(currentClues>=10)
        {
            Destroy(gameObject);
        }
        if (isAlert && isProtective)
        {
            rend.material = red;
        }
        else if (isAlert)
        {
            rend.material = yellow;
        }
        else
        {
            rend.material = green;
        }


        if (isAlert && isProtective)
        {
            if(!isIdling)
            {
                shouldRun = true;
                shouldWalk = false;
            }
         
            agent.speed = protectiveSpeed;
        }
        else
        {
            if(!isIdling)
            {
                shouldRun = false;
                shouldWalk = true;
            }
           
            agent.speed = normalSpeed;
        }

        //if you are alert (so chasing player) it should just constantly update to player pos
        if (isAlert)
        {
            setNewPosition();
            return; // dont go any further in this method
        }

        if (distanceToTarget < recalculationDistance && !hasReachedTarget)
        {
            StartCoroutine(waitThenSetPos());
            hasReachedTarget = true;
        }
    }

    IEnumerator waitThenSetPos()
    {
        shouldWalk = false;
        shouldRun = false;

        isIdling = true;
        yield return new WaitForSecondsRealtime(idleDelay);

        setNewPosition();
        isIdling = false;
    }

    void setNewPosition()
    {
        targetPos = calculatePos();
        agent.destination = targetPos;
      //  shouldWalk = true;
        hasReachedTarget = false;
    }


    public Vector3 calculatePos()
    {
        if (isFollowing) // if monster is following, then random idle relative to player.
        {
            if (isAlert)//however if its alert it will simply follow player
            {
                return player.position;
            }

            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += player.position;//player pos

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);

            return hit.position;
        }
        else// if not following but is alert, then just go to the player. If is not following and isnt alert, then randomly idle around.
        {
            if (isAlert)
            {
                return player.position;
            }

            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection += transform.position;//monster pos

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);

            return hit.position;
        }
    }
}
