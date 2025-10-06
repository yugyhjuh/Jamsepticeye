using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsObj;     

    public void PlayScene()
    {
        SceneManager.LoadScene("Main");

    }

    public void SettingsPage()
    {
        if (SettingsObj != null)
        {
            if (!SettingsObj.activeSelf)
            {
                SettingsObj.SetActive(true);
            }
            else
            {
                SettingsObj.SetActive(false);
            }
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game quit! (Won't show in Editor)");
    }
}

