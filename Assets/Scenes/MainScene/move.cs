using UnityEngine;
using System.Collections;
//移動を制御するスクリプト
public class Move : MonoBehaviour
{
    private bool isThrowing = false;
    private float throwVelocity = 0f;
    private float gravity = -20f; // 重力加速度
    [Header("息継ぎの時に水面から出る勢い(後で変更が必要かも)")]
    public float throwPower = 10f; // 投げ上げ初速度
    private float baseY = 0f;   // 現在のジャンプ基準Y

    // 垂直なげあげを開始する関数（基準Yを引数で指定可能）
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 5f;
        float horizontal = 0f;
        float vertical = 0f;
        float upDown = 0f;
        if(Input.GetKeyDown(KeyCode.L) && !isThrowing) ThrowUp();
        HandleThrow();
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || SerialReceive.pitchAngle <= -60) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || SerialReceive.pitchAngle >= 60) horizontal += 1f;

        if (Input.GetKey(KeyCode.UpArrow) || SerialReceive.rollAngle >= 35) upDown += 1f;
        if (Input.GetKey(KeyCode.DownArrow) || SerialReceive.rollAngle <= -35) upDown -= 1f;

        Vector3 direction = new Vector3(horizontal, upDown, vertical).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        
    }
    // 息継ぎ用関数(ジャンプ)
    void ThrowUp()
    {
        isThrowing = true;
        throwVelocity = throwPower;
        baseY = transform.position.y;
    }

    // Update内で呼び出す処理(ジャンプ)
    void HandleThrow()
    {
        Debug.Log("isThrowing: " + isThrowing);
        if (isThrowing)
        {
            throwVelocity += gravity * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y += throwVelocity * Time.deltaTime;

            if (pos.y <= baseY)
            {
                pos.y = baseY;
                isThrowing = false;
                throwVelocity = 0f;
            }
            transform.position = pos;
        }
    }
}
