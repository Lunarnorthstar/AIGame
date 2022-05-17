using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartbeat : MonoBehaviour
{
    private AudioSource audioSource;

    public Transform enemy;

    public AnimationCurve volume;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        audioSource.volume = volume.Evaluate(Vector3.Distance(transform.position, enemy.position));
    }
}
