using UnityEngine;

public class CleanupManager : MonoBehaviour
{
    void OnApplicationQuit()
    {
        WalkableManager.Instance.Dispose();
    }
}
