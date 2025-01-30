using System;
using System.Collections.Generic;
using UnityEngine;
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

    private void Start()
    {
        foreach (Transform Enemy in Enemies)
        {
            ListOfEnemies.Add(Enemy);
        }
        timeGoing = true;
    }

    private void Update()
    {
        TimerDisp.text = Math.Round(Clock, 2).ToString() + 's';
        timeGoing = false;
        foreach (Transform Enemy in ListOfEnemies)
        {
            if (Enemy.gameObject.activeInHierarchy)
            {
                timeGoing = true;
                break;
            }
        }
        if (timeGoing)
        {
            Clock += Time.deltaTime;
        }
    }
}
