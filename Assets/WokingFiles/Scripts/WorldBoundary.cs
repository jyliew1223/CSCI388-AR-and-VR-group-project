using UnityEngine;

public class WorldBoundary : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPosition;
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.transform.position = spawnPosition.position;
    }
}
