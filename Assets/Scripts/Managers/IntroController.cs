using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string mainSceneName = "Main";

    public void PlayGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}