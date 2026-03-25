using System;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    CustomGravityProvider gravProvider;
    PlayerController playerController;

    public float snapAngle = 45f; // Snap limit
    public float threshold = 5f; // Allowable range before snapping


    private bool playerInTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gravProvider = FindFirstObjectByType<CustomGravityProvider>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Switch Base Connected: " + GetComponent<ConfigurableJoint>().connectedBody);
        Debug.Log("Switch Rotation: " + transform.localEulerAngles.z);

        float zRotation = transform.localEulerAngles.z;

        // Convert Unity's 0-360 range to -180 to 180
        if (zRotation > 180f) zRotation -= 360f;

        // Check if rotation is near snap points
        if (Mathf.Abs(zRotation - snapAngle) < threshold)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, snapAngle);
            Debug.Log("Snapped to 45 degrees!");
        }
        else if (Mathf.Abs(zRotation + snapAngle) < threshold)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -snapAngle);
            Debug.Log("Snapped to -45 degrees!");
        }



        if (playerInTrigger && Input.GetKeyDown(KeyCode.F)) // Check if player is inside and presses F
        {
            gravProvider.InvertGravity(); // Call the function
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the collider belongs to the player
        {
            playerInTrigger = true;
        }
        Debug.Log("Press F to trigger");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Reset when player leaves the trigger
        {
            playerInTrigger = false;
        }
    }

}
