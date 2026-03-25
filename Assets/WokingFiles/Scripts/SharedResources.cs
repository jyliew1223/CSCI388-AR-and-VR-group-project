using UnityEngine;
using UnityEngine.SceneManagement;

public class SharedResources
{
    private const int startingSceneIndex = 0;
    private static int currentScene = startingSceneIndex;
    private static int totalScenes = SceneManager.sceneCountInBuildSettings;
    // For checking all the scene that is loaded in BuildProfile
    public static void CheckLoadedScene()
    {

        string message = "";

        for (int i = 0; i < totalScenes; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            message += $"Scene {i}: {name}\n";
        }

        Debug.Log($"SharedResource: Total loaded scenes: {totalScenes}\n" +
                message +
                $"Current Scene: {GetCurrentSceneName()}\n" +
                $"Next Scene: {GetNextSceneName()}");
    }
    public static void LoadNextScene()
    {
        currentScene++;

        currentScene %= totalScenes;

        // load the next scene
        SceneManager.LoadScene(currentScene);
    }
    public static void LoadSceneByIndex(int index)
    {
        currentScene = index;
        // load the next scene
        SceneManager.LoadScene(currentScene);

    }
    public static void UpdateCurrentSceneIndex(int index)
    {
        currentScene = index;
    }
    public static string GetCurrentSceneName()
    {
        string output = SceneManager.GetActiveScene().name;
        if (output == "")
        {
            output = "Error!";
        }
        return output;
    }
    public static string GetNextSceneName()
    {
        string path = SceneUtility.GetScenePathByBuildIndex(currentScene + 1);
        string name = System.IO.Path.GetFileNameWithoutExtension(path);
        if (name == "")
        {
            name = "Error!";
        }
        return name;
    }
}