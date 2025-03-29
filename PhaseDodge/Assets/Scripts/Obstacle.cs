using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Obstacle : MonoBehaviour
{
    public float speed = 5f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        if (IsOffScreen())
        {
            Destroy(gameObject);
        }
    }

    private bool IsOffScreen()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        return screenPosition.y < 0;
    }
}
