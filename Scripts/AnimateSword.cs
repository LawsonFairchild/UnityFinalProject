using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AnimateSword : MonoBehaviour
{
    public Transform SwordArm;
    public GameObject AttackRange;
    public float SwingTime;
    private float SwingTimer;
    private bool SwingingSword;
    public float SwingingSwordRestartTime;
    private Vector3 BaseSwordLocation;
    public float SwingSpeed;

    void Start() {
        BaseSwordLocation = SwordArm.localEulerAngles;
        SwingTimer = 10;
    }

    void Update()
    {
        SwingSword();
        AnimateSwordSwing();
    }

    private void SwingSword()
    {
        if (SwingTimer > SwingingSwordRestartTime) {
            SwingingSword = false;
        }
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(0)) && !SwingingSword) {
            SwingingSword = true;
            if (!AttackRange.activeInHierarchy) {
                AttackRange.SetActive(true);
            }
            SwingTimer = 0;
        }
        if (SwingTimer > SwingTime && AttackRange.activeInHierarchy) {
                AttackRange.SetActive(false);
        }
        SwingTimer += Time.deltaTime;  
        if (SwingTimer > 10) {
            SwingTimer = 10;
        }
    }

    private void AnimateSwordSwing() {
        if (SwingTimer < SwingTime/2) {
            SwordArm.localEulerAngles = new Vector3(BaseSwordLocation.x -= SwingSpeed * Time.deltaTime, BaseSwordLocation.y, BaseSwordLocation.z);
        }
        else if (SwingTimer < SwingTime) {
            SwordArm.localEulerAngles = new Vector3(BaseSwordLocation.x += SwingSpeed * Time.deltaTime, BaseSwordLocation.y, BaseSwordLocation.z);
        }
    }
}
