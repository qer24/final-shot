using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugScript : MonoBehaviour
{
#if UNITY_STANDALONE && !UNITY_EDITOR
    private void Awake()
    {
        Destroy(gameObject);
        return;
    }
#endif

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
