using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CheatSocketBehaviour : MonoBehaviour
{
    private XRSocketInteractor socket;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSelectEntered);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
            Destroy(socket.GetOldestInteractableSelected().transform.gameObject);
    }
    void OnDestroy()
    {
        if (socket != null)
        {
            socket.selectEntered.RemoveListener(OnSelectEntered);
        }
    }
}
