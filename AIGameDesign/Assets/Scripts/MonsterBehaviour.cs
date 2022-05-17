using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;

public class MonsterBehaviour : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    Renderer rend;

    Vector3 targetPos;
    public GameObject heartBeatObject;

    //the distance at which it will choose a new position to go to.
    //maximum distance from player to calculate new distance
    public float recalculationDistance;
    public float walkRadius;
    public float idleDelay;

    [Space]
    public float currentClues;
    public float alertRange;
    public float longAlertRange; // the distance to the monster at which the monster gets alert (follows)
    public float protectiveRange; // the distance the player gets to a clue before the monster gets aggresive (follows fast)

    [Space]
    public float normalSpeed;
    public float protectiveSpeed;

    [Space]
    public Transform player; // the place relative the player that the monster will follow.
    public Transform rayCastPos;
    public farObjects[] farObjects;
    Transform currentFarObject;

    [Space]
    public MeshRenderer EyeRend1;
    public MeshRenderer EyeRend2;
    public Material IdleWanderMat;
    public Material ChaseMat;
    public Material AggresiveMat;
    //  private AudioSource source;
    // public AudioClip patrol;

    public AnimationCurve NoiseGradient;
    public PostProcessVolume processVolume;
    public Grain grain;

    [Header("Read Only")]
    [SerializeField] float distanceToTarget;
    [SerializeField] float distanceToPlayer;

    [Space]
    public bool isRunning;
    public bool isAlert;
    public bool isFollowing;
    public bool isProtective;
    public bool isAggresive;
    public bool shouldChase;

    [Space]
    public bool shouldWalk;
    public bool shouldRun;
    private bool hasReachedTarget;
    private bool isIdling;


    public void Start()
    {
        //  source = gameObject.AddComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        processVolume.profile.TryGetSettings(out grain);

        Initialise();

        farObjects = FindObjectsOfType<farObjects>();
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
        // FindObjectOfType<AudioManager>().Play("Monster");
        //  source.Play();
        anim.SetBool("walk", shouldWalk);
        anim.SetBool("Run", shouldRun);
        distanceToTarget = Vector3.Distance(transform.position, targetPos);
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentClues < 2)
        {
            float maxDistance = 0;
            foreach (var farObject in farObjects)
            {
                if (farObject.distance > maxDistance)
                {
                    maxDistance = farObject.distance;
                    currentFarObject = farObject.transform;
                }
            }
        }
        else
        {
            heartBeatObject.SetActive(true);
            grain.intensity.value = NoiseGradient.Evaluate(distanceToPlayer);//apply grain to the camera depending on the distance to the player
        }

        // SUMMARY
        // if you have less than 9 clues but more than 2 and you are within a certain range, then be alert
        if (currentClues < 9 && currentClues > 2)// but less than 8
        {
            if (distanceToPlayer < alertRange)// and is within range
            {
                isAlert = true;
            }
            else if (canSeePlayer() && distanceToPlayer < longAlertRange)
            {
                isAlert = true;
            }
            else // and isnt within range
            {
                isAlert = false;
            }
        }
        else if (currentClues >= 9) // is more than 9, always be alert.
        {
            isAlert = true;
        }
        else
        {
            isAlert = false;
        }

        //if is alert and your flashlight is on and he can see you then chase mode.
        if (player.GetComponent<PlayerMovement>().turnOn && isAlert && canSeePlayer())
        {
            isAggresive = true;
            // isAggresive = canSeePlayer();
        }
        else if (distanceToPlayer < 10)
        {
            isAggresive = true;
        }
        else
        {
            isAggresive = false;
        }

        //protectivness is handled by the monster manager and player pickup scripts.

        if (isAggresive || (isProtective && isAlert))//if is either aggresive or (protecive and alert) then chase mode.
        {
            shouldChase = true;
        }
        else
        {
            shouldChase = false;
        }

        //kill on collection of body
        if (currentClues >= 10)
        {
            Destroy(gameObject);
        }

        //deal with eye colours
        if (shouldChase)
        {
            EyeRend1.material = AggresiveMat;
            EyeRend2.material = AggresiveMat;
        }
        else if (isAlert)
        {
            EyeRend1.material = ChaseMat;
            EyeRend2.material = ChaseMat;
        }
        else
        {
            EyeRend1.material = IdleWanderMat;
            EyeRend2.material = IdleWanderMat;
        }

        if (shouldChase)
        {
            if (!isIdling)
            {
                shouldRun = true;
                shouldWalk = false;
            }

            agent.speed = protectiveSpeed;
        }
        else
        {
            if (!isIdling)
            {
                shouldRun = false;
                shouldWalk = true;
            }

            agent.speed = normalSpeed;
        }

        //if you are alert (so chasing player) it should just constantly update to player pos
        if (isAlert || shouldChase)
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

    public bool canSeePlayer()
    {
        Vector3 dir = player.position - rayCastPos.position;
        RaycastHit hit;

        if (Physics.Raycast(rayCastPos.position, dir, out hit))
        {
            if (hit.transform.gameObject.CompareTag("Player")) return true;
            else
            {
                //Debug.Log(hit.transform.gameObject.name);
                return false;
            }
        }
        else
        {

            return false;
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
        if (currentClues < 2)
        {
            return currentFarObject.position;
        }
        else
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
}
