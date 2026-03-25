using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class PlayerController : MonoBehaviour, IGravityController
{
    private CustomGravityProvider m_GravityProvider;
    private float rotationSpeed = 3f; // Adjust speed as needed
    private Quaternion targetRotation;
    private bool inverted, gravityInverted, gravityChange, gravityLocked;
    private float gravityRotationCount, rotationCount;

    public Transform playerTransform;

    public bool canProcess => true;
    public bool gravityPaused => false;

    public void OnGravityLockChanged(GravityOverride gravityOverride) { }
    public void OnGroundedChanged(bool isGrounded) { }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_GravityProvider = GetComponentInChildren<CustomGravityProvider>();
        targetRotation = playerTransform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        gravityInverted = m_GravityProvider.gravityInverted;
        gravityRotationCount = m_GravityProvider.rotationCount;

        if (inverted != gravityInverted)
        {
            OnGravityInverted();
        }

        if (gravityRotationCount != rotationCount)
        {
            OnGravityRotated();
        }

        if (gravityChange)
            transformPlayer();   
    }

    public void targetRotationUpdate(float y)
    {
        targetRotation = Quaternion.AngleAxis(y, m_GravityProvider.GetCurrentUp()) * playerTransform.rotation;
        //Debug.Log(targetRotation);
    }

    public void OnGravityInverted()
    {
        inverted = gravityInverted;

        //Rotate 180 degrees clockwise
        targetRotation = Quaternion.AngleAxis(180f, playerTransform.forward) * playerTransform.rotation;

        gravityChange = true;

        Physics.gravity = -Physics.gravity;

        Debug.Log("Gravity has flipped!");
    }

    public void OnGravityRotated()
    {
        rotationCount = gravityRotationCount;

        //Rotate 90 degrees clockwise
        targetRotation = Quaternion.AngleAxis(-90f, Vector3.forward) * playerTransform.rotation;
        gravityChange = true;

        //if (!inverted)
        //{
            switch (rotationCount)
            {
                case 0:
                    Physics.gravity = new Vector3(0f, -Physics.gravity.x, 0f);
                    break;
                case 1:
                    Physics.gravity = new Vector3(Physics.gravity.y, 0f, 0f);
                    break;
                case 2:
                    Physics.gravity = new Vector3(0f, -Physics.gravity.x, 0f);
                    break;
                case 3:
                    Physics.gravity = new Vector3(Physics.gravity.y, 0f, 0f);
                    break;

            }
        /*}
        else if (inverted)
        {
            switch (rotationCount)
            {
                case 0:
                    Physics.gravity = new Vector3(0f, -Physics.gravity.y, 0f);
                    break;
                case 1:
                    Physics.gravity = new Vector3(-Physics.gravity.y, 0f, 0f);
                    break;
                case 2:
                    Physics.gravity = new Vector3(0f, Physics.gravity.y, 0f);
                    break;
                case 3:
                    Physics.gravity = new Vector3(Physics.gravity.y, 0f, 0f);
                    break;

            }
        }*/
            Debug.Log("Gravity has rotated!");
    }

    void transformPlayer()
    {
        //Lock gravity if not already locked
        if (!gravityLocked)
        {
            Vector3 nudgeDirection = m_GravityProvider.GetCurrentUp();
            float nudgeAmount = 1.5f;//Small bump to prevent intersecting geometry

            playerTransform.position += nudgeDirection * nudgeAmount;

            m_GravityProvider.TryLockGravity(this, GravityOverride.ForcedOff);
            gravityLocked = true;
        }

        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed); //Rotate the player with slerp

        //if (playerTransform.rotation == targetRotation)
        //    gravityChange = false;

        //Check if rotation is complete
        if (Quaternion.Angle(playerTransform.rotation, targetRotation) < 0.1f)
        {
            Debug.Log("Done");
            playerTransform.rotation = targetRotation; //Snap exactly to target
            gravityChange = false;

            //Unlock gravity
            m_GravityProvider.UnlockGravity(this);
            gravityLocked = false;
        }
    }

    bool IGravityController.TryLockGravity(GravityOverride gravityOverride)
    {
        throw new System.NotImplementedException();
    }

    void IGravityController.RemoveGravityLock()
    {
        throw new System.NotImplementedException();
    }

    float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
}
