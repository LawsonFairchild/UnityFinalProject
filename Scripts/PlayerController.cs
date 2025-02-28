using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// PlayerController.cs
/// Lawson Fairchild <lawson.fairchild@student.cune.edu>
/// 2024-12-01
/// 
/// Control the player's movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Container showing the orientation of the player.
    /// </summary>
    public Transform Orientation;
    /// <summary>
    /// Layer detirming what objects are considered terrain.
    /// </summary>
    public LayerMask WhatIsGround;
    /// <summary>
    /// Physic material showing what the player is made out of.
    /// </summary>
    public PhysicMaterial PlayerMaterial;
    /// <summary>
    /// The gameobject that is the player
    /// </summary>
    public GameObject Player;
    public GameObject MagicBall;
    public Camera PlayerCamera;
    private RaycastHit LastWallHit;
    private float PlayerHeight;
    private float PlayerWidth;
    private float PlayerAccel;
    public float GroundedPlayerAccel;
    public float AirPlayerAccel;
    public float JumpHeight;
    public float DragForce;
    public float WallDrag;
    public float WallKickForce;
    public float WallMaxSpeed;
    public float AirMaxSpeed;
    public float GroundMaxSpeed;
    private bool Grounded;
    private bool TouchingWall;
    private bool DoubleJumpAllowed;
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
    private RaycastHit ClosestHit;
    private float ShootTime;
    public bool Paused;
    private RaycastHit UnRunableWall;
    public static bool TiltAllowed;
    public float GravityForce;
    private Vector3 PrePauseVelocity;
    private float DashTimer;
    public float DashReset;
    public float DashLength;
    public float DashMultiplier;
    private Vector3 DashDirection;
    private float VelocityTempVar;
    private bool IsDashing;

    private void Start()
    {
        PlayerRb = GetComponent<Rigidbody>();
        PlayerRb.freezeRotation = true;
        PlayerHeight = 1.2f;
        PlayerWidth = .8f;
        ShootTime = .5f;
        Paused = false;
        Physics.Raycast(Orientation.position, -Orientation.up, out LastWallHit, 10);
        Physics.Raycast(Orientation.position, -Orientation.up, out UnRunableWall, 10);
    }

    private void Update()
    {
        Pause();
        if (!Paused)
        {
            Dash();
            GravityConstantForce();
            CheckLastWall();
            ResetGame();
            ResetWallRunTimer();
            MovePlayer();
            MyInput();
            Grounded = CheckIfGrounded();
            if (Grounded)
            {
                Physics.Raycast(Orientation.position, -Orientation.up, out LastWallHit, 10);
                Physics.Raycast(Orientation.position, -Orientation.up, out UnRunableWall, 10);
            }
            TouchingWall = CheckIfTouchingWall() != 0;
            ShootTime += Time.deltaTime;
            Shoot();
            WallSlide();
            Jump();
            DoubleJump();
            WallJump();
            DetirmineIfLurchAllowed();
            Lurch();
            TimerMaxes();
            CheckIfCameraShouldTilt();
        }
    }

    private void MyInput()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
    }

    private void WallSlide()
    {
        if (TouchingWall && !Grounded && WallRunTimer < 3)
        {
            WallRunTimer += Time.deltaTime;
            PlayerMaterial.dynamicFriction = .5f;
            if (PlayerRb.velocity.y > 0)
            {
                PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, PlayerRb.velocity.y - WallDrag * Time.deltaTime, PlayerRb.velocity.z);
            }
            else
            {
                PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, -WallDrag, PlayerRb.velocity.z);
            }
            DoubleJumpAllowed = true;
            MaxSpeed = WallMaxSpeed;
            PlayerRb.position = Vector3.MoveTowards(PlayerRb.position, WallHit.point, WallHoldForce * Time.deltaTime);
        }
        else
        {
            PlayerMaterial.dynamicFriction = 1;
            MaxSpeed = GroundMaxSpeed;
        }
    }
    private void ResetGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    private void WallJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && TouchingWall && !Grounded)
        {
            WallHoldForce = 0f;
            Timer = 0;
            if (CheckIfTouchingWall() == 1)
            {
                WallKickDirection = -Orientation.right * .9f - WallHit.point + Orientation.forward * .1f;
            }
            else if (CheckIfTouchingWall() == 2)
            {
                WallKickDirection = Orientation.right * .9f - WallHit.point + Orientation.forward * .1f;
            }
            WallKickDirection.Normalize();
            Vector3 WallKickVector = WallKickDirection * WallKickForce;
            PlayerRb.velocity += WallKickVector;
            UnRunableWall = LastWallHit;
            DoubleJumpAllowed = true;
        }
        else
        {
            WallHoldForce = .2f;
        }
    }

    private void MovePlayer()
    {
        if (Grounded)
        {
            PlayerAccel = GroundedPlayerAccel;
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
        else if (PlayerRb.velocity.magnitude < AirMaxSpeed)
        {
            PlayerRb.AddForce(MoveDirection.normalized * PlayerAccel * 10f * Time.deltaTime, ForceMode.Force);
        }
    }

    private void Jump()
    {
        if (Grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Timer = 0;
            PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, JumpHeight, PlayerRb.velocity.z);
        }
        else if (Grounded)
        {
            PlayerRb.drag = DragForce;
            DoubleJumpAllowed = true;
        }
        else
        {
            PlayerRb.drag = 0;
        }
    }

    private void DoubleJump()
    {
        if (DoubleJumpAllowed && Input.GetKeyDown(KeyCode.Space))
        {
            Timer = 0;
            PlayerRb.velocity = new Vector3(PlayerRb.velocity.x, JumpHeight, PlayerRb.velocity.z);
            DoubleJumpAllowed = false;
        }
    }

    private bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, PlayerHeight, WhatIsGround);
    }

    private int CheckIfTouchingWall()
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
                return 0;
            }
            else
            {
                return 1;
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
                return 0;
            }
            else
            {
                return 2;
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
        if (collision.gameObject.tag == "DEATH")
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    private void Lurch()
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

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && DashTimer > DashReset)
        {
            DashDirection = PlayerCamera.transform.forward;
            VelocityTempVar = PlayerRb.velocity.magnitude;
            DashTimer = 0;
            PlayerRb.velocity = DashDirection * DashMultiplier;
            IsDashing = true;
        }
        DashTimer += Time.deltaTime;
        if (DashTimer > DashLength && IsDashing)
        {
            IsDashing = false;
            PlayerRb.velocity = PlayerCamera.transform.forward.normalized * VelocityTempVar;
        }
    }


    private void ResetWallRunTimer()
    {
        if (!TouchingWall && CheckIfTouchingWall() != 0)
        {
            WallRunTimer = 0;
        }
        if (WallRunTimer > 3)
        {
            WallRunTimer = 3;
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
        if (Timer < .2f)
        {
            return LurchForce;
        }
        else
        {
            return LurchForce / 2;
        }
    }

    private void TimerMaxes()
    {
        if (Timer > 10)
        {
            Timer = 10;
        }
        if (WallRunTimer > 10)
        {
            WallRunTimer = 10;
        }
        if (DashTimer > 10)
        {
            DashTimer = 10;
        }

    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && ShootTime > .5f)
        {
            GameObject MagicBallShot = Instantiate(MagicBall, PlayerCamera.transform.position + PlayerCamera.transform.forward, PlayerCamera.transform.rotation);
            MagicBallShot.GetComponent<Rigidbody>().velocity = MagicBallShot.transform.forward * 100;
        }
    }

    void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
            if (Paused)
            {
                PrePauseVelocity = PlayerRb.velocity;
                PlayerRb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                PlayerRb.constraints = RigidbodyConstraints.None;
                PlayerRb.constraints = RigidbodyConstraints.FreezeRotation;
                PlayerRb.velocity = PrePauseVelocity;
            }
        }
    }

    private void CheckLastWall()
    {
        if (TouchingWall && (CheckIfTouchingWall() == 0))
        {

        }
    }

    private void GravityConstantForce()
    {
        if (!Grounded)
        {
            PlayerRb.AddForce(Vector3.down * GravityForce * Time.deltaTime, ForceMode.Force);
        }
    }
    private void CheckIfCameraShouldTilt()
    {
        if (CheckIfTouchingWall() != 0 && !TiltAllowed)
        {
            TiltAllowed = true;
        }
        else if (TiltAllowed && CheckIfTouchingWall() == 0)
        {
            TiltAllowed = false;
        }
    }
}