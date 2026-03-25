using UnityEngine;

public class DeflectorTracker : MonoBehaviour
{
    public DeflectorSpawner spawner;

    private void OnDestroy()
    {
        if (spawner != null)
        {
            //spawner.NotifyDeflectorDestroyed();
        }
    }
}
