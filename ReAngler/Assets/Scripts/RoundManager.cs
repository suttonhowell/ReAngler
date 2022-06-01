using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : MonoBehaviour
{
    GameObject target;

    public RoundManager(){

    }
    // Start is called before the first frame update
    void Start()
    {
        //find the target
        target = GameObject.Find("centerTarget");
        //verify target found
        if (target == null) throw new Exception("Target could not be found");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
