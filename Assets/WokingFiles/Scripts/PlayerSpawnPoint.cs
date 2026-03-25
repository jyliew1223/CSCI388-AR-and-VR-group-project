using System.Threading;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField]
    private bool isConsoleEcho = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning($"{this.GetType().Name}: Player not found");
        }
        else
        {
            player.transform.position = gameObject.transform.position;
            player.transform.rotation = gameObject.transform.rotation;
            if(isConsoleEcho) Debug.Log($"{this.GetType().Name}: Teleport Player to spawn point");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}