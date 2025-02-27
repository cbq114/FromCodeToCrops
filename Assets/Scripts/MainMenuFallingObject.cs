using UnityEngine;

public class MainMenuFallingObject : MonoBehaviour
{
    public float minFallSpeed = 2f, maxFallSpeed = 5f, minRotSpeed = -360f, maxRotSpeed = 360f;

    private float fallSpeed, rotSpeed;
    private float rotValue;

    public float destroyHeight = -6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        rotSpeed = Random.Range(minRotSpeed, maxRotSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        rotValue += rotSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, rotValue);

        if(transform.position.y < destroyHeight)
        {
            Destroy(gameObject);
        }
    }
}
