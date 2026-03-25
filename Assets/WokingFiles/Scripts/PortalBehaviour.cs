using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalBehaveiour : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Set to -100 if not using LoadSceneByIndex()")]
    private int nextSceneIndex = -100;
    [SerializeField]
    private bool isConsoleEcho = false;

    private Canvas canvas;
    private TextMeshProUGUI textMeshPro;
    private Vector3 startingGravity = new();
    void Awake()
    {
        startingGravity = Physics.gravity;
    }
    private void Start()
    {
        // Update SharedRsesource to avoid scene not loaded in order
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SharedResources.UpdateCurrentSceneIndex(currentSceneIndex);

        if (isConsoleEcho)
        {
            // for check all scene in Build Scene
            SharedResources.CheckLoadedScene();
        }

        // Next Scene Canvas
        canvas = gameObject.GetComponentInChildren<Canvas>();

        if (canvas != null)
        {
            textMeshPro = canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshPro != null)
            {
                textMeshPro.text = SharedResources.GetNextSceneName();
            }
            else
            {
                Debug.LogWarning($"{this.GetType().Name}: {canvas.name} have no chidren that have Text Mesh Pro component");
            }
        }
        else
        {
            Debug.LogWarning($"{this.GetType().Name}: {gameObject.name} have no chidren that have Canvas component");
        }
    }
    // called when a GameObject collides with the collider
    void OnTriggerEnter(Collider other)
    {
        // check whether the player collided with the trigger
        if (other.tag == "Player")
        {
            ChangeScene();
        }
    }
    void ChangeScene()
    {
        // Reset gravity before changing scene
        ResetGravity();

        if (nextSceneIndex >= 0)
        {
            // load scene br index
            int totalScenes = SceneManager.sceneCountInBuildSettings;

            if (nextSceneIndex < totalScenes)
            {
                SharedResources.LoadSceneByIndex(nextSceneIndex);
            }
            else
            {
                Debug.LogError($"{this.GetType().Name}: nextSceneIndex ({nextSceneIndex}) is out of range. Only {totalScenes} scenes are available.");
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            // load the next scene
            SharedResources.LoadNextScene();
        }
    }
    void ResetGravity()
    {
        Physics.gravity = startingGravity;
    }
}