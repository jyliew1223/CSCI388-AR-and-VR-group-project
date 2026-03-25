using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class SocketRotateBehaviour : MonoBehaviour
{

    private XRSocketInteractor socket;
    private CustomJumpProvider jumpProvider;
    private bool isInSocket = false;
    private bool delayed = false;

    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnObjectPlaced);
        socket.selectExited.AddListener(OnObjectRemoved);

        jumpProvider = FindFirstObjectByType<CustomJumpProvider>();
    }

    void OnObjectPlaced(SelectEnterEventArgs args)
    {
        if (!isInSocket && !delayed)
        {
            isInSocket = true;
            jumpProvider.boolSwap("rb");
            jumpProvider.Jump();

            // Remove the object from the socket
            socket.interactionManager.SelectExit(socket, args.interactableObject);
        }
    }

    void OnObjectRemoved(SelectExitEventArgs args)
    {
        //Debug.Log("Object Removed");
        isInSocket = false;
        StartCoroutine(DelayFunction());
    }

    IEnumerator DelayFunction()
    {
        if (!isInSocket)
        {
            delayed = true;
            yield return new WaitForSeconds(2f);
            delayed = false;
        }
    }
}
