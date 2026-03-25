using UnityEngine;

public class TriggerBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool isConsoleEcho = false;

    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "RotationTriggerAnchor")
        {
            if (isConsoleEcho) Debug.Log("Collision Entered!");
            isTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "RotationTriggerAnchor")
        {
            if (isConsoleEcho) Debug.Log("Collision Exited!");
            isTrigger = false;
        }
    }
    public bool GetIsTrigger()
    {
        return isTrigger;
    }
}
