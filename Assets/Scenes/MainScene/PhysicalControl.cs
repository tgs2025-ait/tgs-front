using UnityEngine;

public class PhysicalControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationSpeed = 120f; // 回転速度を調整するための変数

        // QキーでX軸正の向きに回転
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }

        // ZキーでX軸負の向きに回転
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
        }
    }
}
