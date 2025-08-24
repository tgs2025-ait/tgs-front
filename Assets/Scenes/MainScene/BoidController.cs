using UnityEngine;

public class BoidController : MonoBehaviour
{
    [Header("スポーン設定")]
    public GameObject boidPrefab;
    public int numBoids = 100;
    public float spawnRadius = 10f;

    [Header("Boid 設定")]
    [Range(0.1f, 20.0f)]
    public float maxSpeed = 5.0f;
    [Range(0.1f, 10.0f)]
    public float perceptionRadius = 2.5f; // 仲間を認識する範囲
    [Range(0.0f, 5.0f)]
    public float avoidanceRadius = 1.0f;  // 衝突を避けるための範囲

    [Header("ルールの重み")]
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float boundsWeight = 1.0f; // 行動範囲からの離脱を防ぐ力の重み

    [Header("行動範囲")]
    public Vector3 boundsSize = new Vector3(50, 50, 50);

    // 生成したBoidをすべて保持する配列
    [HideInInspector]
    public Boid[] allBoids;

    void Start()
    {
        
        allBoids = new Boid[numBoids];
        GameObject a = Instantiate(boidPrefab, transform.position, Quaternion.identity, transform);
        a.name = "テストのボイドだよ";
        Debug.Log("テストボイドの生成");
        for (int i = 0; i < numBoids; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject boidGO = Instantiate(boidPrefab, randomPos, Quaternion.identity, transform);
            boidGO.name = "Boid " + i;

            allBoids[i] = boidGO.GetComponent<Boid>();
            allBoids[i].Initialize(this);
        }
    }

    // Sceneビューで行動範囲を視覚化するためのGizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireCube(transform.position, boundsSize);
    }
}