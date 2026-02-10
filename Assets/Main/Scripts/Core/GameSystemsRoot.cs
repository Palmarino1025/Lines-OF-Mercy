using UnityEngine;

public class GameSystemsRoot : MonoBehaviour
{
    private static GameSystemsRoot instance;

    private void Awake()
    {
        // If another GameSystems already exists, destroy this duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // root object only
    }
}
