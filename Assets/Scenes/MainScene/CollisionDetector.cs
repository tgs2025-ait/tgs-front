using UnityEngine;

public class CollisionDetector : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        // 衝突したオブジェクトの名前を表示
        //Debug.Log("衝突しました！対象: " + collision.gameObject.name);
        if (collision.gameObject.name == "Bone")
        {
            GameObject toRemove = collision.gameObject.transform.parent.parent.gameObject;
            Debug.Log("消去:"+ toRemove.name);
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
