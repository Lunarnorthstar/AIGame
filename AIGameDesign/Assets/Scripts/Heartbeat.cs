using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartbeat : MonoBehaviour
{
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        

        if (other.tag == "Enemy")
        {
            Debug.Log("TEST");
            audioSource.Play(0);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            audioSource.Pause();
        }
    }
}
