using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeController : MonoBehaviour
{
    public Transform AttackRange;
    public Transform PlayerOrientation;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AttackRange.transform.position = PlayerOrientation.position + PlayerOrientation.forward * 2f;
    }
}
