using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MainGameSystem : MonoBehaviour
{
    public TMP_Text scoreText;
    public GameObject boidController;
    public GameObject explosion;
    private bool isPressed = false;
    private Vector3 originalPosition;

    // BoidControllerの移動速度（Unityインスペクターから指定可能）
    [Header("BoidControllerの移動速度（z軸負方向）")]
    public float boidControllerMoveSpeed = 10f;

    // 生成されたBoidControllerのリスト
    private List<GameObject> spawnedBoidControllers = new List<GameObject>();

    void Start()
    {
        PointMemory.point = 0;
        originalPosition = transform.localPosition;

        // コルーチンでBoidControllerの自動スポーンを開始
        StartCoroutine(SpawnBoidControllerCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // 手動スポーン（デバッグ用、必要なら残す）
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30f);
            GameObject obj = Instantiate(boidController, spawnPos, transform.rotation);
            spawnedBoidControllers.Add(obj);
        }

        // スペースキーが押されたらシーンを切り替える
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("FinishScene");
        }
        // スコアを表示
        scoreText.text = PointMemory.point.ToString();

        // リターンキーを押している間だけローカル座標を基準からz軸方向に10だけ動かす
        // リターンキーを押している間、滑らかにz軸方向に5だけ移動し、離したら元の位置に滑らかに戻る
        Animator animator = GetComponent<Animator>();
        if (Input.GetKey(KeyCode.Return))
        {
                        // アニメーションを一回再生する
            if(!isPressed) animator.SetTrigger("trigger");
            isPressed = true;
        }
        else
        {

            isPressed = false;
        }
        float targetZ = isPressed ? 5f : 0f;
        Vector3 targetPosition = originalPosition + new Vector3(0, 0, targetZ);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 8f);

        // BoidControllerの移動と破棄処理
        for (int i = spawnedBoidControllers.Count - 1; i >= 0; i--)
        {
            GameObject obj = spawnedBoidControllers[i];
            if (obj == null)
            {
                spawnedBoidControllers.RemoveAt(i);
                continue;
            }
            // z軸負方向に移動
            obj.transform.position += new Vector3(0, 0, -boidControllerMoveSpeed * Time.deltaTime);

            // 自分のz座標を超えたら破棄
            if (obj.transform.position.z <= transform.position.z)
            {
                Destroy(obj);
                spawnedBoidControllers.RemoveAt(i);
            }
        }
    }

    IEnumerator SpawnBoidControllerCoroutine()
    {
        while (true)
        {
            float waitTime = Random.Range(1f, 5f);
            yield return new WaitForSeconds(waitTime);

            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 30f);
            GameObject obj = Instantiate(boidController, spawnPos, transform.rotation);
            spawnedBoidControllers.Add(obj);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        // 衝突したオブジェクトの名前を表示
        //Debug.Log("衝突しました！対象: " + collision.gameObject.name);
        if (collision.gameObject.name == "Bone" && isPressed)
        {
            GameObject toRemove = collision.gameObject.transform.parent.parent.gameObject;
            // explositionオブジェクトをtoremoveの位置に生成
            Instantiate(explosion, toRemove.transform.position, Quaternion.identity);
            Debug.Log("消去:" + toRemove.name);
            PointMemory.point += 1200;
            Destroy(toRemove);
        }

    }
}
