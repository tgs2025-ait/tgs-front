using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Script : MonoBehaviour
{
    public Text scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log(PointMemory.point);
        scoreText.text = PointMemory.point.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("WelcomeScene");
        }
    }
}
