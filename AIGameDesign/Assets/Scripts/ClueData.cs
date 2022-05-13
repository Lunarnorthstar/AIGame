using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueData : MonoBehaviour
{
    public GameObject modelPrefab;

    public String clueType;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(modelPrefab, gameObject.transform);
    }
}
