using UnityEngine;

public class SocketDeflectorRotator : MonoBehaviour
{
    [SerializeField] private Transform attachTransform;

    // Called by UI button to rotate around Y (horizontal clockwise)
    public void RotateClockwise()
    {
        if (attachTransform != null)
        {
            attachTransform.Rotate(0f, 45f, 0f, Space.Self);
        }
    }

    // Called by UI button to rotate around Z (upward tilt)
    public void RotateUpward()
    {
        if (attachTransform != null)
        {
            attachTransform.Rotate(0f, 0f, 45f, Space.Self);
        }
    }

    // Called by UI button to reset rotation
    public void ResetRotation()
    {
        if (attachTransform != null)
        {
            attachTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
