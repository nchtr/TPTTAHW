using UnityEngine;

// Attach this to one empty GameObject in the scene.
// It creates every required game object at startup.
public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        SetupCamera();
        Ensure<GameManager>("GameManager");
        Ensure<LevelManager>("LevelManager");
        Ensure<EnemySpawner>("EnemySpawner");

        if (FindFirstObjectByType<PlayerController>() == null)
        {
            var p = new GameObject("Player");
            p.transform.position = new Vector3(0f, -3f, 0f);
            p.AddComponent<PlayerController>();
        }

        if (FindFirstObjectByType<UIManager>() == null)
            new GameObject("UIManager").AddComponent<UIManager>();
    }

    static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
        }
        cam.orthographic = true;
        cam.orthographicSize = 5.5f;
        cam.transform.position = new Vector3(0f, 0f, -10f);
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.1f);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    static void Ensure<T>(string goName) where T : Component
    {
        if (FindFirstObjectByType<T>() == null)
            new GameObject(goName).AddComponent<T>();
    }
}
