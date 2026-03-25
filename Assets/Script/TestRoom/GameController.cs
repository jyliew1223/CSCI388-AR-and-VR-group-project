using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private float timer = 0f;
    private bool isRunning = true;

    [SerializeField] private float timeDisplay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;
            timeDisplay = timer;
        }
    }

    public float GetTimer()
    {
        return timer;
    }

    public void ResetTimer()
    {
        timer = 0f;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }
}