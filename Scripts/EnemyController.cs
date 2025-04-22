using Unity.VisualScripting;
using UnityEngine;
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
    private float DespawnTimer;
    public Collider EnemyCollider;
    private Rigidbody[] RigidbodyArray;
    private Transform[] GameObjectArray;

    void Update()
    {
        if (Enemy.layer == 8 && DespawnTimer < 5) {
            DespawnTimer += Time.deltaTime;
        }
        if (DespawnTimer >= 5) {
            Enemy.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name != "Player") 
        {
            Enemy.layer = 8;
            EnemyCollider.enabled = false;
            RigidbodyArray = Enemy.GetComponentsInChildren<Rigidbody>();
            GameObjectArray = Enemy.GetComponentsInChildren<Transform>();
            for (int i = 0; i < RigidbodyArray.Length; i++) {
                RigidbodyArray[i].isKinematic = false;
            }
            for (int i = 0; i < GameObjectArray.Length; i++) {
                GameObjectArray[i].gameObject.layer = 8;
            }
        }
    }
}
