using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    private const string MappingApp = "MappingApp";
    private const string ContentPlacementSample = "ContentPlacementSample";

    public void SwitchScene()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentSceneName != MappingApp ? MappingApp : ContentPlacementSample);
    }
}
