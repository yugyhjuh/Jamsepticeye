using UnityEngine;

public class resolution : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject); // stays alive between scenes

        // Set your desired resolution
        Screen.SetResolution(640, 480, false); // false = windowed mode
    }
}
