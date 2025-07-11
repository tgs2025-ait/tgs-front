using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MainGameSystem : MonoBehaviour
{
    public TMP_Text scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PointMemory.point = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        // シフトキーが押されたらシーンを切り替える
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("FinishScreen");
        }
        scoreText.text = "Score: " + PointMemory.point.ToString();
        
    }
    void OnTriggerEnter(Collider collision)
    {
        // 衝突したオブジェクトの名前を表示
        //Debug.Log("衝突しました！対象: " + collision.gameObject.name);
        if (collision.gameObject.name == "Bone")
        {
            GameObject toRemove = collision.gameObject.transform.parent.parent.gameObject;
            Debug.Log("消去:"+ toRemove.name);
            PointMemory.point += 1200;
            Destroy(toRemove);


        }
        
        // 衝突情報を取得する例
        // ContactPoint[] contacts = collision.contacts;
        // foreach (ContactPoint contact in contacts)
        // {
        //     Debug.Log("接触点位置: " + contact.point);
        // }
    }
}
