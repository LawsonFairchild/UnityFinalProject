using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateStaff : MonoBehaviour
{
    public GameObject MagicParticles;
    public GameObject MagicStaff;
    public float rotationAngle;
    public float SpinMomentum;

    // Update is called once per frame
    void Update()
    {
        MagicParticles.transform.Rotate(0, rotationAngle * Time.deltaTime, 0, Space.Self);
        SpinStaff();
    }

    private void SpinStaff()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SpinMomentum += 5;
        }
        MagicStaff.transform.Rotate(0, rotationAngle * Time.deltaTime * SpinMomentum, 0, Space.Self);
        if (SpinMomentum > 0)
        {
            SpinMomentum -= .01f;
        }
        else
        {
            SpinMomentum = 0;
        }
        if (SpinMomentum > 3)
        {
            SpinMomentum = 3;
        }
        
    }
}
