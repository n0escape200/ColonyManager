using UnityEngine;

public class ExitApp : MonoBehaviour
{
    public void ExitGame()
    {
        // This will work in a built app
        Application.Quit();

        // This is just for debugging in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
