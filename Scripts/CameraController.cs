using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;
using Unity.Mathematics;


/// <summary>
/// CameraController.cs
/// Lawson Fairchild [lawson.fairchild@student.cune.edu]
/// 2024-12-03
/// 
/// Control the main camera to make it follow the player and look the direction the player is looking.
/// </summary>


public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Transform that shows the orientation of the player
    /// </summary>
    public Transform Orientation;
    /// <summary>
    /// Layermask that detirmines what is seen as terrain
    /// </summary>
    public LayerMask WhatIsGround;
    /// <summary>
    /// Image if a rectangle for the crosshair
    /// </summary>
    public Camera cam;
    /// <summary>
    /// The transform of the center dot
    /// </summary>
    public RectTransform rectTransform;
    public float DesiredTiltAngle;
    private float sensX;
    private float sensY;
    private bool XYSynced;
    private bool YInverted;
    private bool TouchingLeft;
    private bool TouchingRight;
    private float PlayerWidth;
    private float xRotation;
    private float yRotation;
    private float PlayerHeight;
    public bool Grounded;
    private bool Paused;
    public GameObject MenuGameObject;
    public Slider XSenseSlider;
    public Slider YSenseSlider;
    public Toggle YInvertToggle;
    public Toggle XYSyncToggle;
    private bool PrevToggle;
    public float CurrTilt;
    public float TiltSpeed;

    private void Start()
    {
        LoadSettings();
        PrevToggle = true;
        PlayerWidth = .8f;
        PlayerHeight = 1.3f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rectTransform.anchoredPosition = UnityEngine.Vector2.zero;
    }

    private void Update()
    {
        XYSynced = XYSyncToggle.isOn;
        if (PrevToggle != XYSynced)
        {
            if (XYSynced && YSenseSlider.enabled)
            {
                YSenseSlider.enabled = false;
            }
            else if (!YSenseSlider.enabled)
            {
                YSenseSlider.enabled = true;
            }
        }

        if (XYSynced)
        {
            YSenseSlider.value = XSenseSlider.value;
        }
        PrevToggle = XYSynced;

        PauseAndUnpause();
        if (!Paused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = UnityEngine.Quaternion.Euler(xRotation, yRotation, Orientation.rotation.z);
            Orientation.rotation = UnityEngine.Quaternion.Euler(0, yRotation, 0);

            TouchingLeft = TouchingWallLeft();
            TouchingRight = TouchingWallRight();

            Grounded = Physics.Raycast(Orientation.position, -Orientation.up, PlayerHeight, WhatIsGround);
            Tilt();
        }

        OpenAndCloseMenu();
    }

    private bool TouchingWallLeft()
    {
        bool RayLeft = Physics.Raycast(Orientation.position, -Orientation.right, PlayerWidth, WhatIsGround);
        bool RayLeftForward = Physics.Raycast(Orientation.position, -Orientation.right + Orientation.forward, PlayerWidth, WhatIsGround);
        bool RayLeftBack = Physics.Raycast(Orientation.position, -Orientation.right - Orientation.forward, PlayerWidth, WhatIsGround);
        return RayLeft || RayLeftBack || RayLeftForward;
    }

    private bool TouchingWallRight()
    {
        bool RayRight = Physics.Raycast(Orientation.position, Orientation.right, PlayerWidth, WhatIsGround);
        bool RayRightForward = Physics.Raycast(Orientation.position, Orientation.right + Orientation.forward, PlayerWidth / 2, WhatIsGround);
        bool RayRightBack = Physics.Raycast(Orientation.position, Orientation.right - Orientation.forward, PlayerWidth / 2, WhatIsGround);
        return RayRight || RayRightForward || RayRightBack;
    }

    private void Tilt()
    {
        if (!Grounded && PlayerController.TiltAllowed)
        {
            if (TouchingLeft && CurrTilt > -DesiredTiltAngle)
            {
                CurrTilt -= Time.deltaTime * TiltSpeed;
            }
            else if (TouchingRight && CurrTilt < DesiredTiltAngle)
            {
                CurrTilt += Time.deltaTime * TiltSpeed;
            }
        }
        else if (CurrTilt > 5) {
            CurrTilt -= Time.deltaTime * TiltSpeed;
        }
        else if (CurrTilt < -5) {
            CurrTilt += Time.deltaTime * TiltSpeed;
        }
        else {
            CurrTilt = 0;
        }
        cam.transform.localRotation = UnityEngine.Quaternion.Euler(cam.transform.localRotation.eulerAngles.x, cam.transform.localRotation.eulerAngles.y, CurrTilt);    }


    private void LoadSettings()
    {
        string FilePath = Path.Combine(Application.streamingAssetsPath, "PlayerSettings.json");
        if (File.Exists(FilePath))
        {
            string SettingsString = File.ReadAllText(FilePath);
            PlayerSettings settings = JsonUtility.FromJson<PlayerSettings>(SettingsString);
            sensX = settings.sensX;
            sensY = settings.sensY;
            YInverted = settings.YInverted;
            XYSynced = settings.XYSynced;

            if (YInverted)
            {
                sensY = -sensY;
            }

            YInvertToggle.isOn = YInverted;
            XSenseSlider.value = sensX;
            if (YInverted)
            {
                YSenseSlider.value = -sensY;
            }
            else
            {
                YSenseSlider.value = sensY;
            }

        }
        else
        {
            UnityEngine.Debug.Log("It's not working!!!!!!!");
        }
    }

    private void SaveSettings()
    {
        string FilePath = Path.Combine(Application.streamingAssetsPath, "PlayerSettings.json");
        PlayerSettings Settings = new()
        {
            sensX = XSenseSlider.value,
            sensY = YSenseSlider.value,
            YInverted = YInvertToggle.isOn
        };

        string SettingsString = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(FilePath, SettingsString);
    }

    private void PauseAndUnpause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused = !Paused;
        }
    }

    private void OpenAndCloseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuGameObject.activeInHierarchy)
            {
                MenuGameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SaveSettings();
                LoadSettings();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                MenuGameObject.SetActive(true);
            }
        }
    }
}

[System.Serializable]
public class PlayerSettings
{
    public float sensX;
    public float sensY;
    public bool YInverted;
    public bool XYSynced;
}