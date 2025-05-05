using UnityEngine;

public class Core : MonoBehaviour
{
    public static Core shared;

    private void Awake()
    {
        if (shared == null)
        {
            shared = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
