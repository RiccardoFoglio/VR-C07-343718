using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("List of scene names to load additively when this GameObject runs in the Master scene.")]
    [SerializeField]
    private string[] scenesToLoad = new string[] { "Environment", "Props", "Lighting", "Audio" };

    // Prevent double-loading if this component is recreated
    private bool hasLoaded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Only attempt to load when playing (not while editing in scene view)
        if (!Application.isPlaying)
            return;

        // Only run when the active scene is Master
        Scene active = SceneManager.GetActiveScene();
        if (active.name != "Master")
            return;

        if (!hasLoaded)
            StartCoroutine(LoadScenesAdditively());
    }

    private IEnumerator LoadScenesAdditively()
    {
        foreach (var sceneName in scenesToLoad)
        {
            if (string.IsNullOrEmpty(sceneName))
                continue;

            if (IsSceneLoaded(sceneName))
            {
                Debug.Log($"Scene '{sceneName}' is already loaded.");
                continue;
            }

            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncOp == null)
            {
                Debug.LogWarning($"Failed to start loading scene '{sceneName}'. Make sure it is added to Build Settings and the name is correct.");
                continue;
            }

            // Optionally yield until loaded
            while (!asyncOp.isDone)
                yield return null;

            Debug.Log($"Loaded scene additively: {sceneName}");
        }

        hasLoaded = true;
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.name == sceneName)
                return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
