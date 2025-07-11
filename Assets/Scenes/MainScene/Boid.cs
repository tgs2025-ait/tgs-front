using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidController controller;
    private Vector3 velocity;

    // 初期化メソッド
    public void Initialize(BoidController boidController)
    {
        this.controller = boidController;
        // ランダムな初期速度
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * controller.maxSpeed;
    }

    void Update()
    {
        if (controller == null) return;

        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 boundsSteer = Vector3.zero;
        int perceivedNeighbors = 0;

        // 周囲のBoidをスキャン
        foreach (Boid other in controller.allBoids)
        {
            if (other == this || other == null) continue;//自身のオブジェクトか破棄されたオブジェクトの場合

            float dist = Vector3.Distance(transform.position, other.transform.position);

            if (dist > 0 && dist < controller.perceptionRadius)
            {
                // 1. 分離 (Separation)
                if (dist < controller.avoidanceRadius)
                {
                    separation -= (other.transform.position - transform.position) / (dist * dist);
                }

                // 2. 整列 (Alignment)
                alignment += other.velocity;

                // 3. 結合 (Cohesion)
                cohesion += other.transform.position;

                perceivedNeighbors++;
            }
        }

        if (perceivedNeighbors > 0)
        {
            // 各ルールの平均を計算
            cohesion /= perceivedNeighbors;
            cohesion = Steer(cohesion); // 現在位置から中心位置へのベクトルを計算

            alignment /= perceivedNeighbors;
            alignment = Steer(transform.position + alignment); // 平均速度ベクトルを目標として操舵
        }

        // 行動範囲からの逸脱を防ぐ
        if (!IsInBounds())
        {
            boundsSteer = Steer(controller.transform.position);
        }

        // 各ルールに重みを付けて力を合成
        Vector3 acceleration = Vector3.zero;
        acceleration += separation * controller.separationWeight;
        acceleration += alignment * controller.alignmentWeight;
        acceleration += cohesion * controller.cohesionWeight;
        acceleration += boundsSteer * controller.boundsWeight;

        // 速度を更新
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, controller.maxSpeed);

        // 位置と向きを更新
        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
    }

    // 目標に向かうための操舵力を計算するヘルパー関数
    private Vector3 Steer(Vector3 target)
    {
        Vector3 desired = (target - transform.position).normalized * controller.maxSpeed;
        return desired - velocity;
    }

    // 行動範囲内にいるかチェック
    private bool IsInBounds()
    {
        return transform.position.x >= controller.transform.position.x - controller.boundsSize.x / 2 &&
               transform.position.x <= controller.transform.position.x + controller.boundsSize.x / 2 &&
               transform.position.y >= controller.transform.position.y - controller.boundsSize.y / 2 &&
               transform.position.y <= controller.transform.position.y + controller.boundsSize.y / 2 &&
               transform.position.z >= controller.transform.position.z - controller.boundsSize.z / 2 &&
               transform.position.z <= controller.transform.position.z + controller.boundsSize.z / 2;
    }
}