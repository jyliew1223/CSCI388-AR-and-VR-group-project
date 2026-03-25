using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketBehavior : MonoBehaviour
{
    public GameObject objectsTransform;
    public GameObject[] objectsActivated;
    public bool socketOccupied = false;
    public Color lightColor;

    private XRSocketInteractor socket;
    private PlayerController playerController;
    private TestRoomController roomController;
    private Vector3 playerPosition;
    private Quaternion playerRotation;
    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnObjectPlaced);

        playerController = FindFirstObjectByType<PlayerController>();
        roomController = FindFirstObjectByType<TestRoomController>();

        playerPosition = playerController.transform.position;
        playerRotation = playerController.transform.rotation;

        initialPositions = new Vector3[objectsActivated.Length];
        initialRotations = new Quaternion[objectsActivated.Length];

        for (int i = 0; i < objectsActivated.Length; i++)
        {
            if (objectsActivated[i] != null)
            {
                initialPositions[i] = objectsActivated[i].transform.position;
                initialRotations[i] = objectsActivated[i].transform.rotation;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (socketOccupied)
        {
            ActivateObjects();
        }
    }
    void ActivateObjects()
    {
        foreach (GameObject obj in objectsActivated)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        socketOccupied = false;
    }

    public void ResetRoom()
    {
        for (int i = 0; i < objectsActivated.Length; i++)
        {
            GameObject obj = objectsActivated[i];
            if (obj != null)
            {
                // Clear socket
                XRSocketInteractor socket = obj.GetComponent<XRSocketInteractor>();
                if (socket != null && socket.hasSelection)
                {
                    IXRSelectInteractable selectedObject = socket.GetOldestInteractableSelected();

                    socket.EndManualInteraction();
                    socket.interactionManager.SelectExit(socket, selectedObject);

                    if (selectedObject is MonoBehaviour mb)
                    {
                        GameObject heldObject = mb.gameObject;

                        heldObject.transform.position = new Vector3(0, -1000, 0);
                    }
                }
                obj.transform.position = initialPositions[i];
                obj.transform.rotation = initialRotations[i];
            }
        }

        playerController.transform.position = playerPosition;
        playerController.transform.rotation = playerRotation;

        }

    void OnObjectPlaced(SelectEnterEventArgs args)
    {
        if (objectsTransform != null)
        {
            foreach (Transform child in objectsTransform.transform)
            {
                child.gameObject.SetActive(false); // Deactivate each child
            }
        }

        socketOccupied = true;

        ResetRoom();

        roomController.ChangeLightColour(lightColor);
    }
}
