using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AnimateStaff : MonoBehaviour
{
    public GameObject MagicParticles;
    public GameObject MagicBall;
    public GameObject MagicStaff;
    public float StaffRotation;
    public float RotationAngle;
    public float SpinMomentum;
    private bool StaffSwingDirection;
    private float StaffSwingTimer;
    public float StaffSwingLength;
    public float StaffSwingSpeed;

    void Start() {
        StaffRotation = 0;
    }

    void Update()
    {
        MagicParticles.transform.Rotate(0, RotationAngle * Time.deltaTime, 0, Space.Self);
        SpinBall();
        RunAnimation();
    }

    private void RunAnimation() {
        if (Input.GetKey(KeyCode.W)) {
            StaffSwingTimer += .01f;
            if (StaffSwingTimer > StaffSwingLength) {
                StaffSwingDirection = !StaffSwingDirection;
                StaffSwingTimer = 0;
            }
            if (StaffSwingDirection) {
                StaffRotation += StaffSwingSpeed;
            }
            else {
                StaffRotation -= StaffSwingSpeed;
            }
        }
        MagicStaff.transform.localRotation = Quaternion.Euler(StaffRotation + 60, MagicStaff.transform.localRotation.y, MagicStaff.transform.localRotation.z);
    }

    private void SpinBall()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SpinMomentum += 5;
        }
        MagicBall.transform.Rotate(0, RotationAngle * Time.deltaTime * SpinMomentum, 0, Space.Self);
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
