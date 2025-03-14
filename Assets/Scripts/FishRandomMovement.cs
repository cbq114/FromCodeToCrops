using UnityEngine;

public class FishRandomMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float moveInterval = 2f;
    public float areaRadiusX = 1.5f; // phạm vi bơi ngang
    public float areaRadiusY = 1f;   // phạm vi bơi dọc
    private Vector3 initialPosition;

    private Vector3 targetPos;
    void Start()
    {
        initialPosition = transform.position; // Lưu vị trí ban đầu của cá
        InvokeRepeating(nameof(SetRandomPosition), 0, moveInterval);
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Flip (đảo hướng cá theo hướng di chuyển)
        if ((targetPos.x - transform.position.x) < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void SetRandomPosition()
    {
        targetPos = initialPosition + new Vector3(
            Random.Range(-areaRadiusX, areaRadiusX),
            Random.Range(-areaRadiusY, areaRadiusY),
            0f
        );
    }
}
