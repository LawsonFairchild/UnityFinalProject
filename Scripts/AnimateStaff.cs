using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateStaff : MonoBehaviour
{
    public GameObject Staff;
    public float rotationAngle;

    // Update is called once per frame
    void Update()
    {
        Staff.transform.Rotate(0, rotationAngle * Time.deltaTime, 0, Space.Self);
    }
}
