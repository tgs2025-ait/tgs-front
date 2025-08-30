using UnityEngine;
using System.Collections;
//移動を制御するスクリプト
public class Move : MonoBehaviour
{
    private bool isThrowing = false;
    private Vector3 throwVelocity = Vector3.zero;
    private float gravity = -20f; // 重力加速度
    [Header("息継ぎの時に水面から出る勢い(後で変更が必要かも)")]
    public float throwPower; // 投射初速度
    [Header("斜方投射の角度（度）")]
    public float throwAngle = 45f; // 斜方投射の角度
    private Vector3 basePosition = Vector3.zero;   // 現在のジャンプ基準位置
    private Vector3 throwStartPosition = Vector3.zero; // 斜方投射開始時の位置
    public GameObject orca;

    // 回転補間用
    private Quaternion targetRotation = Quaternion.identity;
    private float rotationLerpSpeed = 3f; // 補間速度

    void Start()
    {
        if (orca != null)
        {
            targetRotation = orca.transform.localRotation;
        }
        else
        {
            targetRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        float moveSpeed = 5f;
        float horizontal = 0f;
        float vertical = 0f;
        float upDown = 0f;
        if((Input.GetKeyDown(KeyCode.L) && !isThrowing)|| (SerialReceive.ayAcceleration >= 1.5f && !isThrowing)) ThrowOblique();
        HandleThrow();
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || SerialReceive.pitchAngle <= -60){
            horizontal -= 1f;
            orca.transform.localRotation = Quaternion.Euler(0, 0, -SerialReceive.pitchAngle);
        }
        if (Input.GetKey(KeyCode.D) || SerialReceive.pitchAngle >= 60) {
            horizontal += 1f;
            orca.transform.localRotation = Quaternion.Euler(0, 0, -SerialReceive.pitchAngle);
        }

        if (Input.GetKey(KeyCode.UpArrow) || SerialReceive.rollAngle >= 35) {
            upDown += 1f;
            orca.transform.localRotation = Quaternion.Euler(-SerialReceive.rollAngle, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow) || SerialReceive.rollAngle <= -35) {
            upDown -= 1f;
            orca.transform.localRotation = Quaternion.Euler(-SerialReceive.rollAngle, 0, 0);
        }

        Vector3 direction = new Vector3(horizontal, upDown, vertical).normalized;
        // 斜方投射中は通常移動を無効化
        if (!isThrowing)
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }

        // 回転を線形補間でorcaのlocalRotationに適用
        if (orca != null)
        {
            orca.transform.localRotation = Quaternion.Lerp(orca.transform.localRotation, targetRotation, Time.deltaTime * rotationLerpSpeed);
        }
    }

    // 息継ぎ用関数(斜方投射ジャンプ)
    void ThrowOblique()
    {
        isThrowing = true;
        //これやらないとシャチが回転してくれません
        orca.GetComponent<Animator>().enabled = false;

        basePosition = transform.position;
        throwStartPosition = transform.position;

        // 進行方向（前方）を基準に斜方投射
        // 水平成分（XZ平面）をtransform.forwardで取得
        Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        if (forwardXZ == Vector3.zero) forwardXZ = Vector3.forward; // 万一ゼロベクトルならZ+方向

        float rad = throwAngle * Mathf.Deg2Rad;
        // 速度ベクトルを分解
        float v = throwPower;
        float vy = v * Mathf.Sin(rad);
        float vh = v * Mathf.Cos(rad);

        throwVelocity = forwardXZ * vh + Vector3.up * vy;
    }

    // Update内で呼び出す処理(斜方投射ジャンプ)
    void HandleThrow()
    {
        if (isThrowing)
        {
            throwVelocity += Vector3.up * gravity * Time.deltaTime;
            Vector3 pos = throwStartPosition;

            // y座標のみを更新
            float deltaY = throwVelocity.y * Time.deltaTime;
            float newY = transform.position.y + deltaY;

            // 地面（ジャンプ開始位置）に戻ったら終了
            if (newY <= basePosition.y)
            {
                newY = basePosition.y;
                StartCoroutine(EnableOrcaAnimatorAfterDelay(rotationLerpSpeed));//線形補間のための時間を待ってからアニメーションを再開
                
                isThrowing = false;
                throwVelocity = Vector3.zero;
            }
            Vector3 currentPos = transform.position;
            currentPos.y = newY;
            transform.position = currentPos;

            // 速度ベクトルの向きに合わせてx軸周りに回転
            // XZ平面上の速度ベクトル
            Vector3 velocityXZ = new Vector3(throwVelocity.x, 0, throwVelocity.z);
            float speedXZ = velocityXZ.magnitude;
            float speedY = throwVelocity.y;

            // x軸回転角度（上向きが正、下向きが負）
            float angleX = 0f;
            if (speedXZ > 0.0001f)
            {
                angleX = Mathf.Atan2(speedY, speedXZ) * Mathf.Rad2Deg;
            }
            // y軸回転は進行方向に合わせる
            float angleY = 0f;
            if (velocityXZ != Vector3.zero)
            {
                angleY = Mathf.Atan2(velocityXZ.x, velocityXZ.z) * Mathf.Rad2Deg;
            }
            // 回転を適用（線形補間用にtargetRotationを更新）
            targetRotation = Quaternion.Euler(-angleX, angleY, 0f);
        }
    }

    // 線形補完のための時間を待ってからアニメーションを再開
    IEnumerator EnableOrcaAnimatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (orca != null)
        {
            Animator animator = orca.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;
                animator.Rebind();
                animator.Update(0f);
            }
        }
    }
}
