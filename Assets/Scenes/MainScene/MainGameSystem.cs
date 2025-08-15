using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MainGameSystem : MonoBehaviour
{
    public TMP_Text scoreText;
    private bool isPressed = false;
    private Vector3 originalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PointMemory.point = 0;
        
        originalPosition = transform.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        // シフトキーが押されたらシーンを切り替える
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("FinishScene");
        }
        scoreText.text = "Score: " + PointMemory.point.ToString();
        // リターンキーを押している間だけローカル座標を基準からz軸方向に10だけ動かす
        // リターンキーを押している間、滑らかにz軸方向に5だけ移動し、離したら元の位置に滑らかに戻る
        if (Input.GetKey(KeyCode.Return))
        {
            isPressed = true;
        }
        else
        {
            isPressed = false;
        }
        float targetZ = isPressed ? 5f : 0f;
        Vector3 targetPosition = originalPosition + new Vector3(0, 0, targetZ);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 8f);
    }
    void OnTriggerEnter(Collider collision)
    {
        // 衝突したオブジェクトの名前を表示
        //Debug.Log("衝突しました！対象: " + collision.gameObject.name);
        if (collision.gameObject.name == "Bone" && isPressed)
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
