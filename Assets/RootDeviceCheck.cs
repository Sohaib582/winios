using UnityEngine;
using UnityEngine.SceneManagement;

public class RootDeviceCheck : MonoBehaviour
{
    public static ApplicationSandboxType sandboxType;

    private void Start()
    {
        if (Application.sandboxType == ApplicationSandboxType.SandboxBroken)
        {
            Application.Quit();
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "main")
                {
                    //UnityEditor.EditorApplication.isPlaying = false;
                    Application.Quit();
                }
            }
        }
    }
}