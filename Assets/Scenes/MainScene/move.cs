using UnityEngine;

public class Move : MonoBehaviour
{
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

        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;

        if (Input.GetKey(KeyCode.UpArrow)) upDown += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) upDown -= 1f;

        Vector3 direction = new Vector3(horizontal, upDown, vertical).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        
    }
}
