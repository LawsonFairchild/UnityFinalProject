using UnityEngine;

/// <summary>
/// MiniMapCamController.cs
/// Lawson Fairchild <lawson.fairchild@student.cune.edu>
/// 2024-12-06
/// 
/// Control the x and z value of the minimap camera.
/// </summary>

public class MinimapCamController : MonoBehaviour
{
    /// <summary>
    /// Provides the orientation of the player
    /// </summary>
    public Transform Orientation;
    /// <summary>
    /// Empty parent gameobject that moves to move the mini map cam
    /// </summary>
    public GameObject MinimapCamHolder;
    private void Update()
    {        
        MinimapCamHolder.transform.position = new Vector3(Orientation.position.x, 30, Orientation.position.z);
    }
}
