using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ButtonHandler : MonoBehaviour
{
    public Button button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClick()
    {
        // Load the SampleScene 1 when the button is clicked
        SceneManager.LoadScene("MainGameScene");
    }
}
