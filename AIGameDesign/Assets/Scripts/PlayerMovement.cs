using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("AI variables")]
    public static Vector3 playerPos;

    [Header("Stamina Variables")]
    public float maxStamina = 5;
    private float stamina;
    public float runSpeedMult = 2;
    //public GameObject staminaBar;

    [Header("Movement variables")]
    private Vector3 moveDirection; //The direction the player is moving in
    private Rigidbody myRB; //The player's rigidbody
    public Transform cameraobject;
    public float movespeed;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();

        stamina = maxStamina;

        //Getting player position
        StartCoroutine(TrackPlayer());
    }
    //Tracking player position
    IEnumerator TrackPlayer()
    {
        while (true)
        {
            playerPos = gameObject.transform.position;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

        moveDirection = cameraobject.forward * Input.GetAxis("Vertical");
        moveDirection += cameraobject.right * Input.GetAxis("Horizontal"); //These two lines get the horizontal and vertical components of movement based on player input
        moveDirection.Normalize(); //Normalize it so it's between 0 and 1
        moveDirection.y = 0; //Make sure you aren't going up any

        Vector3 movementVelocity = moveDirection * movespeed; //Multiply by speed to get velocity


        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))//If you're sprinting and not stationary and have stamina...
        {
            movementVelocity *= runSpeedMult; //Multiply your speed by the relevant multiplier.

            stamina -= Time.deltaTime; //Reduce your stamina.
        }

        myRB.velocity = movementVelocity; //Apply that to your rigidbody


        if (!Input.GetKey(KeyCode.LeftShift) && stamina < maxStamina) //If you're not trying to sprint and your stamina is below maximum...
        {
            stamina += Time.deltaTime; //Gain stamina based on time passed.

        }

        // staminaBar.transform.localScale = new Vector3( 1000 * (stamina / maxStamina), staminaBar.transform.localScale.y, staminaBar.transform.localScale.z); //Mess with the stamina bar gameobject to shrink it based on missing stamina

    }
}
