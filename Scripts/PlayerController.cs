using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Unity.Barracuda;
using Unity.Mathematics;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// PlayerController.cs
/// Lawson Fairchild <lawson.fairchild@student.cune.edu>
/// 2024-12-01
/// 
/// Control the player's movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Transform Orientation;
    public LayerMask WhatIsGround;
    public LayerMask WhatIsGoal;
    public PhysicMaterial PlayerMaterial;
    public Camera PlayerCamera;
    private RaycastHit LastWallHit;
    public const float PlayerHeight = 1.2f;
    private const float PlayerWidth = .8f;
    private float PlayerAccel;
    public float GroundedPlayerAccel;
    public float AirPlayerAccel;
    public float WallRunAccel;
    public float JumpHeight;
    public float DragForce;
    public float WallDrag;
    public float WallKickForce;
    public float WallMaxSpeed;
    public float AirMaxSpeed;
    public float GroundMaxSpeed;
    private bool Grounded;
    private bool TouchingWall;
    private Rigidbody PlayerRb;
    private float MaxSpeed;
    private float HorizontalInput;
    private float VerticalInput;
    private Vector3 MoveDirection;
    private Vector3 LurchDirection;
    private Vector3 WallKickDirection;
    private float Timer;
    public float LurchTimeAmount;
    public float LurchForce;
    private bool LurchAllowed;
    private float WallRunTimer;
    private RaycastHit WallHit;
    public float WallHoldForce;
    private bool HoldingWall;
    private RaycastHit ClosestHit;
    private bool Paused;
    private RaycastHit UnRunableWall;
    public static bool TiltAllowed;
    public float GravityForce;
    private Vector3 PrePauseVelocity;
    private float PowerUseTimer;
    public float PowerUseCooldown;
    public float PowerUseDuration;
    public float DashMultiplier;
    private Vector3 DashDirection;
    private float VelocityTempVar;
    private bool WasGroundedPrev;
    private bool IsDashing;
    private int PowersLeft;
    public int NumActions;
    public float WallRunDistance;
    public float TimeBeforeLurchDiminish;
    public GameObject UIImage;
    public float WallRunningDynamicFriction;
    private WallLocation DirectionTouchingWall;
    private bool TouchingWallPrev;
    public GameObject AttackRange;
    public float TimeSinceLastAttack;
    public float TimeBetweenAttacks;
    private enum WallLocation
    {
        NotTouching,
        Right,
        Left,
    }

    private void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        PlayerRb.freezeRotation = true;
        Paused = false;
        Physics.Raycast(Orientation.position, -Orientation.up, out LastWallHit, 10);
        Physics.Raycast(Orientation.position, -Orientation.up, out UnRunableWall, 10);
        WeaponController.MeleeAttackRange = AttackRange;
    }

    private void Update()
    {
        CheckPaused();
        if (!Paused)
        {
            MoveMeleeAttackRange();
            CheckLShiftPower();
            ApplyGravityConstantForce();
            CheckResetGame();
            CheckResetForWallRunTimer();
            MovePlayer();
            GetLookInput();
            if (!WasGroundedPrev && IsGrounded())
            {
                PlayerRb.AddForce(PlayerRb.velocity.normalized * 1.1f);
            }
            Grounded = IsGrounded();
            if (Grounded)
            {
                PowersLeft = NumActions;
                Physics.Raycast(Orientation.position, -Orientation.up, out LastWallHit, 10);
                Physics.Raycast(Orientation.position, -Orientation.up, out UnRunableWall, 10);
            }
            TouchingWall = IsTouchingWall();
            if (TouchingWall)
            {
                DirectionTouchingWall = CheckLocationOfWall();
                PowersLeft = NumActions;
            }
            CheckWallSlide();
            CheckJump();
            CheckDoubleJump();
            CheckWallJump();
            DetirmineIfLurchAllowed();
            CheckLurch();
            SetTimersToMaxes();
            CheckIfCameraShouldTilt();
            CheckAttacks();
            WasGroundedPrev = Grounded;
            TouchingWallPrev = TouchingWall;
        }
    }

    private void CheckAttacks()
    {
        TimeSinceLastAttack += Time.deltaTime;
        if (Input.GetMouseButtonDown(1) && TimeSinceLastAttack >= TimeBetweenAttacks)
        {
            WeaponController.Weapon.RightClickAttack();
            TimeSinceLastAttack = 0;
        }
        else if (Input.GetMouseButtonDown(0) && TimeSinceLastAttack >= TimeBetweenAttacks)
        {
            WeaponController.Weapon.LeftClickAttack(PlayerCamera.transform);
            TimeSinceLastAttack = 0;
        }
    }

    private void GetLookInput()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
    }

    private void CheckWallSlide()
    {
        if (TouchingWall && !Grounded && WallRunTimer < WallRunDistance)
        {
            WallRunTimer += Time.deltaTime;
            PlayerMaterial.dynamicFriction = WallRunningDynamicFriction;
            if (PlayerRb.velocity.y > 0)
            {
                PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, PlayerRb.velocity.y - WallDrag * Time.deltaTime, PlayerRb.velocity.z);
            }
            else
            {
                PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, -WallDrag, PlayerRb.velocity.z);
            }
            MaxSpeed = WallMaxSpeed;
            if (HoldingWall)
            {
                PlayerRb.position = Vector3.MoveTowards(PlayerRb.position, WallHit.point, WallHoldForce * Time.deltaTime);
            }
        }
        else
        {
            PlayerMaterial.dynamicFriction = 0;
            MaxSpeed = GroundMaxSpeed;
        }
    }
    private void CheckResetGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene S = SceneManager.GetActiveScene();
            SceneManager.LoadScene(S.name);
        }
    }

    private void CheckWallJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && TouchingWall && !Grounded)
        {
            HoldingWall = false;
            Timer = 0;
            if (DirectionTouchingWall == WallLocation.Right)
            {
                WallKickDirection = -Orientation.right * .9f - WallHit.point + Orientation.forward * .1f;
            }
            else if (DirectionTouchingWall == WallLocation.Left)
            {
                WallKickDirection = Orientation.right * .9f - WallHit.point + Orientation.forward * .1f;
            }
            WallKickDirection.Normalize();
            Vector3 WallKickVector = WallKickDirection * WallKickForce;
            PlayerRb.velocity += WallKickVector;
            UnRunableWall = LastWallHit;
        }
        else
        {
            if (!HoldingWall)
            {
                HoldingWall = true;
            }
        }
    }

    private void MovePlayer()
    {
        if (Grounded)
        {
            PlayerAccel = GroundedPlayerAccel;
        }
        else if (TouchingWall)
        {
            PlayerAccel = WallRunAccel;
        }
        else
        {
            PlayerAccel = AirPlayerAccel;
        }
        MoveDirection = Orientation.forward * VerticalInput + Orientation.right * HorizontalInput;
        if ((PlayerRb.velocity.magnitude < MaxSpeed) && (Grounded || TouchingWall))
        {
            PlayerRb.AddForce(MoveDirection.normalized * PlayerAccel * 10f * Time.deltaTime, ForceMode.Force);
        }
        else if (transform.InverseTransformDirection(PlayerRb.velocity).z < AirMaxSpeed && transform.InverseTransformDirection(PlayerRb.velocity).z > -AirMaxSpeed && !TouchingWall)
        {
            PlayerRb.AddForce(MoveDirection.normalized * PlayerAccel * 10f * Time.deltaTime, ForceMode.Force);
        }
    }

    private void CheckJump()
    {
        if (Grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Timer = 0;
            PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, JumpHeight, PlayerRb.velocity.z);
        }
        else if (Grounded)
        {
            PlayerRb.drag = DragForce;
        }
        else
        {
            PlayerRb.drag = 0;
        }
    }

    private void CheckDoubleJump()
    {
        if (PowersLeft > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Timer = 0;
            PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, JumpHeight, PlayerRb.velocity.z);
            if (!TouchingWall)
            {
                PowersLeft--;
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, PlayerHeight, WhatIsGround) || Physics.Raycast(transform.position, Vector3.down, PlayerHeight, WhatIsGoal);
    }

    private bool IsTouchingWall()
    {
        bool RayRight = Physics.Raycast(Orientation.position, Orientation.right, out WallHit, PlayerWidth, WhatIsGround);
        bool RayRightForward = Physics.Raycast(Orientation.position, Orientation.right + Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        bool RayRightBack = Physics.Raycast(Orientation.position, Orientation.right - Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        bool RayLeft = Physics.Raycast(Orientation.position, -Orientation.right, out WallHit, PlayerWidth, WhatIsGround);
        bool RayLeftForward = Physics.Raycast(Orientation.position, -Orientation.right + Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        bool RayLeftBack = Physics.Raycast(Orientation.position, -Orientation.right - Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        return RayRight || RayRightForward || RayRightBack || RayLeft || RayLeftForward || RayLeftBack;
    }

    private WallLocation CheckLocationOfWall()
    {
        bool RayRight = Physics.Raycast(Orientation.position, Orientation.right, out WallHit, PlayerWidth, WhatIsGround);
        ClosestHit = WallHit;
        bool RayRightForward = Physics.Raycast(Orientation.position, Orientation.right + Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        bool RayRightBack = Physics.Raycast(Orientation.position, Orientation.right - Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        bool RayLeft = Physics.Raycast(Orientation.position, -Orientation.right, out WallHit, PlayerWidth, WhatIsGround);
        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        bool RayLeftForward = Physics.Raycast(Orientation.position, -Orientation.right + Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        bool RayLeftBack = Physics.Raycast(Orientation.position, -Orientation.right - Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        bool RayForward = Physics.Raycast(Orientation.position, Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);
        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        bool RayBackwards = Physics.Raycast(Orientation.position, -Orientation.forward, out WallHit, PlayerWidth, WhatIsGround);

        if (ClosestHit.distance < WallHit.distance)
        {
            ClosestHit = WallHit;
        }
        if (RayRight || RayRightBack || RayRightForward)
        {
            if (ClosestHit.collider != null && LastWallHit.collider.name != ClosestHit.collider.name)
            {
                LastWallHit = ClosestHit;
            }
            if (ClosestHit.collider.name == UnRunableWall.collider.name)
            {
                return WallLocation.Left;
            }
            else
            {
                return WallLocation.Right;
            }
        }
        else if (RayLeft || RayLeftForward || RayLeftBack)
        {
            if (ClosestHit.collider != null && LastWallHit.collider.name != ClosestHit.collider.name)
            {
                LastWallHit = ClosestHit;
            }
            if (ClosestHit.collider.name == UnRunableWall.collider.name)
            {
                return WallLocation.NotTouching;
            }
            else
            {
                return WallLocation.Left;
            }
        }
        else
        {
            if (TouchingWall)
            {
                UnRunableWall = LastWallHit;
            }
            return 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DEATH"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void CheckLurch()
    {
        if (LurchAllowed)
        {
            LurchDirection = Orientation.right * HorizontalInput;
            if (Input.GetKeyDown(KeyCode.D))
            {
                PlayerRb.velocity = new Vector3(LurchDirection.normalized.x * DetirmineLurchForce() + PlayerRb.velocity.x, PlayerRb.velocity.y, LurchDirection.normalized.z * LurchForce + PlayerRb.velocity.z);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                PlayerRb.velocity = new Vector3(LurchDirection.normalized.x * DetirmineLurchForce() + PlayerRb.velocity.x, PlayerRb.velocity.y, LurchDirection.normalized.z * LurchForce + PlayerRb.velocity.z);
            }
        }
    }

    private void CheckLShiftPower()
    {
        if (PowersLeft > 0 && Input.GetKeyDown(KeyCode.LeftShift) && PowerUseTimer > PowerUseCooldown)
        {
            DashDirection = (PlayerCamera.transform.forward * 1.5f + PlayerRb.velocity.normalized).normalized;
            VelocityTempVar = PlayerRb.velocity.magnitude;
            PowerUseTimer = 0;
            PlayerRb.velocity = DashDirection * DashMultiplier;
            IsDashing = true;
            PowersLeft--;
            if (TimeSinceLastAttack < TimeBetweenAttacks)
            {
                WeaponController.Weapon.LeftShiftAttack();
                TimeSinceLastAttack = 0;
            }
        }
        PowerUseTimer += Time.deltaTime;
        if (PowerUseTimer > PowerUseDuration && IsDashing)
        {
            IsDashing = false;
            PlayerRb.velocity = 1.1f * VelocityTempVar * PlayerRb.velocity.normalized;
        }
    }


    private void CheckResetForWallRunTimer()
    {
        if (!TouchingWallPrev && IsTouchingWall())
        {
            WallRunTimer = 0;
        }
        if (WallRunTimer > WallRunDistance)
        {
            WallRunTimer = WallRunDistance;
        }
    }

    private void DetirmineIfLurchAllowed()
    {
        Timer += Time.deltaTime;
        if (Timer > 10)
        {
            Timer = 10;
        }
        if (Timer < LurchTimeAmount && !Grounded && !TouchingWall)
        {
            LurchAllowed = true;
        }
        else
        {
            LurchAllowed = false;
        }
    }

    private float DetirmineLurchForce()
    {
        if (Timer < TimeBeforeLurchDiminish)
        {
            return LurchForce;
        }
        else
        {
            return LurchForce / 2;
        }
    }

    private void SetTimersToMaxes()
    {
        if (Timer > 10)
        {
            Timer = 10;
        }
        if (WallRunTimer > 10)
        {
            WallRunTimer = 10;
        }
        if (PowerUseTimer > 10)
        {
            PowerUseTimer = 10;
        }
        if (TimeSinceLastAttack > TimeBetweenAttacks)
        {
            TimeSinceLastAttack = TimeBetweenAttacks;
        }

    }
    void CheckPaused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
            if (Paused)
            {
                PrePauseVelocity = PlayerRb.velocity;
                PlayerRb.constraints = RigidbodyConstraints.FreezeAll;
                UIImage.SetActive(false);
            }
            else
            {
                PlayerRb.constraints = RigidbodyConstraints.None;
                PlayerRb.constraints = RigidbodyConstraints.FreezeRotation;
                PlayerRb.velocity = PrePauseVelocity;
                UIImage.SetActive(true);
            }
        }
    }

    private void ApplyGravityConstantForce()
    {
        if (!Grounded)
        {
            PlayerRb.AddForce(Vector3.down * GravityForce * Time.deltaTime, ForceMode.Force);
        }
    }
    private void CheckIfCameraShouldTilt()
    {
        if (TouchingWall && !TiltAllowed)
        {
            TiltAllowed = true;
        }
        else if (TiltAllowed && !TouchingWall)
        {
            TiltAllowed = false;
        }
    }
    
    private void MoveMeleeAttackRange()
    {
        AttackRange.transform.position = Orientation.position + Orientation.forward * 2f;
    }
}