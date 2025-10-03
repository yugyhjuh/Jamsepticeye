using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsObject;

    // Load a scene by its name
    public void PlayGame()
    {
        SceneManager.LoadScene("sceneName");
    }

    public void Settings()
    {
        if (SettingsObject != null)
        {
            if (!SettingsObject.activeSelf)
            {
                SettingsObject.SetActive(true);
            }
            else
            {
                SettingsObject.SetActive(false);
            }
        }
    }

    // Quit the application (works in build, not in editor)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit!");
    }
}
