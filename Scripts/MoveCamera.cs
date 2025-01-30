using UnityEngine;

/// <summary>
/// MoveCamera.cs
/// Lawson Fairchild <lawson.fairchild@student.cune.edu>
/// 2024-12-03
/// 
/// Move the player camera to the same x, y, and znvalue as the player.
/// </summary>
public class MoveCamera : MonoBehaviour
{
    /// <summary>
    /// The position of the camera
    /// </summary>
    public Transform CameraPosition;
    private void Update()
    {
        transform.position = CameraPosition.position;
    }

}