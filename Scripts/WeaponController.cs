using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using System.Threading;

public class WeaponController : MonoBehaviour
{
    public static IWeapon Weapon;
    public static GameObject MeleeAttackRange;
    public interface IWeapon
    {
        public void LeftClickAttack(Transform PlayerOrientation);
        public void RightClickAttack();
        public void LeftShiftAttack();
        public string GetWeaponKey();
    }
    public class Shotgun : IWeapon
    {
        public void LeftClickAttack(Transform PlayerOrientation)
        {
            Debug.Log("weapon = shotgun\nkablam");
        }
        public void RightClickAttack()
        {
            // Block
        }
        public void LeftShiftAttack()
        {
            // Dash Swing Sword
        }
        public string GetWeaponKey() { return "shotgun"; }
    }
    public class Pistol : IWeapon
    {
        public void LeftClickAttack(Transform CameraOrientation)
        {
            Debug.Log("weapon = pistol\npew pew pew");
            RaycastHit ThingHit;
                        Debug.DrawRay(CameraOrientation.position, CameraOrientation.forward * 20000, Color.green, 5, false);
            Physics.Raycast(CameraOrientation.transform.position, CameraOrientation.forward, out ThingHit, 1000000);
            Debug.Log(ThingHit.distance);
        }
        public void RightClickAttack()
        {
            // Block
        }
        public void LeftShiftAttack()
        {
            // Dash Swing Sword
        }
        public string GetWeaponKey() { return "pistol"; }
    }
    void Start(){}
    public static void LoadFromWeaponKey(string WeaponKey)
    {
        switch (WeaponKey)
        {
            case "shotgun": Weapon = new Shotgun(); break;
            case "pistol": Weapon = new Pistol(); break;
        }
    }
}
