using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class farObjects : MonoBehaviour
{
    public Transform player;
    public float distance;

    void Start()
    {
        InvokeRepeating("checkDistance", 0, 0.1f);
    }

    public void checkDistance()
    {
        distance = Vector3.Distance(transform.position, player.position);
    }
}
