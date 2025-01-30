using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

/// <summary>
/// EnemyController.cs
/// Lawson Fairchild <lawson.fairchild@student.cune.edu>
/// 2024-12-05
/// 
/// Control the status of the enemies.
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// Game object that is the enemy
    /// </summary>
    public GameObject Enemy;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name != "Player") 
        {
            Destroy(collision.gameObject);
            gameObject.SetActive(false);
        }
    }
}
