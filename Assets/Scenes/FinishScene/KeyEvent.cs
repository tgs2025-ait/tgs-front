using UnityEngine;
using UnityEngine.SceneManagement;
public class KeyEvent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
                // Enterキーが押されたときの処理
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("WelcomeScene");
        }
    }
}
