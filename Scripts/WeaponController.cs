using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static IWeapon Weapon;


    public interface IWeapon
    {
        public void LeftClickAttack();
        public void RightClickAttack();
        public void LeftShiftAttack();
        public string Serialize();
    }

    public class Sword : IWeapon
    {
        public void LeftClickAttack()
        {
            // Swing Sword
        }
        public void RightClickAttack()
        {
            // Block
        }
        public void LeftShiftAttack()
        {
            // Dash Swing Sword
        }
        public string Serialize()
        {
            return "";
        }
    }

    void Start()
    {
        Weapon = new Sword();
    }

    public static void LoadFromWeaponKey(string weaponKey)
    {
        switch (weaponKey)
        {
            case "sword": Weapon = new Sword(); break;
            //case "axe": Weapon = new Axe(); break;
        }
    }
    

}
