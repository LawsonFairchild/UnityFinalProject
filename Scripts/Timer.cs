using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Timer.cs
/// Lawson Fairchild <lawson.fairchild@student.cune.edu>
/// 2024-12-10
/// 
/// Run a timer until all enemies are killed.
/// </summary>

public class Timer : MonoBehaviour
{
    /// <summary>
    /// The text box that the time is displayed in.
    /// </summary>
    public Text TimerDisp;
    /// <summary>
    /// The location of the enemies
    /// </summary>
    public Transform Enemies;
    /// <summary>
    /// A list of all the enemies in the game
    /// </summary>
    public List<Transform> ListOfEnemies;
    private bool timeGoing;
    private float Clock;
    public LayerMask Goal;
    public Transform PlayerTransform;
    public static float Level1Time;
    public static float Level2Time;
    public TextMeshProUGUI Level1TimeText;
    public TextMeshProUGUI Level2TimeText;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "LevelSelector")
        {
            Level1TimeText.text = "Time: " + Math.Round(Level1Time, 2);
            Level2TimeText.text = "Time: " + Math.Round(Level2Time, 2);
        }
        else
        {
            foreach (Transform Enemy in Enemies)
            {
                ListOfEnemies.Add(Enemy);
            }
            timeGoing = true;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "LevelSelector")
        {
            TimerDisp.text = Math.Round(Clock, 2).ToString() + 's';
            timeGoing = false;
            foreach (Transform Enemy in ListOfEnemies)
            {
                //for every enemy if the enemy is still alive (layer 9) continue timing
                if (Enemy.gameObject.layer == 7)
                {
                    timeGoing = true;
                    break;
                }
            }
            if (timeGoing)
            {
                Clock += Time.deltaTime;
            }
            CheckIfTouchingGoal();
        }
    }

    void CheckIfTouchingGoal()
    {
        if (!timeGoing && Physics.Raycast(PlayerTransform.position, Vector3.down, PlayerController.PlayerHeight, Goal))
        {
            if (SceneManager.GetActiveScene().name == "Level1")
            {
                Level1Time = Clock;
            }
            else if (SceneManager.GetActiveScene().name == "Level2")
            {
                Level2Time = Clock;
            }
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("LevelSelector");
        }
    }
}
